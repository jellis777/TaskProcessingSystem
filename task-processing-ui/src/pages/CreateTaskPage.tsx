import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { createTask } from '../api/tasks';

const TASK_TYPE_OPTIONS = [
  'report-generation',
  'text-summary',
  'file-processing',
];

export default function CreateTaskPage() {
  const navigate = useNavigate();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [type, setType] = useState('report-generation');
  const [payloadJson, setPayloadJson] = useState(`{"sample":"value"}`);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    try {
      setIsSubmitting(true);
      setError(null);

      const createdTask = await createTask({
        title,
        description,
        type,
        payloadJson,
      });

      navigate(`/tasks/${createdTask.id}`);
    } catch (err) {
      console.error(err);

      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Failed to create task.');
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <div className="mb-8">
        <h2 className="mb-2 text-3xl font-bold text-slate-900">Create Task</h2>
        <p className="text-sm text-slate-600">
          Submit a new background processing task for the worker to handle.
        </p>
      </div>

      <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label
              htmlFor="title"
              className="mb-2 block text-sm font-medium text-slate-800"
            >
              {' '}
              Title
            </label>
            <input
              id="title"
              type="text"
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              placeholder="Generate monthly report"
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
            />
            <p className="mt-2 text-xs text-slate-500">
              Keep it short and descriptive
            </p>
          </div>

          <div>
            <label
              htmlFor="description"
              className="mb-2 block text-sm font-medium text-slate-800"
            >
              Description
            </label>
            <textarea
              id="description"
              value={description}
              onChange={(event) => setDescription(event.target.value)}
              placeholder="Create the reporting summary for March."
              rows={4}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
            />
          </div>

          <div>
            <label
              htmlFor="type"
              className="mb-2 block text-sm font-medium text-slate-800"
            >
              Task Type
            </label>
            <select
              id="type"
              value={type}
              onChange={(event) => setType(event.target.value)}
              className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
            >
              {TASK_TYPE_OPTIONS.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label
              htmlFor="payloadJson"
              className="mb-2 block text-sm font-medium text-slate-800"
            >
              Payload JSON
            </label>
            <textarea
              id="payloadJson"
              value={payloadJson}
              onChange={(event) => setPayloadJson(event.target.value)}
              rows={8}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 font-mono text-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
            />
            <p className="mt-2 text-xs text-slate-500">
              Must be valid JSON based on your backend validation rules.
            </p>
          </div>

          {error && (
            <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
              {error}
            </div>
          )}

          <div className="flex items-center gap-3">
            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
            >
              {isSubmitting ? 'Creating...' : 'Create Task'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
