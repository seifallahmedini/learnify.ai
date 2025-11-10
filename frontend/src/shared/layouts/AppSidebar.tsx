import { NavLink, useLocation, matchPath } from "react-router-dom"
import { useEffect, useMemo, useRef, useState } from "react"
import {
  Sidebar,
  SidebarContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarFooter,
  SidebarGroupLabel,
  SidebarGroup,
  useSidebar,
  SidebarInput,
} from "@/shared/components/ui/sidebar"
import {
  Home,
  BookOpen,
  Users,
  User,
  GraduationCap,
  LogOut,
  FolderOpen,
  Search,
  Layers3,
  Bot,
} from "lucide-react"
import { Avatar, AvatarFallback, AvatarImage } from "@/shared/components/ui/avatar"
import { Badge } from "@/shared/components/ui/badge"
import { Separator } from "@/shared/components/ui/separator"
import type { LucideIcon } from "lucide-react"

// Type definitions for menu items
interface MenuItem {
  title: string
  url: string
  icon: LucideIcon
  description: string
  badge?: string | null
}

interface MenuGroup {
  title: string
  items: MenuItem[]
}

// Menu items configuration - aligned with actual routes
const menuItems: MenuGroup[] = [
  {
    title: "Overview",
    items: [
      {
        title: "Dashboard",
        url: "/",
        icon: Home,
        description: "Overview and quick insights",
      },
    ],
  },
  {
    title: "Learning",
    items: [
      {
        title: "Courses",
        url: "/courses",
        icon: BookOpen,
        description: "Browse and manage courses",
      },
      {
        title: "Categories",
        url: "/categories",
        icon: FolderOpen,
        description: "Course categories",
      },
      {
        title: "Learning Paths",
        url: "/learning-paths",
        icon: Layers3,
        description: "Curated course sequences",
      },
    ],
  },
  {
    title: "Assistant",
    items: [
      {
        title: "AI Assistant",
        url: "/assistant",
        icon: Bot,
        description: "Chat with Learnify AI",
        badge: "Beta",
      },
    ],
  },
  {
    title: "Management",
    items: [
      {
        title: "Users",
        url: "/users",
        icon: Users,
        description: "Manage learners and instructors",
      },
    ],
  },
  {
    title: "Account",
    items: [
      {
        title: "Profile",
        url: "/profile",
        icon: User,
        description: "Personal settings and preferences",
      },
    ],
  },
]

