import config from '../config'

/**
 * Utility functions for API operations
 */
export class ApiClient {
  private static baseUrl = config.apiBaseUrl

  /**
   * Constructs a full API URL for a given endpoint
   * @param endpoint - The API endpoint (e.g., '/users', '/courses')
   * @returns The full API URL
   */
  static getUrl(endpoint: string): string {
    // Remove leading slash if present to avoid double slashes
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint
    return `${this.baseUrl}/${cleanEndpoint}`
  }

  /**
   * Gets the base API URL
   * @returns The base API URL
   */
  static getBaseUrl(): string {
    return this.baseUrl
  }

  /**
   * Checks if we're running in development mode
   * @returns true if in development mode
   */
  static isDevelopment(): boolean {
    return config.isDevelopment
  }

  /**
   * Checks if we're running in production mode
   * @returns true if in production mode
   */
  static isProduction(): boolean {
    return config.isProduction
  }
}

export default ApiClient
