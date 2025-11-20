import { Component, OnInit } from '@angular/core';
import { AuthUser } from '../../models/auth.model';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SettingsService } from '../../services/settings.service';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { User, NewUser } from '../../models/user.model';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-settings',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatIconModule
  ],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  currentUser: User | null = null;
  loading = false;
  showSuccess = false;
  errorMessage = '';
  returnUrl = '';
  SettingsUsernameForm: FormGroup;
  SettingsProfilePictureForm: FormGroup;
  file = "";
  userProfilePicture = "";

  constructor(
    private authService: AuthService,
    private settingsService: SettingsService,
    private router: Router,
    private formBuilder: FormBuilder,
  ) {
    this.SettingsUsernameForm = this.formBuilder.group({
      updatedUserName: ['', [Validators.minLength(3), Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9_]+$/)]],
    });
    this.SettingsProfilePictureForm = this.formBuilder.group({
      updatedProfilePicture: ['', [Validators.minLength(3), Validators.maxLength(1000000)]]
    });
  }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.getDynamicProfilePicture();
    })
  }
  
  updatingUsername(): void {
    if (this.SettingsUsernameForm.valid && this.currentUser) {
      this.loading = true;
      this.showSuccess = false;
      this.errorMessage = '';

      const updatedUserName : NewUser = {
        newUserName: this.SettingsUsernameForm.value.updatedUserName,
        newProfilePicture: ""
      }

      this.settingsService.ChangeUserName(this.currentUser.id, updatedUserName).subscribe({
        next: (response) => {
          this.loading = false;
          this.showSuccess = true;
          if(this.currentUser){
            this.currentUser.userName = updatedUserName.newUserName;
          }
        },
        error: (error) => {
          console.error('Error updating username:', error);
          this.loading = false;
          this.errorMessage = 'Failed to update username. Please try again.';
        }
      });
    } else {
      this.markUsernameFormGroupTouched();
    }
  }

  async updatingProfilePicture(event: any): Promise<void> {
    if (this.currentUser) {
      this.loading = true;
      this.showSuccess = false;
      this.errorMessage = '';

      try {
        const base64file = await this.readFileProfilePicture(event);

        const updatedProfilePicture : NewUser = {
          newUserName: "",
          newProfilePicture: base64file,
        }

        this.settingsService.changeProfilePicture(this.currentUser.id, updatedProfilePicture).subscribe({
          next: (response) => {
            this.loading = false;
            this.showSuccess = true;
            if(this.currentUser){
              this.currentUser.profilePicture = updatedProfilePicture.newProfilePicture;
              this.userProfilePicture = this.currentUser.profilePicture;
            }
          },
          error: (error) => {
            console.error('Error updating profile picture:', error);
            this.loading = false;
            this.errorMessage = 'Failed to update the profile picture. Please try again.';
          }
        });
      } catch (err) {
        console.error('Error reading file:', err);
        this.loading = false;
        this.errorMessage = 'Failed to read the selected file.';
      } 
    } else {
      this.markProfilePictureFormGroupTouched();
    }
  }
 
  private async readFileProfilePicture(event: any): Promise<string>{
    const files = event.target.files as FileList;

    if (files.length > 0) {
      const _file = URL.createObjectURL(files[0]);
      this.file = _file;
      this.resetInput();   
    }
    return this.file;
  }

  private resetInput(){
    const input = document.getElementById('avatar-input-file') as HTMLInputElement;
    if(input){
      input.value = "";
    }
  }

  private markUsernameFormGroupTouched(): void {
    Object.keys(this.SettingsUsernameForm.controls).forEach(key => {
      const control = this.SettingsUsernameForm.get(key);
      control?.markAsTouched();
    });
  }

  private markProfilePictureFormGroupTouched(): void {
    Object.keys(this.SettingsProfilePictureForm.controls).forEach(key => {
      const control = this.SettingsProfilePictureForm.get(key);
      control?.markAsTouched();
    });
  }

  private async getDynamicProfilePicture(): Promise<void>{
    if (!this.currentUser?.id) {
      this.userProfilePicture = '';
      return;
    }

    this.settingsService.getProfilePicture(this.currentUser.id).subscribe({
      next: async (response) => {
        let dataUrl: string;
        if (response.startsWith('data:')) {
          dataUrl = response;
        } else {
          this.isBase64(response)
          dataUrl = `${response}`;
        }
        this.userProfilePicture = dataUrl;

      },
      error: (error) => {
      console.error('Error getting the profile picture:', error);
    }
    });
  }

  private isBase64(str: string): boolean {
    // basic heuristic — adjust as needed
    return !!str && /^[A-Za-z0-9+/=\s]+$/.test(str) && str.length % 4 === 0;
  }

  goHome(): void {
    this.router.navigate(['/home']);
  }
}