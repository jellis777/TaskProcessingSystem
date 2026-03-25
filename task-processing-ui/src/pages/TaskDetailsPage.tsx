import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import type { TaskDetails } from '../types/task';
import { getTaskById, retryTask } from '../api/tasks';
import FormattedDate from '../components/FormattedDate';
import StatusBadge from '../components/StatusBadge';

export default function TaskDetailsPage() {
  const { id } = useParams();
  const taskId = Number(id);

  const [task, setTask] = useState<TaskDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isRetrying, setIsRetrying] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [retryError, setRetryError] = useState<string | null>(null);

  useEffect(() => {
    async function loadTask() {
      try {
        setIsLoading(true);
        setLoadError(null);

        const data = await getTaskById(taskId);
        setTask(data);
      } catch (err) {
        console.error(err);
        setLoadError('Failed to load task details.');
      } finally {
        setIsLoading(false);
      }
    }

    if (!Number.isNaN(taskId)) {
      loadTask();
    } else {
      setLoadError('Invalid task id.');
      setIsLoading(false);
    }
  }, [taskId]);

  async function handleRetry() {
    if (!task) {
      return;
    }

    try {
      setIsRetrying(true);
      setRetryError(null);

      const updatedTask = await retryTask(task.id);
      setTask(updatedTask);
    } catch (err) {
      console.error(err);

      if (err instanceof Error) {
        setRetryError(err.message);
      } else {
        setRetryError('Failed to retry task.');
      }
    } finally {
      setIsRetrying(false);
    }
  }

  if (isLoading) {
    return <p className="text-sm text-slate-600">Loading task details...</p>;
  }

  if (loadError) {
    return (
      <div className="space-y-4">
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {loadError}
        </div>
        <Link
          to="/"
          className="inline-flex rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-700"
        >
          Back to Tasks
        </Link>
      </div>
    );
  }

  if (!task) {
    return (
      <div className="space-y-4">
        <p className="text-sm text-slate-600">Task not found.</p>
        <Link
          to="/"
          className="inline-flex rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-700"
        >
          Back to Tasks
        </Link>
      </div>
    );
  }

  const canRetry = task.status === 'Failed';

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <Link
            to="/"
            className="mb-3 inline-block text-sm font-medium text-blue-600 transition hover:text-blue-700"
          >
            ← Back to Tasks
          </Link>

          <h2 className="mb-2 text-3xl font-bold text-slate-900">
            {task.title}
          </h2>

          <div className="flex items-center gap-3">
            <StatusBadge status={task.status} />
            <span className="text-sm text-slate-500">{task.type}</span>
          </div>
        </div>

        {canRetry && (
          <button
            onClick={handleRetry}
            disabled={isRetrying}
            className="rounded-lg bg-amber-500 px-4 py-2 text-sm font-medium text-white transition hover:bg-amber-600 disabled:cursor-not-allowed disabled:bg-amber-300"
          >
            {isRetrying ? 'Retrying...' : 'Retry Task'}
          </button>
        )}
      </div>

      {retryError && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {retryError}
        </div>
      )}

      <div className="grid gap-6 lg:grid-cols-2">
        <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <h3 className="mb-4 text-lg font-semibold text-slate-900">
            Task Overview
          </h3>

          <div className="grid gap-3 text-sm text-slate-700">
            <div>
              <span className="font-medium text-slate-900">Description:</span>{' '}
              {task.description || '—'}
            </div>
            <div>
              <span className="font-medium text-slate-900">Type:</span>{' '}
              {task.type}
            </div>
            <div>
              <span className="font-medium text-slate-900">Retry Count:</span>{' '}
              {task.retryCount}
            </div>
            <div>
              <span className="font-medium text-slate-900">Max Retries:</span>{' '}
              {task.maxRetries}
            </div>
            <div>
              <span className="font-medium text-slate-900">Created:</span>{' '}
              <FormattedDate value={task.createdAt} />
            </div>
            <div>
              <span className="font-medium text-slate-900">Updated:</span>{' '}
              <FormattedDate value={task.updatedAt} />
            </div>
            <div>
              <span className="font-medium text-slate-900">Started:</span>{' '}
              {task.startedAt ? <FormattedDate value={task.startedAt} /> : '—'}
            </div>
            <div>
              <span className="font-medium text-slate-900">Completed:</span>{' '}
              {task.completedAt ? (
                <FormattedDate value={task.completedAt} />
              ) : (
                '—'
              )}
            </div>
          </div>
        </section>

        <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <h3 className="mb-4 text-lg font-semibold text-slate-900">
            Processing Outcome
          </h3>

          <div className="space-y-4 text-sm text-slate-700">
            <div>
              <div className="mb-2 font-medium text-slate-900">
                Payload JSON
              </div>
              <pre className="overflow-x-auto rounded-lg bg-slate-100 p-3 text-xs text-slate-800">
                {task.payloadJson || '—'}
              </pre>
            </div>

            <div>
              <div className="mb-2 font-medium text-slate-900">Result JSON</div>
              <pre className="overflow-x-auto rounded-lg bg-slate-100 p-3 text-xs text-slate-800">
                {task.resultJson || '—'}
              </pre>
            </div>

            <div>
              <div className="mb-2 font-medium text-slate-900">
                Error Message
              </div>
              <div className="rounded-lg bg-slate-100 p-3 text-xs text-slate-800">
                {task.errorMessage || '—'}
              </div>
            </div>
          </div>
        </section>
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <h3 className="mb-4 text-lg font-semibold text-slate-900">
          Processing Logs
        </h3>

        {task.processingLogs.length === 0 ? (
          <p className="text-sm text-slate-600">
            No logs available for this task yet.
          </p>
        ) : (
          <div className="space-y-3">
            {task.processingLogs.map((log) => (
              <div
                key={log.id}
                className="rounded-xl border border-slate-200 bg-slate-50 p-4"
              >
                <div className="mb-2 flex flex-wrap items-center justify-between gap-3">
                  <span
                    className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${
                      log.level === 'Error'
                        ? 'bg-red-100 text-red-700'
                        : 'bg-slate-200 text-slate-700'
                    }`}
                  >
                    {log.level}
                  </span>

                  <span className="text-xs text-slate-500">
                    <FormattedDate value={log.createdAt} />
                  </span>
                </div>

                <p className="text-sm text-slate-700">{log.message}</p>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
