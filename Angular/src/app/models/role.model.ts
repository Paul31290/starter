export interface Role {
  id: number;
  name: string;
  description?: string;
}

export interface AssignRolesRequest {
  userId: number;
  roleIds: number[];
}

