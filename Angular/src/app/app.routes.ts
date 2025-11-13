import { Routes } from "@angular/router";
import { HomeComponent } from "./home/home.component";
import { AuthGuard } from "./guards/auth.guard";
import { RoleGuard } from "./guards/role.guard";

export const routes: Routes = [
  {
    path: "",
    redirectTo: "/home",
    pathMatch: "full",
  },
  {
    path: "home",
    component: HomeComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "users",
    loadComponent: () =>
      import("./components/user-table/user-table.component").then((c) => c.UserTableComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Administrator', 'Manager'] }
  },
  {
    path: "auth",
    children: [
      {
        path: "login",
        loadComponent: () =>
          import("./components/auth/login/login.component").then((c) => c.LoginComponent),
      },
      {
        path: "register",
        loadComponent: () =>
          import("./components/auth/register/register.component").then((c) => c.RegisterComponent),
      },
      {
        path: "forgot-password",
        loadComponent: () =>
          import("./components/auth/forgot-password/forgot-password.component").then((c) => c.ForgotPasswordComponent),
      },
      {
        path: "reset-password",
        loadComponent: () =>
          import("./components/auth/reset-password/reset-password.component").then((c) => c.ResetPasswordComponent),
      }
    ]
  },
  {
    path: "unauthorized",
    loadComponent: () =>
      import("./components/unauthorized/unauthorized.component").then((c) => c.UnauthorizedComponent),
  },
  {
    path: "**",
    loadComponent: () =>
      import("./not-found.component").then((c) => c.NotFoundComponent),
  },
];
