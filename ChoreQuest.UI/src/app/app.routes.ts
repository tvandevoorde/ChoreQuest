import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ChoreListDetailComponent } from './components/chore-list-detail/chore-list-detail.component';
import { NotificationsComponent } from './components/notifications/notifications.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'chore-lists/:id', component: ChoreListDetailComponent },
  { path: 'notifications', component: NotificationsComponent }
];
