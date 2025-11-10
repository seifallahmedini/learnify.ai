import { Outlet, useLocation, useMatches, useNavigation, ScrollRestoration } from "react-router-dom"
import { SidebarProvider, SidebarInset, SidebarTrigger } from "@/shared/components/ui/sidebar"
import { Separator } from "@/shared/components/ui/separator"
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/shared/components/ui/breadcrumb"
import { AppSidebar } from "./AppSidebar"
import { Suspense, useMemo, useEffect, useRef } from "react"

// Loading component for better UX
function PageLoader() {
  return (
    <div className="flex items-center justify-center h-64">
      <div className="flex flex-col items-center space-y-4">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
        <p className="text-sm text-muted-foreground">Loading...</p>
      </div>
    </div>
  )
}

export function MainLayout() {
  const location = useLocation()
  const matches = useMatches()
  const navigation = useNavigation()
  const mainRef = useRef<HTMLDivElement | null>(null)
  
  // Build breadcrumb items based on route matches + handles
  const breadcrumbItems = useMemo(() => {
    const seen = new Set<string>()
    const items: Array<{ label: string; href: string }> = []
    matches.forEach(match => {
      const pathname = match.pathname || '/'
      if (seen.has(pathname)) return
      seen.add(pathname)
      // Derive label from handle if provided
      let label: string | undefined
      const handle: any = match.handle
      if (handle) {
        if (typeof handle === 'function') {
          const res = handle({ params: match.params })
          if (typeof res === 'string') label = res
          else if (res?.breadcrumb) label = res.breadcrumb
        } else if (handle.breadcrumb) {
          if (typeof handle.breadcrumb === 'function') {
            label = handle.breadcrumb({ params: match.params })
          } else {
            label = handle.breadcrumb
          }
        }
      }
      if (!label) {
        if (pathname === '/') label = 'Dashboard'
        else {
          const segments = pathname.split('/').filter(Boolean)
          const last = segments[segments.length - 1]
          if (/^[0-9]+$/.test(last)) {
            const parent = segments[segments.length - 2]
            switch (parent) {
              case 'courses': label = 'Course'; break
              case 'lessons': label = 'Lesson'; break
              case 'categories': label = 'Category'; break
              case 'users': label = 'User'; break
              case 'quizzes': label = 'Quiz'; break
              default: label = 'Details'
            }
          } else {
            label = last.charAt(0).toUpperCase() + last.slice(1).replace(/-/g, ' ')
          }
        }
      }
      items.push({ label, href: pathname })
    })
    return items
  }, [matches])

  // Update document title from last breadcrumb
  useEffect(() => {
    const last = breadcrumbItems[breadcrumbItems.length - 1]
    if (last) {
      document.title = `${last.label} | Learnify.ai`
    }
  }, [breadcrumbItems])

  // Focus main content on path change for accessibility
  useEffect(() => {
    mainRef.current?.focus()
  }, [location.pathname])
  
  return (
    <SidebarProvider defaultOpen={true}>
      <div className="min-h-screen flex w-full bg-background">
        {/* Skip link for keyboard users */}
        <a href="#main-content" className="sr-only focus:not-sr-only focus:absolute focus:top-2 focus:left-2 bg-primary text-primary-foreground px-3 py-1 rounded-md text-sm z-50">Skip to content</a>
        <AppSidebar />
        <SidebarInset className="flex-1 flex flex-col overflow-hidden">
          {/* Header with sidebar trigger and breadcrumbs */}
          <header className="sticky top-0 z-40 border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60" role="banner">
            {/* Route transition progress bar */}
            {navigation.state === 'loading' && (
              <div className="h-0.5 w-full bg-primary/20">
                <div className="h-full w-1/3 animate-[progress_1.2s_linear_infinite] bg-primary" />
              </div>
            )}
            <div className="flex h-14 items-center gap-4 px-4 lg:px-6">
              <SidebarTrigger className="-ml-1" aria-label="Toggle sidebar navigation" />
              <Separator orientation="vertical" className="h-4" aria-hidden="true" />
              <Breadcrumb role="navigation" aria-label="Breadcrumb">
                <BreadcrumbList>
                  {breadcrumbItems.map((item, index) => {
                    const isLast = index === breadcrumbItems.length - 1
                    return (
                      <div key={item.href} className="flex items-center">
                        {index > 0 && (
                          <BreadcrumbSeparator className="hidden md:block mx-2" aria-hidden="true" />
                        )}
                        <BreadcrumbItem className={index === 0 ? "hidden md:block" : ""}>
                          {isLast ? (
                            <BreadcrumbPage aria-current="page">
                              {item.label}
                            </BreadcrumbPage>
                          ) : (
                            <BreadcrumbLink href={item.href}>
                              {item.label}
                            </BreadcrumbLink>
                          )}
                        </BreadcrumbItem>
                      </div>
                    )
                  })}
                </BreadcrumbList>
              </Breadcrumb>
            </div>
          </header>

          {/* Main content area */}
          <main id="main-content" ref={mainRef} tabIndex={-1} className="flex-1 overflow-auto outline-none" role="main" aria-label="Main content">
            <div className="container max-w-screen-2xl mx-auto p-4 lg:p-6 space-y-6">
              <Suspense fallback={<PageLoader />}>
                <Outlet />
                <ScrollRestoration />
              </Suspense>
            </div>
          </main>

          {/* Footer */}
          <footer className="border-t border-border/40 bg-muted/30" role="contentinfo">
            <div className="container max-w-screen-2xl mx-auto px-4 lg:px-6 py-4">
              <div className="flex flex-col sm:flex-row justify-between items-center gap-4 text-sm text-muted-foreground">
                <p>© 2025 Learnify.ai. All rights reserved.</p>
                <nav aria-label="Footer navigation">
                  <div className="flex items-center gap-4">
                    <a href="#" className="hover:text-foreground transition-colors focus:underline focus:outline-none">Privacy</a>
                    <a href="#" className="hover:text-foreground transition-colors focus:underline focus:outline-none">Terms</a>
                    <a href="#" className="hover:text-foreground transition-colors focus:underline focus:outline-none">Support</a>
                  </div>
                </nav>
              </div>
            </div>
          </footer>
        </SidebarInset>
      </div>
    </SidebarProvider>
  )
}