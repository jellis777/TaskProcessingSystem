import { useEffect, useState } from 'react';
import type { TaskSummary } from '../types/task';
import { getTasks } from '../api/tasks';
import { Link } from 'react-router-dom';
import StatusBadge from '../components/StatusBadge';
import FormattedDate from '../components/FormattedDate';

export default function TaskListPage() {
  const [tasks, setTasks] = useState<TaskSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadTasks() {
      try {
        setIsLoading(true);
        setError(null);

        const data = await getTasks();
        setTasks(data);
      } catch (err) {
        console.error(err);
        setError('Failed to load tasks.');
      } finally {
        setIsLoading(false);
      }
    }
    loadTasks();
  }, []);

  if (isLoading) {
    return <p>Loading tasks...</p>;
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (tasks.length === 0) {
    return (
      <div>
        <h2>Tasks</h2>
        <p>No tasks found yet.</p>
        <Link to="/tasks/new">Create your first task</Link>
      </div>
    );
  }

  return (
    <div>
      <div style={{ marginBottom: '1.5rem' }}>
        <h2 style={{ marginBottom: '0.5rem' }}>Tasks</h2>
        <p>View and monitor background processing tasks.</p>
      </div>

      <div style={{ display: 'grid', gap: '1rem' }}>
        {tasks.map((task) => (
          <Link
            key={task.id}
            to={`/tasks/${task.id}`}
            style={{
              display: 'block',
              backgroundColor: '#ffffff',
              borderRadius: '12px',
              padding: '1rem',
              border: '1px solid #e5e5e5',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.08)',
            }}
          >
            <div
              style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                gap: '1rem',
                marginBottom: '0.75rem',
              }}
            >
              <h3 style={{ margin: 0 }}>{task.title}</h3>
              <StatusBadge status={task.status} />
            </div>

            <div style={{ display: 'grid', gap: '0.35rem', color: '#555' }}>
              <div>
                <strong>Type:</strong> {task.type}
              </div>
              <div>
                <strong>Created:</strong>{' '}
                <FormattedDate value={task.createdAt} />
              </div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
