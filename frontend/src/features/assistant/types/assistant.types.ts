export type AssistantRole = 'user' | 'assistant' | 'system'

export interface AssistantMessage {
  id: string
  role: AssistantRole
  content: string
  createdAt: number
}

export interface AssistantSendRequest {
  message: string
  context?: Record<string, unknown>
  systemPrompt?: string
}

export interface AssistantSendResponse {
  messages: AssistantMessage[]
  processingTimeMs?: number
}

export interface AssistantStreamChunk {
  id: string
  delta: string
  done?: boolean
}

export interface AssistantError {
  message: string
  code?: string
  status?: number
}

// Backend Azure Function raw response shape (IntelligentEducationalAssistant)
export interface RawAssistantFunctionResponse {
  success: boolean
  response: string
  processingTime?: string
  error?: string
}
