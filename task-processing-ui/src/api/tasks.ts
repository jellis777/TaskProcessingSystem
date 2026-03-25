import type {
  CreateTaskRequest,
  TaskDetails,
  TaskSummary,
} from '../types/task';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function getTasks(): Promise<TaskSummary[]> {
  const response = await fetch(API_BASE_URL);

  if (!response.ok) {
    throw new Error('Failed to fetch tasks.');
  }

  return response.json();
}

export async function getTaskById(id: number): Promise<TaskDetails> {
  const response = await fetch(`${API_BASE_URL}/${id}`);

  if (!response.ok) {
    throw new Error('Failed to fetch task details');
  }

  return response.json();
}

export async function createTask(
  request: CreateTaskRequest,
): Promise<TaskDetails> {
  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const errorBody = await response.json().catch(() => null);
    const message =
      errorBody?.error || errorBody?.title || 'Failed to create task.';

    throw new Error(message);
  }

  return response.json();
}

export async function retryTask(id: number): Promise<TaskDetails> {
  const response = await fetch(`${API_BASE_URL}/${id}/retry`, {
    method: 'POST',
  });

  if (!response.ok) {
    const errorBody = await response.json().catch(() => null);

    const message =
      errorBody?.error || errorBody?.title || 'Failed to retry task.';

    throw new Error(message);
  }

  return response.json();
}
