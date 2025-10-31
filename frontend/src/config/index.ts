// Environment configuration
const config = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api',
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD
}

export default config
