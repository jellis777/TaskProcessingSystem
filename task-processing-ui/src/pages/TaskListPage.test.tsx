import { MemoryRouter } from 'react-router-dom';
import { render, screen, waitFor } from '@testing-library/react';
// import { beforeEach, describe, expect, it, vi } from 'vitest';
import TaskListPage from './TaskListPage';
import * as tasksApi from '../api/tasks';

vi.mock('../api/tasks');

const mockedGetTasks = vi.mocked(tasksApi.getTasks);

describe('TaskListPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('shows loading state initially', () => {
    mockedGetTasks.mockReturnValue(new Promise(() => {}));

    render(
      <MemoryRouter>
        <TaskListPage />
      </MemoryRouter>,
    );

    expect(screen.getByText('Loading tasks...')).toBeInTheDocument();
  });

  it('renders tasks returned from the API', async () => {
    mockedGetTasks.mockResolvedValue([
      {
        id: 1,
        title: 'Generate report',
        type: 'report-generation',
        status: 'Queued',
        createdAt: new Date().toISOString(),
      },
      {
        id: 2,
        title: 'Summarize text',
        type: 'text-summary',
        status: 'Completed',
        createdAt: new Date().toISOString(),
      },
    ]);

    render(
      <MemoryRouter>
        <TaskListPage />
      </MemoryRouter>,
    );

    expect(await screen.findByText('Generate report')).toBeInTheDocument();
    expect(screen.getByText('Summarize text')).toBeInTheDocument();
    expect(screen.getByText('Queued')).toBeInTheDocument();
    expect(screen.getByText('Completed')).toBeInTheDocument();
  });

  it('shows an error message when loading fails', async () => {
    mockedGetTasks.mockRejectedValue(new Error('API failure'));

    render(
      <MemoryRouter>
        <TaskListPage />
      </MemoryRouter>,
    );

    await waitFor(() => {
      expect(screen.getByText('Failed to load tasks.')).toBeInTheDocument();
    });
  });
});
