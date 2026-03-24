export type TaskStatus = 'Queued' | 'Processing' | 'Completed' | 'Failed';

export interface TaskProcessingLog {
  id: number;
  message: string;
  level: string;
  createdAt: string;
}

export interface TaskSummary {
  id: number;
  title: string;
  type: string;
  status: TaskStatus;
  createdAt: string;
}

export interface TaskDetails {
  id: number;
  title: string;
  description: string;
  type: string;
  status: TaskStatus;
  payloadJson: string | null;
  resultJson: string | null;
  errorMessage: string | null;
  retryCount: number;
  maxRetries: number;
  createdAt: string;
  updatedAt: string;
  startedAt: string | null;
  completedAt: string | null;
  processingLogs: TaskProcessingLog[];
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  type: string;
  payloadJson: string;
}
