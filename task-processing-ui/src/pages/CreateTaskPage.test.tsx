import { MemoryRouter } from 'react-router-dom';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import CreateTaskPage from './CreateTaskPage';
import * as tasksApi from '../api/tasks';

const mockNavigate = vi.fn();

vi.mock('../api/tasks');
vi.mock('react-router-dom', async () => {
  const actual =
    await vi.importActual<typeof import('react-router-dom')>(
      'react-router-dom',
    );
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

const mockedCreateTask = vi.mocked(tasksApi.createTask);

describe('CreateTaskPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('submits the form and navigates to the new task', async () => {
    const user = userEvent.setup();

    mockedCreateTask.mockResolvedValue({
      id: 42,
      title: 'Generate report',
      description: 'Create a monthly report',
      type: 'report-generation',
      status: 'Queued',
      payloadJson: '{"month":"March"}',
      resultJson: null,
      errorMessage: null,
      retryCount: 0,
      maxRetries: 3,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      startedAt: null,
      completedAt: null,
      processingLogs: [],
    });

    render(
      <MemoryRouter>
        <CreateTaskPage />
      </MemoryRouter>,
    );

    await user.type(screen.getByLabelText('Title'), 'Generate report');
    await user.type(
      screen.getByLabelText('Description'),
      'Create a monthly report',
    );
    await user.selectOptions(
      screen.getByLabelText('Task Type'),
      'report-generation',
    );
    const payloadInput = screen.getByLabelText('Payload JSON');
    await user.clear(payloadInput);
    await user.click(payloadInput);
    await user.paste('{"month":"March"}');

    await user.click(screen.getByRole('button', { name: 'Create Task' }));

    await waitFor(() => {
      expect(mockedCreateTask).toHaveBeenCalledWith({
        title: 'Generate report',
        description: 'Create a monthly report',
        type: 'report-generation',
        payloadJson: '{"month":"March"}',
      });
    });

    expect(mockNavigate).toHaveBeenCalledWith('/tasks/42');
  });

  it('shows an error message when task creation fails', async () => {
    const user = userEvent.setup();

    mockedCreateTask.mockRejectedValue(
      new Error('PayloadJson must be valid JSON.'),
    );

    render(
      <MemoryRouter>
        <CreateTaskPage />
      </MemoryRouter>,
    );

    await user.type(screen.getByLabelText('Title'), 'Bad task');
    await user.click(screen.getByRole('button', { name: 'Create Task' }));

    await waitFor(() => {
      expect(
        screen.getByText('PayloadJson must be valid JSON.'),
      ).toBeInTheDocument();
    });
  });
});
