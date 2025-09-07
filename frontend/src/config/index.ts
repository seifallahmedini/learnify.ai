// Environment configuration
const config = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5271/api',
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD
}

export default config
