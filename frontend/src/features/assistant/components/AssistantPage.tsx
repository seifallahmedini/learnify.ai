import { Card, CardHeader, CardTitle, CardContent, CardFooter } from '@/shared/components/ui/card'
import { Input } from '@/shared/components/ui/input'
import { Button } from '@/shared/components/ui/button'
import { Separator } from '@/shared/components/ui/separator'
import { Send, Bot, User as UserIcon, RefreshCcw, Zap } from 'lucide-react'
import { useAssistantChat } from '../hooks/useAssistantChat'
import { Badge } from '@/shared/components/ui/badge'

export function AssistantPage() {
  const {
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
  } = useAssistantChat()

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
      e.preventDefault()
      send()
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-end justify-between gap-3 flex-wrap">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Learnify Assistant</h1>
          <p className="text-sm text-muted-foreground">Ask questions about courses, categories, users.</p>
        </div>
        <Badge variant="secondary" className="h-7">Beta</Badge>
      </div>
      <Card className="overflow-hidden">
        <CardHeader>
          <div className="flex items-center justify-between gap-3 flex-wrap">
            <CardTitle className="text-lg">Ask anything about your learning data</CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" onClick={reset} disabled={isSending || isStreaming} className="gap-1">
                <RefreshCcw className="h-3.5 w-3.5" /> Reset
              </Button>
              <Button variant="outline" size="sm" onClick={stream} disabled={!canSend} className="gap-1">
                <Zap className="h-3.5 w-3.5" /> Stream
              </Button>
            </div>
          </div>
        </CardHeader>
        <Separator />
        <CardContent className="p-0">
          <div ref={listRef} className="h-[480px] overflow-auto p-4 space-y-4">
            {messages.map(m => (
              <div key={m.id} className={`flex items-start gap-3 ${m.role === 'user' ? 'justify-end' : ''}`}>
                {m.role === 'assistant' && (
                  <div className="mt-0.5 rounded-md bg-primary/10 p-1 text-primary"><Bot className="h-4 w-4" /></div>
                )}
                <div className={`max-w-[75%] rounded-lg px-3 py-2 text-sm leading-relaxed shadow ${m.role === 'user' ? 'bg-primary text-primary-foreground' : 'bg-muted'}`}>
                  <div className="whitespace-pre-wrap break-words">{m.content}</div>
                  {m.processingTime && (
                    <div className="mt-1.5 text-xs opacity-60">
                      Processed in {m.processingTime < 1000 ? `${m.processingTime}ms` : `${(m.processingTime / 1000).toFixed(1)}s`}
                    </div>
                  )}
                </div>
                {m.role === 'user' && (
                  <div className="mt-0.5 rounded-md bg-muted p-1"><UserIcon className="h-4 w-4 text-muted-foreground" /></div>
                )}
              </div>
            ))}
            {(isSending || isStreaming) && (
              <div className="flex items-start gap-3">
                <div className="mt-0.5 rounded-md bg-primary/10 p-1 text-primary"><Bot className="h-4 w-4" /></div>
                <div className="rounded-lg bg-muted px-3 py-2 text-sm shadow">
                  <span className="inline-block h-2 w-2 animate-bounce rounded-full bg-muted-foreground/70 mr-1" />
                  <span className="inline-block h-2 w-2 animate-bounce rounded-full bg-muted-foreground/70 mr-1 [animation-delay:120ms]" />
                  <span className="inline-block h-2 w-2 animate-bounce rounded-full bg-muted-foreground/70 [animation-delay:240ms]" />
                </div>
              </div>
            )}
          </div>
        </CardContent>
        <Separator />
        <CardFooter className="p-4">
          <div className="flex w-full items-center gap-2">
            <Input
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder="Type your message... (Ctrl/⌘ + Enter to send)"
              aria-label="Chat input"
              disabled={isSending || isStreaming}
            />
            <Button onClick={send} disabled={!canSend} className="gap-2">
              <Send className="h-4 w-4" /> Send
            </Button>
          </div>
        </CardFooter>
      </Card>
    </div>
  )
}
