import { Outlet, useLocation } from "react-router-dom"
import { SidebarProvider, SidebarInset, SidebarTrigger } from "@/shared/components/ui/sidebar"
import { Separator } from "@/shared/components/ui/separator"
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/shared/components/ui/breadcrumb"
import { AppSidebar } from "./AppSidebar"
import { Suspense, useMemo } from "react"

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
  
  // Build breadcrumb items based on path segments
  const breadcrumbItems = useMemo(() => {
    const segments = location.pathname.split('/').filter(Boolean)
    const items: Array<{ label: string; href: string }> = []
    
    // Always start with Dashboard
    items.push({ label: 'Dashboard', href: '/' })
    
    // Build breadcrumb path
    let currentPath = ''
    segments.forEach((segment, index) => {
      currentPath += `/${segment}`
      
      // Handle numeric IDs (route parameters)
      if (!isNaN(Number(segment))) {
        // Check if next segment is 'lessons' (nested route)
        const nextSegment = segments[index + 1]
        if (nextSegment === 'lessons' && segments[index - 1] === 'courses') {
          // Skip this numeric segment, will be handled in next iteration
          return
        }
        
        // This is a parameter, use a generic label based on parent route
        if (segments[index - 1] === 'courses') {
          items.push({ label: 'Course Details', href: currentPath })
        } else if (segments[index - 1] === 'lessons') {
          items.push({ label: 'Lesson Details', href: currentPath })
        } else if (segments[index - 1] === 'quizzes') {
          items.push({ label: 'Quiz Details', href: currentPath })
        } else if (segments[index - 1] === 'categories') {
          items.push({ label: 'Category Details', href: currentPath })
        } else if (segments[index - 1] === 'users') {
          items.push({ label: 'User Details', href: currentPath })
        } else {
          items.push({ label: 'Details', href: currentPath })
        }
      } else {
        // Handle special nested routes
        const prevSegment = segments[index - 1]
        const prevIsNumeric = index > 0 && !isNaN(Number(prevSegment))
        
        if (segment === 'lessons' && prevIsNumeric && segments[index - 2] === 'courses') {
          // This is /courses/:id/lessons route
          items.push({ label: 'Course Lessons', href: currentPath })
        } else if (segment === 'quizzes' && prevIsNumeric && segments[index - 2] === 'courses') {
          // This is /courses/:id/quizzes route
          items.push({ label: 'Course Quizzes', href: currentPath })
        } else {
          // Regular route segment, capitalize it
          const label = segment.charAt(0).toUpperCase() + segment.slice(1).replace(/-/g, ' ')
          items.push({ label, href: currentPath })
        }
      }
    })
    
    return items
  }, [location.pathname])
  
  return (
    <SidebarProvider defaultOpen={true}>
      <div className="min-h-screen flex w-full bg-background">
        <AppSidebar />
        <SidebarInset className="flex-1 flex flex-col overflow-hidden">
          {/* Header with sidebar trigger and breadcrumbs */}
          <header className="sticky top-0 z-40 border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60" role="banner">
            <div className="flex h-14 items-center gap-4 px-4 lg:px-6">
              <SidebarTrigger className="-ml-1" aria-label="Toggle sidebar navigation" />
              <Separator orientation="vertical" className="h-4" aria-hidden="true" />
              <Breadcrumb role="navigation" aria-label="Breadcrumb navigation">
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
          <main className="flex-1 overflow-auto" role="main" aria-label="Main content">
            <div className="container max-w-screen-2xl mx-auto p-4 lg:p-6 space-y-6">
              <Suspense fallback={<PageLoader />}>
                <Outlet />
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