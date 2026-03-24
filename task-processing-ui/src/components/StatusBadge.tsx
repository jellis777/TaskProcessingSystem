import type { TaskStatus } from '../types/task';

type StatusBadgeProps = {
  status: TaskStatus;
};

const getStatusStyles = (status: TaskStatus) => {
  switch (status) {
    case 'Queued':
      return {
        backgroundColor: '#fffcd',
        color: '#856404',
      };
    case 'Processing':
      return {
        backgroundColor: '#cce5ff',
        color: '#004085',
      };
    case 'Completed':
      return {
        backgroundColor: '#d4edda',
        color: '#155724',
      };
    case 'Failed':
      return {
        backgroundColor: '#f8d7da',
        color: '#721c24',
      };
    default:
      return {
        backgroundColor: '#e2e3e5',
        color: '#383d41',
      };
  }
};

export default function StatusBadge({ status }: StatusBadgeProps) {
  const styles = getStatusStyles(status);

  return (
    <span
      style={{
        display: 'inline-block',
        padding: '0.35rem 0.75rem',
        borderRadius: '999px',
        fontSize: '0.875rem',
        fontWeight: 600,
        ...styles,
      }}
    >
      {status}
    </span>
  );
}
