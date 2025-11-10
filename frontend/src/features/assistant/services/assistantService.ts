import config from '@/config'
import type { AssistantMessage, AssistantSendRequest, AssistantSendResponse, RawAssistantFunctionResponse } from '../types/assistant.types'

class AssistantApiService {
  private readonly maxRetries = 2
  private readonly retryDelay = 1000 // 1 second

  private getEndpoint(path: string) {
    const baseUrl = config.functionsBaseUrl
    const cleanBase = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl
    const cleanPath = path.startsWith('/') ? path.slice(1) : path
    const base = `${cleanBase}/${cleanPath}`
    const fnCode: string | undefined = (import.meta as any)?.env?.VITE_AZURE_FUNCTION_CODE
    if (fnCode && !base.includes('code=')) {
      const sep = base.includes('?') ? '&' : '?'
      return `${base}${sep}code=${encodeURIComponent(fnCode)}`
    }
    return base
  }

  private endpoint = this.getEndpoint('IntelligentEducationalAssistant')
  private azureEndpoint = this.getEndpoint('AzureOpenAIEducationalAssistant')

  /**
   * Builds a context-aware system prompt based on the request context
   */
  private buildSystemPrompt(payload: AssistantSendRequest): string | undefined {
    if (payload.systemPrompt) {
      return payload.systemPrompt
    }

    // Build context-aware prompt if context is provided
    if (payload.context) {
      const contextParts: string[] = []
      
      if (payload.context.lessonTitle) {
        contextParts.push(`Current Lesson: ${payload.context.lessonTitle}`)
      }
      if (payload.context.courseTitle) {
        contextParts.push(`Course: ${payload.context.courseTitle}`)
      }
      if (payload.context.lessonDescription) {
        contextParts.push(`Lesson Description: ${payload.context.lessonDescription}`)
      }
      if (payload.context.learningObjectives) {
        const objectives = Array.isArray(payload.context.learningObjectives)
          ? payload.context.learningObjectives.join(', ')
          : String(payload.context.learningObjectives)
        contextParts.push(`Learning Objectives: ${objectives}`)
      }

      if (contextParts.length > 0) {
        return `You are an expert educational AI assistant. Context:\n${contextParts.join('\n')}\n\nProvide helpful, accurate, and educational responses based on this context.`
      }
    }

    return undefined
  }

  /**
   * Parses processing time from TimeSpan string format
   */
  private parseProcessingTime(timeSpan?: string): number | undefined {
    if (!timeSpan) return undefined
    
    try {
      // Handle TimeSpan format: "00:00:00.1234567" or "00:00:01.2345678"
      const match = /^(\d+):(\d+):(\d+\.\d+)$/.exec(timeSpan)
      if (match) {
        const [, hours, minutes, seconds] = match
        const totalSeconds = Number(hours) * 3600 + Number(minutes) * 60 + Number(seconds)
        return Math.round(totalSeconds * 1000) // Convert to milliseconds
      }
      
      // Fallback: try to parse as number (assuming milliseconds)
      const num = Number(timeSpan)
      if (!isNaN(num)) return num
    } catch (error) {
      console.warn('Failed to parse processing time:', timeSpan, error)
    }
    
    return undefined
  }

  /**
   * Retry logic with exponential backoff
   */
  private async retryRequest<T>(
    requestFn: () => Promise<T>,
    retries = this.maxRetries
  ): Promise<T> {
    try {
      return await requestFn()
    } catch (error) {
      if (retries > 0 && this.isRetryableError(error)) {
        await this.delay(this.retryDelay)
        return this.retryRequest(requestFn, retries - 1)
      }
      throw error
    }
  }

  /**
   * Checks if an error is retryable (network errors, 5xx status codes)
   */
  private isRetryableError(error: unknown): boolean {
    if (error instanceof Error) {
      // Network errors are retryable
      if (error.message.includes('fetch') || error.message.includes('network')) {
        return true
      }
      // 5xx errors are retryable
      if (error.message.match(/5\d{2}/)) {
        return true
      }
    }
    return false
  }

  /**
   * Delay helper for retries
   */
  private delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms))
  }

  /**
   * Sends a message to the Intelligent Educational Assistant
   * Uses the IntelligentEducationalAssistant endpoint with enhanced error handling
   */
  async sendMessage(payload: AssistantSendRequest): Promise<AssistantSendResponse> {
    const systemPrompt = this.buildSystemPrompt(payload)
    const body = JSON.stringify({ 
      message: payload.message, 
      systemPrompt 
    })

    return this.retryRequest(async () => {
      const startTime = performance.now()
      
      const res = await fetch(this.endpoint, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body,
        signal: AbortSignal.timeout(120000) // 2 minute timeout
      })

      const clientProcessingTime = Math.round(performance.now() - startTime)

      if (!res.ok) {
        const text = await res.text()
        let errorMessage = `Request failed (${res.status})`
        
        try {
          const errorJson = JSON.parse(text)
          errorMessage = errorJson.error || errorJson.message || errorMessage
        } catch {
          errorMessage = text || errorMessage
        }
        
        throw new Error(errorMessage)
      }

      const raw: RawAssistantFunctionResponse = await res.json()
      
      if (!raw.success) {
        throw new Error(raw.error || 'Assistant backend error')
      }

      if (!raw.response) {
        throw new Error('Empty response from assistant')
      }

      const assistantMsg: AssistantMessage = {
        id: crypto.randomUUID(),
        role: 'assistant',
        content: raw.response,
        createdAt: Date.now()
      }

      const processingTimeMs = this.parseProcessingTime(raw.processingTime) || clientProcessingTime

      return { 
        messages: [assistantMsg], 
        processingTimeMs 
      }
    })
  }

  /**
   * Sends a message to the Azure OpenAI Educational Assistant
   * Alternative endpoint for Azure-specific features
   */
  async sendMessageAzure(payload: AssistantSendRequest): Promise<AssistantSendResponse> {
    const systemPrompt = this.buildSystemPrompt(payload)
    const body = JSON.stringify({ 
      message: payload.message, 
      systemPrompt 
    })

    return this.retryRequest(async () => {
      const res = await fetch(this.azureEndpoint, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body,
        signal: AbortSignal.timeout(120000) // 2 minute timeout
      })

      if (!res.ok) {
        const text = await res.text()
        let errorMessage = `Azure Assistant request failed (${res.status})`
        
        try {
          const errorJson = JSON.parse(text)
          errorMessage = errorJson.error || errorJson.message || errorMessage
        } catch {
          errorMessage = text || errorMessage
        }
        
        throw new Error(errorMessage)
      }

      const raw: RawAssistantFunctionResponse = await res.json()
      
      if (!raw.success) {
        throw new Error(raw.error || 'Azure assistant backend error')
      }

      if (!raw.response) {
        throw new Error('Empty response from Azure assistant')
      }

      const assistantMsg: AssistantMessage = {
        id: crypto.randomUUID(),
        role: 'assistant',
        content: raw.response,
        createdAt: Date.now()
      }

      const processingTimeMs = this.parseProcessingTime(raw.processingTime)

      return { 
        messages: [assistantMsg],
        processingTimeMs 
      }
    })
  }
}

export const assistantApi = new AssistantApiService()
