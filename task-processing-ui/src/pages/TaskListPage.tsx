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
    return <p className="text-sm text-slate-600">Loading tasks...</p>;
  }

  if (error) {
    return (
      <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
        {error}
      </div>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <h2 className="mb-2 text-2xl font-bold text-slate-900">Tasks</h2>
        <p className="mb-4 text-sm text-slate-600">No tasks found yet.</p>
        <Link
          to="/tasks/new"
          className="inline-flex rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-700"
        >
          Create your first task
        </Link>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="mb-2 text-3xl font-bold text-slate-900">Tasks</h2>
        <p className="text-sm text-slate-600">
          View and monitor background processing tasks.
        </p>
      </div>

      <div className="grid gap-4">
        {tasks.map((task) => (
          <Link
            key={task.id}
            to={`/tasks/${task.id}`}
            className="block rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-blue-300 hover:shadow-md"
          >
            <div className="mb-3 flex items-center justify-between gap-4">
              <h3 className="text-lg font-semibold text-slate-900">
                {task.title}
              </h3>
              <StatusBadge status={task.status} />
            </div>

            <div className="grid gap-1 text-sm text-slate-600">
              <div>
                <span className="font-medium text-slate-800">Type:</span>{' '}
                {task.type}
              </div>
              <div>
                <span className="font-medium text-slate-800">Created:</span>{' '}
                <FormattedDate value={task.createdAt} />
              </div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
