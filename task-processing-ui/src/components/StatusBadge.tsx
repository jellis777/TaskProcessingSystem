import type { TaskStatus } from '../types/task';

type StatusBadgeProps = {
  status: TaskStatus;
};

function getStatusClasses(status: TaskStatus) {
  switch (status) {
    case 'Queued':
      return 'bg-amber-100 text-amber-800';
    case 'Processing':
      return 'bg-blue-100 text-blue-800';
    case 'Completed':
      return 'bg-emerald-100 text-emerald-800';
    case 'Failed':
      return 'bg-red-100 text-red-800';
    default:
      return 'bg-slate-200 text-slate-700';
  }
}

export default function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <span
      className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${getStatusClasses(
        status,
      )}`}
    >
      {status}
    </span>
  );
}
