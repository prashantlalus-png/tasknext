import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { TaskItem } from '../../models/task.models';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

type FilterMode = 'all' | 'pending' | 'done';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  tasks: TaskItem[] = [];
  loading = false;
  error = '';

  // create form
  title = '';
  description = '';

  filter: FilterMode = 'all';

  constructor(
    private tasksApi: TaskService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  get userName(): string {
    return this.auth.getUser()?.name ?? 'User';
  }

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

  loadTasks(): void {
    this.loading = true;
    this.error = '';

    this.tasksApi.getMyTasks().subscribe({
      next: (data) => {
        this.tasks = data ?? [];
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error ?? 'Failed to load tasks (are you logged in?)';
      }
    });
  }

  createTask(): void {
    if (!this.title.trim()) return;

    this.tasksApi.createTask({
      title: this.title.trim(),
      description: this.description?.trim() || null
    }).subscribe({
      next: (created) => {
        this.tasks = [created, ...this.tasks];
        this.title = '';
        this.description = '';
      },
      error: (err) => {
        this.error = err?.error ?? 'Failed to create task';
      }
    });
  }

  toggleDone(task: TaskItem): void {
    this.tasksApi.updateTask(task.id, {
      title: task.title,
      description: task.description ?? null,
      isDone: !task.isDone
    }).subscribe({
      next: (updated) => {
        this.tasks = this.tasks.map(t => t.id === updated.id ? updated : t);
      },
      error: (err) => {
        this.error = err?.error ?? 'Failed to update task';
      }
    });
  }

  deleteTask(task: TaskItem): void {
    this.tasksApi.deleteTask(task.id).subscribe({
      next: () => {
        this.tasks = this.tasks.filter(t => t.id !== task.id);
      },
      error: (err) => {
        this.error = err?.error ?? 'Failed to delete task';
      }
    });
  }

  get filteredTasks(): TaskItem[] {
    if (this.filter === 'pending') return this.tasks.filter(t => !t.isDone);
    if (this.filter === 'done') return this.tasks.filter(t => t.isDone);
    return this.tasks;
  }
}
