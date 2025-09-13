import { Outlet } from "react-router-dom"
import { SidebarProvider, SidebarInset, SidebarTrigger } from "@/shared/components/ui/sidebar"
import { Separator } from "@/shared/components/ui/separator"
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/shared/components/ui/breadcrumb"
import { AppSidebar } from "./AppSidebar"
import { Suspense } from "react"

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
                  <BreadcrumbItem className="hidden md:block">
                    <BreadcrumbLink href="/">
                      Dashboard
                    </BreadcrumbLink>
                  </BreadcrumbItem>
                  <BreadcrumbSeparator className="hidden md:block" aria-hidden="true" />
                  <BreadcrumbItem>
                    <BreadcrumbPage aria-current="page">Current Page</BreadcrumbPage>
                  </BreadcrumbItem>
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