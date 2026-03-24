import { Link, Route, Routes } from 'react-router-dom';
import TaskDetailsPage from './pages/TaskDetailsPage';
import CreateTaskPage from './pages/CreateTaskPage';
import TaskListPage from './pages/TaskListPage';

function App() {
  return (
    <div style={{ padding: '2rem', fontFamily: 'Arial, sans-serif' }}>
      <header style={{ marginBottom: '2rem' }}>
        <h1>Task Processing System</h1>
        <nav style={{ display: 'flex', gap: '1rem' }}>
          <Link to="/">Tasks</Link>
          <Link to="/tasks/new">Create Task</Link>
        </nav>
      </header>
      <Routes>
        <Route path="/" element={<TaskListPage />} />
        <Route path="/tasks/new" element={<CreateTaskPage />} />
        <Route path="/tasks/:id" element={<TaskDetailsPage />} />
      </Routes>
    </div>
  );
}

export default App;
