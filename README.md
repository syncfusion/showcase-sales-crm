# Sales CRM Pipeline Intelligence

Sales CRM Pipeline Intelligence is a cross-framework enterprise showcase application built with a shared ASP.NET Core Web API and three Syncfusion-powered clients: Angular, React, and Blazor.

The application demonstrates how a sales organization can bring account intelligence, opportunity execution, forecasting, activities, documents, territories, and AI-assisted guidance into one responsive workspace. It also shows how the same enterprise experience can be delivered consistently across different frontend stacks by using Syncfusion UI components and Syncfusion Code Studio.

> This is a showcase application with deterministic sample data. It is not intended to be used as a production CRM without adding production authentication, authorization, auditing, secrets management, and operational controls.

## What the showcase includes

- Executive dashboard with revenue, pipeline, weighted forecast, win rate, charts, and recent opportunities
- Account and contact management with filtering, grouping, paging, export, and account details
- Opportunity tracking and a drag-and-drop pipeline Kanban with persisted stage changes
- Forecast trend with solid actuals and dotted future forecast months
- Activities calendar for meetings, follow-ups, and tasks
- Territory performance map and summaries
- Sales document workspace and templates
- AI Sales Assistant experience for deal risk, recommended actions, and sales prompts
- Responsive layouts, dark mode, loading states, error handling, and accessible navigation
- Swagger/OpenAPI documentation and deterministic PostgreSQL seed data

## Technology

| Layer | Technology |
| --- | --- |
| API | ASP.NET Core Web API on .NET 10 |
| Data | Entity Framework Core and PostgreSQL |
| Angular client | Angular 21, TypeScript, RxJS, Syncfusion Angular UI |
| React client | React 19, TypeScript, Vite, Syncfusion React UI |
| Blazor client | Blazor on .NET 10, Syncfusion Blazor UI |
| Testing | .NET test projects and Angular Vitest tests |

The UI implementations use Syncfusion components such as DataGrid, Charts, Kanban, Scheduler, Maps, Document Editor, and AI AssistView.

## Why Syncfusion Code Studio and UI components?

This repository is a practical proof of how [Syncfusion Code Studio](https://www.syncfusion.com/code-studio/) and the [Syncfusion component ecosystem](https://www.syncfusion.com/) can accelerate component-rich enterprise development.

- Code Studio can help teams plan features, generate and refine code, debug issues, and create tests with awareness of the existing codebase.
- Production-oriented UI components reduce the amount of custom code required for advanced grids, charts, scheduling, Kanban workflows, maps, document editing, and AI experiences.
- Similar component concepts across Angular, React, and Blazor make it easier to preserve business behavior while choosing the framework that best fits each team.
- Built-in capabilities such as filtering, grouping, export, responsive rendering, accessibility, and theming help teams focus on business workflows rather than foundational UI infrastructure.

## Repository structure

```text
sales-crm-github/
├── webapi/
│   ├── src/                 # API, domain, application, EF Core, and seed data
│   └── tests/               # Unit, integration, and API tests
├── angular/                 # Angular client
├── react/                   # React client
├── blazor/                  # Blazor client  
```

## Run locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A current Node.js LTS release and npm
- PostgreSQL
- A valid Syncfusion license or trial where required

Create a PostgreSQL database named `salescrm_pipeline`, then update the connection string in:

```text
apps/sales-crm-pipeline-intelligence/backend/src/SalesCrm.Api/appsettings.Development.json
```

Trust the local ASP.NET Core HTTPS certificate if needed:

```bash
dotnet dev-certs https --trust
```

### 1. Start the Web API

```bash
cd apps/sales-crm-pipeline-intelligence/backend
dotnet restore
dotnet run --project src/SalesCrm.Api/SalesCrm.Api.csproj
```

The API runs at `https://localhost:59527`, with Swagger UI at `https://localhost:59527/swagger`.

The development API applies EF Core migrations and refreshes the deterministic showcase data when it starts.

### 2. Start a frontend

Angular:

```bash
cd apps/sales-crm-pipeline-intelligence/frontend/apps/angular
npm install
npm start
```

Open `http://localhost:4200`.

React:

```bash
cd apps/sales-crm-pipeline-intelligence/frontend/apps/react
npm install
npm run dev
```

The React client reads `VITE_API_BASE_URL` from `.env.development` and opens at `http://localhost:5173`.

Blazor:

```bash
cd apps/sales-crm-pipeline-intelligence/frontend/apps/blazor
npm install
dotnet run
```

Open `http://localhost:5088`.

## Build and test

```bash
# Backend
cd apps/sales-crm-pipeline-intelligence/backend
dotnet test SalesCrmPipeline.sln

# Angular
cd ../frontend/apps/angular
npm test
npm run build

# React
cd ../react
npm run build

# Blazor
cd ../blazor
dotnet build
```

## Licensing

Syncfusion packages are governed by Syncfusion's licensing terms. Review the [Syncfusion licensing documentation](https://www.syncfusion.com/sales/licensing) before redistributing or deploying the applications. Publishing this source repository does not grant a license to Syncfusion products.

## Intended audience

This showcase is useful for engineering leaders, architects, product teams, and developers evaluating how a modern sales CRM can be implemented with a shared API and multiple enterprise web frameworks.
