import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, takeUntil } from 'rxjs';

import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { ConfirmComponent } from '../confirm/confirm.component';
import { PaginationParams, PaginatedResponse } from '../../services/generic-crud.service';
import { GenericTableComponent, TableConfig, TableColumn, TableAction, TableFilter } from '../generic-table/generic-table.component';

@Component({
  selector: 'app-user-table',
  standalone: true,
  imports: [GenericTableComponent],
  templateUrl: './user-table.component.html',
  styleUrls: ['./user-table.component.scss']
})
export class UserTableComponent implements OnInit, OnDestroy {
  dataSource = new MatTableDataSource<User>([]);

  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  loading = false;
  error: string | null = null;
  tableConfig: TableConfig = {
    title: 'Users Management',
    subtitle: 'Manage and monitor user accounts',
    enableSelection: true,
    enableExport: true,
    enableBulkActions: true,
    pageSizeOptions: [5, 10, 25, 50, 100],
    defaultPageSize: 10,
    columns: [
      { key: 'avatar', label: 'Avatar', type: 'avatar', width: '60px' },
      { key: 'userName', label: 'Username', type: 'text', sortable: true },
      { key: 'email', label: 'Email', type: 'email', sortable: true },
      { key: 'firstName', label: 'First Name', type: 'text', sortable: true },
      { key: 'lastName', label: 'Last Name', type: 'text', sortable: true },
      { key: 'phone', label: 'Phone', type: 'phone' },
      { key: 'roles', label: 'Roles', type: 'roles' },
      { key: 'isActive', label: 'Status', type: 'status', sortable: true },
      { key: 'createdAt', label: 'Created', type: 'date', sortable: true },
      { key: 'lastLoginAt', label: 'Last Login', type: 'date', sortable: true },
      { key: 'actions', label: 'Actions', type: 'actions', width: '120px' }
    ],
    actions: [
      { key: 'edit', label: 'Edit', icon: 'edit', color: 'primary', tooltip: 'Edit User' },
      { key: 'toggle', label: 'Toggle Status', icon: 'toggle_on', color: 'accent', tooltip: 'Toggle Status' },
      { key: 'delete', label: 'Delete', icon: 'delete', color: 'warn', tooltip: 'Delete User' }
    ],
    filters: [
      {
        key: 'isActive', label: 'Status', type: 'select', options: [
          { value: true, label: 'Active' },
          { value: false, label: 'Inactive' }
        ]
      },
      {
        key: 'roles', label: 'Role', type: 'select', options: [
          { value: 'admin', label: 'Admin' },
          { value: 'user', label: 'User' },
          { value: 'moderator', label: 'Moderator' }
        ]
      }
    ]
  };

  private destroy$ = new Subject<void>();

  constructor(
    private userService: UserService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUsers(searchTerm?: string, sortBy?: string, sortDirection?: string, filters?: Record<string, any>): void {
    this.loading = true;
    this.error = null;

    const params: PaginationParams = {
      page: this.pageIndex + 1,
      pageSize: this.pageSize,
      searchTerm: searchTerm,
      sortBy: sortBy || 'createdAt',
      sortDirection: (sortDirection === 'asc' || sortDirection === 'desc') ? sortDirection : 'desc',
      filters: filters || {}
    };

    this.userService.getAll(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: PaginatedResponse<User>) => {
          this.dataSource.data = response.data;
          this.totalCount = response.totalCount;
          this.loading = false;
        },
        error: (error) => {
          this.error = error;
          this.loading = false;
          this.showError('Failed to load users');
        }
      });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }

  onSortChange(sort: Sort): void {
    this.loadUsers(undefined, sort.active, sort.direction);
  }

  onSearchChange(searchTerm: string): void {
    this.pageIndex = 0;
    this.loadUsers(searchTerm);
  }

  onFilterChange(filters: Record<string, any>): void {
    this.pageIndex = 0;
    this.loadUsers(undefined, undefined, undefined, filters);
  }

  onRefresh(): void {
    this.loadUsers();
  }

  onExport(): void {
    this.userService.exportToCsv({
      searchTerm: undefined,
      sortBy: 'createdAt',
      sortDirection: 'desc'
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (csvData) => {
          this.downloadCsv(csvData, 'users.csv');
          this.showSuccess('Users exported successfully');
        },
        error: (error) => {
          this.showError('Failed to export users');
        }
      });
  }

  onActionClick(event: { action: string; item: User }): void {
    const { action, item } = event;

    switch (action) {
      case 'edit':
        this.editUser(item);
        break;
      case 'delete':
        this.deleteUser(item);
        break;
      case 'toggle':
        this.toggleUserStatus(item);
        break;
    }
  }

  onBulkActionClick(event: { action: string; items: User[] }): void {
    const { action, items } = event;

    switch (action) {
      case 'delete':
        this.bulkDelete(items);
        break;
      case 'toggle':
        this.bulkToggleStatus(items);
        break;
    }
  }

  onSelectionChange(selectedItems: User[]): void {
    console.log('Selected items:', selectedItems);
  }

  editUser(user: User): void {
    console.log('Edit user:', user);
    this.showSuccess(`Edit user: ${user.userName}`);
  }

  deleteUser(user: User): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete User',
        message: `Are you sure you want to delete user "${user.userName}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.delete(user.id!)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.showSuccess('User deleted successfully');
              this.loadUsers();
            },
            error: (error) => {
              this.showError('Failed to delete user');
            }
          });
      }
    });
  }

  toggleUserStatus(user: User): void {
    const newStatus = !user.isActive;
    const action = newStatus ? 'activate' : 'deactivate';

    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: '400px',
      data: {
        title: `${action.charAt(0).toUpperCase() + action.slice(1)} User`,
        message: `Are you sure you want to ${action} user "${user.userName}"?`,
        confirmText: action.charAt(0).toUpperCase() + action.slice(1),
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.update(user.id!, { isActive: newStatus })
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.showSuccess(`User ${action}d successfully`);
              this.loadUsers();
            },
            error: (error) => {
              this.showError(`Failed to ${action} user`);
            }
          });
      }
    });
  }

  bulkDelete(users: User[]): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete Users',
        message: `Are you sure you want to delete ${users.length} selected users?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const ids = users.map(user => user.id!);
        this.userService.bulkDelete(ids)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.showSuccess('Users deleted successfully');
              this.loadUsers();
            },
            error: (error) => {
              this.showError('Failed to delete users');
            }
          });
      }
    });
  }

  bulkToggleStatus(users: User[]): void {
    const activeUsers = users.filter(user => user.isActive);
    const inactiveUsers = users.filter(user => !user.isActive);

    let action: string;
    let newStatus: boolean;

    if (activeUsers.length > inactiveUsers.length) {
      action = 'deactivate';
      newStatus = false;
    } else {
      action = 'activate';
      newStatus = true;
    }

    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: '400px',
      data: {
        title: `${action.charAt(0).toUpperCase() + action.slice(1)} Users`,
        message: `Are you sure you want to ${action} ${users.length} selected users?`,
        confirmText: action.charAt(0).toUpperCase() + action.slice(1),
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const updates = users.map(user => ({
          id: user.id,
          isActive: newStatus
        }));

        this.userService.bulkUpdate(updates)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.showSuccess(`Users ${action}d successfully`);
              this.loadUsers();
            },
            error: (error) => {
              this.showError(`Failed to ${action} users`);
            }
          });
      }
    });
  }

  private downloadCsv(csvData: string, filename: string): void {
    const blob = new Blob([csvData], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }
}
