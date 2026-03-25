# Task Processing System

A full-stack task processing platform built with ASP.NET Core, SQL Server, a .NET worker service, React, TypeScript, and Tailwind CSS.

## Features

- Create background processing tasks
- View task list and task details
- Track task lifecycle: Queued, Processing, Completed, Failed
- Automatic retry handling in the worker
- Manual retry from the UI
- Persisted processing logs
- Backend unit and integration tests
- Frontend component/page tests

## Tech Stack

### Backend

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- xUnit
- WebApplicationFactory integration testing

### Background Processing

- .NET Worker Service
- Database-backed polling
- Automatic retry logic
- Processing logs

### Frontend

- React
- TypeScript
- Vite
- Tailwind CSS
- Vitest
- React Testing Library

## Architecture

- **API** handles task creation, retrieval, validation, and manual retry
- **Worker** polls queued tasks and processes them asynchronously
- **Database** stores tasks and task processing logs
- **Frontend** provides a dashboard for creating, viewing, and retrying tasks

## Running Locally

### Backend API

```bash
cd TaskProcessing.Api
dotnet run
```

### Worker

```bash
cd TaskProcessing.Worker
dotnet run
```

### Frontend

```bash
cd task-processing-ui
npm install
npm run dev
```

## Testing

### Backend tests

```bash
dotnet test
```

### Frontend tests

```bash
cd task-processing-ui
npm test
```

## Future Improvements

- Azure deployment
- Delayed retry/backoff strategy
- Authentication and authorization
- Better dashboard filtering/search
- Dockerized local orchestration
