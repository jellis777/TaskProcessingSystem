import { Link, Route, Routes } from 'react-router-dom';
import TaskDetailsPage from './pages/TaskDetailsPage';
import CreateTaskPage from './pages/CreateTaskPage';
import TaskListPage from './pages/TaskListPage';

export default function App() {
  return (
    <div
      style={{
        minHeight: '100vh',
        backgroundColor: '#f7f7f7',
        fontFamily: 'Arial, sans-serif',
      }}
    >
      <header
        style={{
          backgroundColor: '#ffffff',
          borderBottom: '1px solid #e5e5e5',
          padding: '1rem 2rem',
        }}
      >
        <div
          style={{
            maxWidth: '900px',
            margin: '0 auto',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            gap: '1rem',
          }}
        >
          <h1
            className="text-2xl font-bold"
            style={{ margin: 0, fontSize: '1.5rem' }}
          >
            Task Processing System
          </h1>
          <nav style={{ display: 'flex', gap: '1rem' }}>
            <Link to="/">Tasks</Link>
            <Link to="/tasks/new">Create Task</Link>
          </nav>
        </div>
      </header>

      <main
        style={{
          maxWidth: '900px',
          margin: '0 auto',
          padding: '2rem',
        }}
      >
        <Routes>
          <Route path="/" element={<TaskListPage />} />
          <Route path="/tasks/new" element={<CreateTaskPage />} />
          <Route path="/tasks/:id" element={<TaskDetailsPage />} />
        </Routes>
      </main>
    </div>
  );
}
