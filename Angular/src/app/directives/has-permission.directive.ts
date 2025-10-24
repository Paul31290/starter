import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy, inject } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PermissionService } from '../services/permission.service';

/**
 * Structural directive to conditionally display elements based on user permissions
 * 
 * Usage:
 * <div *hasPermission="'Products_Create'">Create Product</div>
 * <div *hasPermission="['Products_Create', 'Products_Update']">Edit Product</div>
 * <div *hasPermission="['Products_Create', 'Products_Update']; requireAll: true">Need both permissions</div>
 */
@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective implements OnInit, OnDestroy {
  private readonly templateRef = inject(TemplateRef<any>);
  private readonly viewContainer = inject(ViewContainerRef);
  private readonly permissionService = inject(PermissionService);
  
  private destroy$ = new Subject<void>();
  private hasView = false;
  private permissions: string[] = [];
  private requireAll = false;

  /**
   * Set the permission(s) required to display the element
   * Can be a single permission string or an array of permissions
   */
  @Input() set hasPermission(val: string | string[]) {
    this.permissions = Array.isArray(val) ? val : [val];
    this.updateView();
  }

  /**
   * Set whether all permissions are required (AND logic) or any permission (OR logic)
   * Default is false (OR logic - any permission is sufficient)
   */
  @Input() set hasPermissionRequireAll(val: boolean) {
    this.requireAll = val;
    this.updateView();
  }

  ngOnInit(): void {
    this.updateView();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Update the view based on permission check
   */
  private updateView(): void {
    if (!this.permissions || this.permissions.length === 0) {
      this.showElement();
      return;
    }

    const hasPermission = this.requireAll
      ? this.permissionService.hasAllPermissionsSync(this.permissions)
      : this.permissionService.hasAnyPermissionSync(this.permissions);

    if (hasPermission) {
      this.showElement();
    } else {
      this.hideElement();
    }
  }

  /**
   * Show the element by creating the embedded view
   */
  private showElement(): void {
    if (!this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    }
  }

  /**
   * Hide the element by clearing the view container
   */
  private hideElement(): void {
    if (this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}

