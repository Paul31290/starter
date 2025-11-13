import { Component, OnInit, OnDestroy, Input, Output, EventEmitter, ViewChild, TemplateRef } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, PageEvent, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, Sort, MatSortModule } from '@angular/material/sort';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SelectionModel } from '@angular/cdk/collections';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';

import { PaginationParams, PaginatedResponse } from '../../services/generic-crud.service';

export interface TableColumn {
  key: string;
  label: string;
  type: 'text' | 'avatar' | 'status' | 'date' | 'email' | 'phone' | 'roles' | 'actions' | 'custom';
  sortable?: boolean;
  width?: string;
  customTemplate?: TemplateRef<any>;
}

export interface TableAction {
  key: string;
  label: string;
  icon: string;
  color: 'primary' | 'accent' | 'warn';
  tooltip: string;
  condition?: (item: any) => boolean;
}

export interface TableFilter {
  key: string;
  label: string;
  type: 'text' | 'select' | 'date';
  options?: { value: any; label: string }[];
  placeholder?: string;
}

export interface TableConfig {
  title: string;
  subtitle?: string;
  columns: TableColumn[];
  actions: TableAction[];
  filters?: TableFilter[];
  enableSelection?: boolean;
  enableExport?: boolean;
  enableBulkActions?: boolean;
  pageSizeOptions?: number[];
  defaultPageSize?: number;
}

@Component({
  selector: 'app-generic-table',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './generic-table.component.html',
  styleUrls: ['./generic-table.component.scss']
})
export class GenericTableComponent<T = any> implements OnInit, OnDestroy {
  @Input() config!: TableConfig;
  @Input() dataSource = new MatTableDataSource<T>([]);
  @Input() loading = false;
  @Input() error: string | null = null;
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() pageIndex = 0;
  @Input() searchPlaceholder = 'Search...';
  @Input() noDataMessage = 'No data found';
  @Input() noDataIcon = 'inbox';

  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() sortChange = new EventEmitter<Sort>();
  @Output() searchChange = new EventEmitter<string>();
  @Output() filterChange = new EventEmitter<Record<string, any>>();
  @Output() refresh = new EventEmitter<void>();
  @Output() export = new EventEmitter<void>();
  @Output() actionClick = new EventEmitter<{ action: string; item: T }>();
  @Output() bulkActionClick = new EventEmitter<{ action: string; items: T[] }>();
  @Output() selectionChange = new EventEmitter<T[]>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  selection = new SelectionModel<T>(true, []);

  searchControl = new FormControl('');
  private searchSubject = new Subject<string>();

  filterControls: { [key: string]: FormControl } = {};

  private destroy$ = new Subject<void>();

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.setupSearch();
    this.setupFilters();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearch(): void {
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(searchTerm => {
        this.searchChange.emit(searchTerm);
      });
  }

  private setupFilters(): void {
    if (this.config.filters) {
      this.config.filters.forEach(filter => {
        this.filterControls[filter.key] = new FormControl('');
        this.filterControls[filter.key].valueChanges
          .pipe(takeUntil(this.destroy$))
          .subscribe(value => {
            this.emitFilterChange();
          });
      });
    }
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchControl.value || '');
  }

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }

  onSortChange(sort: Sort): void {
    this.sortChange.emit(sort);
  }

  onRefresh(): void {
    this.refresh.emit();
  }

  onExport(): void {
    this.export.emit();
  }

  onActionClick(action: string, item: T): void {
    this.actionClick.emit({ action, item });
  }

  onBulkActionClick(action: string): void {
    if (this.selection.selected.length === 0) {
      this.showError('Please select items first');
      return;
    }
    this.bulkActionClick.emit({ action, items: this.selection.selected });
  }

  private emitFilterChange(): void {
    const filters: Record<string, any> = {};
    Object.keys(this.filterControls).forEach(key => {
      const value = this.filterControls[key].value;
      if (value !== null && value !== undefined && value !== '') {
        filters[key] = value;
      }
    });
    this.filterChange.emit(filters);
  }

  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.dataSource.data.forEach(row => this.selection.select(row));
    }
    this.emitSelectionChange();
  }

  onRowSelectionChange(row: T): void {
    this.selection.toggle(row);
    this.emitSelectionChange();
  }

  private emitSelectionChange(): void {
    this.selectionChange.emit(this.selection.selected);
  }

  clearFilters(): void {
    this.searchControl.setValue('');
    Object.keys(this.filterControls).forEach(key => {
      this.filterControls[key].setValue('');
    });
    this.emitFilterChange();
  }

  getColumnValue(item: T, column: TableColumn): any {
    const keys = column.key.split('.');
    let value = item;
    for (const key of keys) {
      value = (value as any)?.[key];
      if (value === undefined || value === null) break;
    }
    return value;
  }

  getDisplayValue(item: T, column: TableColumn): string {
    const value = this.getColumnValue(item, column);
    if (value === null || value === undefined) return '-';
    
    switch (column.type) {
      case 'date':
        return this.formatDate(value);
      case 'email':
        return value;
      case 'phone':
        return value;
      case 'status':
        return value ? 'Active' : 'Inactive';
      case 'roles':
        return Array.isArray(value) ? value.join(', ') : value;
      default:
        return String(value);
    }
  }

  getStatusClass(value: boolean): string {
    return value ? 'status-active' : 'status-inactive';
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'Never';
    return new Date(date).toLocaleDateString();
  }

  getAvatarText(item: T, column: TableColumn): string {
    const value = this.getColumnValue(item, column);
    if (typeof value === 'string') {
      return value.charAt(0).toUpperCase();
    }
    return '?';
  }

  getAvatarUrl(item: T, column: TableColumn): string | null {
    const value = this.getColumnValue(item, column);
    return value || null;
  }

  isActionVisible(action: TableAction, item: T): boolean {
    return !action.condition || action.condition(item);
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }

  get displayedColumns(): string[] {
    const columns = this.config.columns.map(col => col.key);
    if (this.config.enableSelection) {
      columns.unshift('select');
    }
    return columns;
  }

  get pageSizeOptions(): number[] {
    return this.config.pageSizeOptions || [5, 10, 25, 50, 100];
  }

  get defaultPageSize(): number {
    return this.config.defaultPageSize || 10;
  }
}
