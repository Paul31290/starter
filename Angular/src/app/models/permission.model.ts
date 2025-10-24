/**
 * Permission model representing a system permission
 */
export interface Permission {
  id?: number;
  name: string;
  description?: string;
  resource?: string;
  action?: string;
  createdAt?: Date;
}

/**
 * Role-Permission relationship model
 */
export interface RolePermission {
  id?: number;
  roleId: number;
  roleName?: string;
  permissionId: number;
  permission?: Permission;
  createdAt?: Date;
}

/**
 * Permission check result
 */
export interface PermissionCheckResult {
  hasPermission: boolean;
  reason?: string;
}

