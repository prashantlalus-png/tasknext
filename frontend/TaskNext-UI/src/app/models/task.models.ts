export interface TaskItem {
  id: string;
  title: string;
  description?: string | null;
  isDone: boolean;
  createdAt: string;
  userId: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string | null;
  isDone: boolean;
}