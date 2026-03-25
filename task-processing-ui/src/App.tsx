import { Link, Route, Routes } from 'react-router-dom';
import TaskDetailsPage from './pages/TaskDetailsPage';
import CreateTaskPage from './pages/CreateTaskPage';
import TaskListPage from './pages/TaskListPage';

export default function App() {
  return (
    <div className="min-h-screen bg-slate-50 font-sans text-slate-900">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between gap-4 px-6 py-4">
          <h1 className="text-2xl font-bold">Task Processing System</h1>

          <nav className="flex items-center gap-4 text-sm font-medium text-slate-700">
            <Link className="transition hover:text-blue-600" to="/">
              Tasks
            </Link>
            <Link className="transition hover:text-blue-600" to="/tasks/new">
              Create Task
            </Link>
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-6 py-8">
        <Routes>
          <Route path="/" element={<TaskListPage />} />
          <Route path="/tasks/new" element={<CreateTaskPage />} />
          <Route path="/tasks/:id" element={<TaskDetailsPage />} />
        </Routes>
      </main>
    </div>
  );
}