export function AppSidebar() {
  const location = useLocation()
  const { open, setOpen } = useSidebar()
  const [query, setQuery] = useState("")
  const inputRef = useRef<HTMLInputElement | null>(null)

  // Check if a route is active - exact for dashboard, pattern match for others
  const isActive = (url: string): boolean => {
    const pathname = location.pathname

    // Dashboard exact
    if (url === "/") return pathname === "/"

    // Courses: list, details, lessons, and quizzes are considered active
    if (url === "/courses") {
      const inCourses = Boolean(
        matchPath({ path: "/courses" }, pathname) ||
        matchPath({ path: "/courses/:courseId" }, pathname) ||
        matchPath({ path: "/courses/:courseId/lessons" }, pathname) ||
        matchPath({ path: "/courses/:courseId/quizzes" }, pathname)
      )
      return inCourses
    }

    // Groups: prefix match via matchPath wildcard
    return Boolean(
      matchPath({ path: `${url}` }, pathname) ||
      matchPath({ path: `${url}/*` }, pathname)
    )
  }

  // Filter items by search query across groups
  const filteredMenu = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return menuItems
    return menuItems
      .map(group => ({
        ...group,
        items: group.items.filter(i =>
          i.title.toLowerCase().includes(q) || i.description.toLowerCase().includes(q)
        ),
      }))
      .filter(group => group.items.length > 0)
  }, [query])

  // Keyboard shortcut: '/' to focus search, 'Ctrl/Cmd+B' handled by provider for sidebar toggle
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === '/' && !e.metaKey && !e.ctrlKey && !e.altKey) {
        e.preventDefault()
        setOpen(true)
        inputRef.current?.focus()
      }
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [setOpen])

  return (
    <Sidebar collapsible="icon" className="border-r" aria-label="Primary navigation sidebar">
      <SidebarHeader className="h-16 border-b border-sidebar-border">
        <div className={`flex items-center gap-3 px-4 ${!open ? 'justify-center' : ''}`}>
          <div className="flex aspect-square size-10 items-center justify-center rounded-xl bg-primary text-primary-foreground">
            <GraduationCap className="size-5" />
          </div>
          {open && (
            <div className="grid flex-1 text-left text-sm leading-tight">
              <span className="truncate font-bold text-lg">Learnify.AI</span>
              <span className="truncate text-xs text-muted-foreground">
                Learning Management System
              </span>
            </div>
          )}
        </div>
      </SidebarHeader>

      <SidebarContent className="px-3 py-2">
        <div className="space-y-3">
          {open && (
            <div className="px-2">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
                <SidebarInput
                  ref={inputRef as any}
                  placeholder="Search navigation... (/ to focus)"
                  value={query}
                  onChange={(e) => setQuery(e.target.value)}
                  className="pl-8"
                  aria-label="Search navigation"
                />
              </div>
            </div>
          )}
          {(query ? filteredMenu : menuItems).map((group, groupIndex) => (
            <SidebarGroup key={`group-${groupIndex}-${group.title}`} className="space-y-2">
              {open && (
                <SidebarGroupLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider px-2" aria-label={`${group.title} section`}>
                  {group.title}
                </SidebarGroupLabel>
              )}
              <SidebarMenu className="space-y-0.5">
                {group.items.map((item) => {
                  const active = isActive(item.url)
                  return (
                    <SidebarMenuItem key={item.url}>
                      <SidebarMenuButton
                        asChild
                        isActive={active}
                        tooltip={!open ? item.title : undefined}
                        className={`w-full transition-all duration-200 min-h-[2rem] ${
                          active
                            ? 'bg-primary/10 text-primary border-l-2 border-primary'
                            : 'hover:bg-muted/50'
                        } ${!open ? 'justify-center' : ''}`}
                      >
                        <NavLink
                          to={item.url}
                          aria-current={active ? 'page' : undefined}
                          className={`flex items-center gap-3 w-full focus:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 rounded-md ${!open ? 'justify-center' : ''}`}
                          end={item.url === '/'}
                          title={!open ? item.description : undefined}
                        >
                          <item.icon 
                            className={`size-5 shrink-0 transition-colors duration-200 ${
                              active 
                                ? 'text-primary' 
                                : 'text-muted-foreground'
                            }`} 
                          />
                          {open && (
                            <>
                              <div className="flex flex-col flex-1 min-w-0 py-1">
                                <span 
                                  className={`font-medium truncate transition-colors duration-200 leading-tight ${
                                    active ? 'text-primary font-semibold' : 'text-foreground'
                                  }`}
                                >
                                  {item.title}
                                </span>
                                <span className="text-xs text-muted-foreground truncate leading-relaxed">
                                  {item.description}
                                </span>
                              </div>
                              {item.badge && (
                                <Badge 
                                  variant={item.badge === "New" ? "default" : "secondary"} 
                                  className="ml-auto text-xs"
                                >
                                  {item.badge}
                                </Badge>
                              )}
                            </>
                          )}
                        </NavLink>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  )
                })}
              </SidebarMenu>
              {groupIndex < (query ? filteredMenu : menuItems).length - 1 && open && (
                <div className="px-2">
                  <Separator className="my-2" />
                </div>
              )}
            </SidebarGroup>
          ))}
        </div>
      </SidebarContent>

      <SidebarFooter className="p-3 border-t border-sidebar-border">
        {open ? (
          <div className="flex items-center gap-3 p-2.5 rounded-lg border bg-muted/50">
            <Avatar className="h-8 w-8">
              <AvatarImage src="/placeholder-avatar.jpg" alt="User" />
              <AvatarFallback className="bg-primary text-primary-foreground font-semibold text-sm">
                JD
              </AvatarFallback>
            </Avatar>
            <div className="flex flex-col flex-1 min-w-0">
              <span className="text-sm font-semibold truncate">John Doe</span>
              <span className="text-xs text-muted-foreground truncate">
                Administrator
              </span>
            </div>
            <SidebarMenuButton
              size="sm"
              tooltip="Sign out"
              className="ml-auto p-1.5 h-7 w-7 hover:bg-destructive/10 hover:text-destructive"
            >
              <LogOut className="size-4" />
            </SidebarMenuButton>
          </div>
        ) : (
          <div className="flex flex-col items-center gap-2">
            <Avatar className="h-8 w-8">
              <AvatarImage src="/placeholder-avatar.jpg" alt="User" />
              <AvatarFallback className="bg-primary text-primary-foreground font-semibold text-sm">
                JD
              </AvatarFallback>
            </Avatar>
            <SidebarMenuButton
              size="sm"
              tooltip="Sign out"
              className="p-1.5 h-7 w-7 hover:bg-destructive/10 hover:text-destructive"
            >
              <LogOut className="size-4" />
            </SidebarMenuButton>
          </div>
        )}
      </SidebarFooter>
    </Sidebar>
  )
}
