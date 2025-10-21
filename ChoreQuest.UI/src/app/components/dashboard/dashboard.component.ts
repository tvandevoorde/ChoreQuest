import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ChoreListService, ChoreList, CreateChoreListDto } from '../../services/chore-list.service';
import { NotificationService, Notification } from '../../services/notification.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  choreLists: ChoreList[] = [];
  notifications: Notification[] = [];
  showCreateForm = false;
  newChoreList: CreateChoreListDto = { name: '', description: '' };
  currentUser: any;

  constructor(
    private authService: AuthService,
    private choreListService: ChoreListService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadChoreLists();
    this.loadNotifications();
  }

  loadChoreLists(): void {
    this.choreListService.getChoreLists(this.currentUser.id).subscribe(
      lists => this.choreLists = lists
    );
  }

  loadNotifications(): void {
    this.notificationService.getNotifications(this.currentUser.id).subscribe(
      notifications => this.notifications = notifications.filter(n => !n.isRead).slice(0, 5)
    );
  }

  createChoreList(): void {
    if (!this.newChoreList.name.trim()) {
      return;
    }

    this.choreListService.createChoreList(this.currentUser.id, this.newChoreList).subscribe({
      next: (list) => {
        this.choreLists.push(list);
        this.newChoreList = { name: '', description: '' };
        this.showCreateForm = false;
      }
    });
  }

  deleteChoreList(id: number): void {
    if (confirm('Are you sure you want to delete this chore list?')) {
      this.choreListService.deleteChoreList(id).subscribe({
        next: () => {
          this.choreLists = this.choreLists.filter(cl => cl.id !== id);
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getProgressPercentage(list: ChoreList): number {
    return list.choreCount > 0 ? (list.completedChoreCount / list.choreCount) * 100 : 0;
  }
}
