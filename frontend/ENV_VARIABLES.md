# Environment Variables Configuration

## Frontend Environment Variables

### Setup

1. Copy `.env.example` to `.env`:
   ```bash
   cp .env.example .env
   ```

2. Update the values in `.env` to match your local setup.

### Available Environment Variables

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `VITE_API_BASE_URL` | Base URL for the API server | `http://localhost:5271/api` |
| `VITE_FUNCTIONS_BASE_URL` | Base URL for Azure Functions (separate host) | `http://localhost:7071/api` |
| `VITE_AZURE_FUNCTION_CODE` | Azure Function key for protected endpoints (optional) |  |
| `NODE_ENV` | Environment mode | `development` |

### Environment Files

- `.env` - Local development environment (not committed to git)
- `.env.example` - Template file with example values (committed to git)
- `.env.production` - Production environment template

### Usage in Code

```typescript
import config from '@/config'

// Access the API base URL
const apiUrl = config.apiBaseUrl

// Check environment
if (config.isDevelopment) {
  console.log('Running in development mode')
}
```

### Scripts

- `npm run dev` - Start development server with `.env` variables
- `npm run build` - Build for production using current environment
- `npm run build:prod` - Build specifically for production environment
- `npm run preview` - Preview the built application

### Important Notes

- Environment variables must be prefixed with `VITE_` to be accessible in the frontend
- The `.env` file is ignored by git to keep sensitive information secure
- Always update `.env.example` when adding new environment variables
- For production deployments, set environment variables in your hosting platform
