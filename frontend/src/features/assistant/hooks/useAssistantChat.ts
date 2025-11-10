import { useCallback, useEffect, useRef, useState } from 'react'
import type { AssistantMessage } from '../types/assistant.types'
import { assistantApi } from '../services/assistantService'

export function useAssistantChat() {
  const [messages, setMessages] = useState<AssistantMessage[]>([{
    id: 'welcome', role: 'assistant', content: 'Hi! I\'m Learnify AI. How can I help you today?', createdAt: Date.now()
  }])
  const [input, setInput] = useState('')
  const [isSending, setIsSending] = useState(false)
  const [isStreaming, setIsStreaming] = useState(false)
  const listRef = useRef<HTMLDivElement | null>(null)

  const canSend = input.trim().length > 0 && !isSending && !isStreaming

  const scrollToBottom = useCallback(() => {
    requestAnimationFrame(() => {
      if (listRef.current) {
        listRef.current.scrollTop = listRef.current.scrollHeight
      }
    })
  }, [])

  useEffect(() => { scrollToBottom() }, [messages, scrollToBottom])

  const send = useCallback(async () => {
    if (!canSend) return
    const text = input.trim()
    setInput('')
    const userMsg: AssistantMessage = { id: crypto.randomUUID(), role: 'user', content: text, createdAt: Date.now() }
    setMessages(prev => [...prev, userMsg])
    setIsSending(true)
    try {
      const res = await assistantApi.sendMessage({ message: text })
      // Add processing time to messages
      const messagesWithTime = res.messages.map(msg => ({
        ...msg,
        processingTime: res.processingTimeMs
      }))
      setMessages(prev => [...prev, ...messagesWithTime])
      
      // Log processing time if available
      if (res.processingTimeMs) {
        console.debug(`Assistant response processed in ${res.processingTimeMs}ms`)
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'An error occurred while sending your message. Please try again.'
      setMessages(prev => [...prev, { 
        id: crypto.randomUUID(), 
        role: 'assistant', 
        content: `Sorry, I encountered an error: ${errorMessage}`, 
        createdAt: Date.now() 
      }])
    } finally {
      setIsSending(false)
    }
  }, [canSend, input])

  const stream = useCallback(async () => {
    if (!canSend) return
    const text = input.trim()
    setInput('')
    const userMsg: AssistantMessage = { id: crypto.randomUUID(), role: 'user', content: text, createdAt: Date.now() }
    setMessages(prev => [...prev, userMsg])
    setIsStreaming(true)
    try {
      // No streaming endpoint yet; fall back to regular send
      const res = await assistantApi.sendMessage({ message: text })
      // Add processing time to messages
      const messagesWithTime = res.messages.map(msg => ({
        ...msg,
        processingTime: res.processingTimeMs
      }))
      setMessages(prev => [...prev, ...messagesWithTime])
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Streaming error.'
      setMessages(prev => [...prev, { id: crypto.randomUUID(), role: 'assistant', content: message, createdAt: Date.now() }])
    } finally {
      setIsStreaming(false)
    }
  }, [canSend, input])

  const reset = useCallback(() => {
    setMessages([{ id: 'welcome', role: 'assistant', content: 'Hi! I\'m Learnify AI. How can I help you today?', createdAt: Date.now() }])
  }, [])

  return {
    messages,
    input,
    setInput,
    isSending,
    isStreaming,
    canSend,
    send,
    stream,
    reset,
    listRef,
  }
}
