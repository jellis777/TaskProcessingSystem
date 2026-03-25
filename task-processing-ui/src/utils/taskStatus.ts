import type { TaskStatus } from '../types/task';

export function isActiveTaskStatus(status: TaskStatus) {
  return status === 'Queued' || status === 'Processing';
}
