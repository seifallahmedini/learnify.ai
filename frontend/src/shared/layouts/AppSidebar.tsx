import { Link, useLocation } from "react-router-dom"
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
} from "@/shared/components/ui/sidebar"
import {
  Home,
  BookOpen,
  Users,
  User,
  GraduationCap,
  BarChart3,
  Settings,
  Award,
  Calendar,
  HelpCircle,
  LogOut,
  FolderOpen,
  CreditCard,
  MessageSquare,
  BookOpenCheck,
  TrendingUp,
} from "lucide-react"
import { Avatar, AvatarFallback, AvatarImage } from "@/shared/components/ui/avatar"
import { Badge } from "@/shared/components/ui/badge"
import { Separator } from "@/shared/components/ui/separator"

// Menu items configuration
const menuItems = [
  {
    title: "Overview",
    items: [
      {
        title: "Dashboard",
        url: "/",
        icon: Home,
        description: "Overview and analytics",
        badge: null,
      },
      {
        title: "Analytics",
        url: "/analytics",
        icon: TrendingUp,
        description: "Platform insights and metrics",
        badge: null,
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
        badge: "12",
      },
      {
        title: "Categories",
        url: "/categories",
        icon: FolderOpen,
        description: "Course categories",
        badge: null,
      },
      {
        title: "Lessons",
        url: "/lessons",
        icon: BookOpenCheck,
        description: "Individual lesson content",
        badge: null,
      },
    ],
  },
  {
    title: "Assessment",
    items: [
      {
        title: "Assessments",
        url: "/assessments",
        icon: Award,
        description: "Quizzes and evaluations",
        badge: "New",
      },
      {
        title: "Progress",
        url: "/progress",
        icon: BarChart3,
        description: "Learning progress tracking",
        badge: null,
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
        badge: null,
      },
      {
        title: "Enrollments",
        url: "/enrollments",
        icon: Calendar,
        description: "Course enrollments",
        badge: "5",
      },
      {
        title: "Reviews",
        url: "/reviews",
        icon: MessageSquare,
        description: "Course reviews and feedback",
        badge: null,
      },
    ],
  },
  {
    title: "Finance",
    items: [
      {
        title: "Payments",
        url: "/payments",
        icon: CreditCard,
        description: "Payment management",
        badge: null,
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
        description: "Personal settings",
        badge: null,
      },
      {
        title: "Settings",
        url: "/settings",
        icon: Settings,
        description: "App preferences",
        badge: null,
      },
    ],
  },
]

const helpItems = [
  {
    title: "Help Center",
    url: "/help",
    icon: HelpCircle,
  },
  {
    title: "Support",
    url: "/support",
    icon: HelpCircle,
  },
]

export function AppSidebar() {
  const location = useLocation()
  const { open } = useSidebar()

  const isActive = (url: string) => {
    if (url === "/") {
      return location.pathname === "/"
    }
    return location.pathname.startsWith(url)
  }

  return (
    <Sidebar collapsible="icon" className="border-r">
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
          {menuItems.map((group, groupIndex) => (
            <SidebarGroup key={groupIndex} className="space-y-2">
              {open && (
                <SidebarGroupLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider px-2">
                  {group.title}
                </SidebarGroupLabel>
              )}
              <SidebarMenu className="space-y-0.5">
                {group.items.map((item) => (
                  <SidebarMenuItem key={item.url}>
                    <SidebarMenuButton
                      asChild
                      isActive={isActive(item.url)}
                      tooltip={!open ? item.title : undefined}
                      className={`w-full transition-all duration-200 min-h-[2rem] ${
                        isActive(item.url)
                          ? 'bg-primary/10 text-primary border-l-2 border-primary'
                          : 'hover:bg-muted/50'
                      } ${!open ? 'justify-center' : ''}`}
                    >
                      <Link to={item.url} className={`flex items-center gap-3 w-full ${!open ? 'justify-center' : ''}`}>
                        <item.icon className={`size-5 shrink-0 transition-colors duration-200 ${
                          isActive(item.url) 
                            ? 'text-primary' 
                            : 'text-muted-foreground'
                        }`} />
                        {open && (
                          <>
                            <div className="flex flex-col flex-1 min-w-0 py-1">
                              <span className={`font-medium truncate transition-colors duration-200 leading-tight ${
                                isActive(item.url) ? 'text-primary font-semibold' : 'text-foreground'
                              }`}>
                                {item.title}
                              </span>
                              <span className="text-xs text-muted-foreground truncate leading-relaxed">
                                {item.description}
                              </span>
                            </div>
                            {item.badge && (
                              <Badge 
                                variant={item.badge === "New" ? "default" : "secondary"} 
                                className={`ml-auto text-xs ${item.badge === "New" ? 'bg-green-500 hover:bg-green-600' : ''}`}
                              >
                                {item.badge}
                              </Badge>
                            )}
                          </>
                        )}
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
              {groupIndex < menuItems.length - 1 && open && (
                <div className="px-2">
                  <Separator className="my-2" />
                </div>
              )}
            </SidebarGroup>
          ))}

          {/* Help Section */}
          {open && (
            <SidebarGroup className="space-y-2">
              <div className="px-2">
                <Separator className="my-2" />
              </div>
              <SidebarGroupLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider px-2">
                Support
              </SidebarGroupLabel>
              <SidebarMenu className="space-y-0.5">
                {helpItems.map((item) => (
                  <SidebarMenuItem key={item.url}>
                    <SidebarMenuButton
                      asChild
                      tooltip={!open ? item.title : undefined}
                      className="w-full transition-all duration-200 hover:bg-muted/50"
                    >
                      <Link to={item.url} className="flex items-center gap-3">
                        <item.icon className="size-5 text-muted-foreground transition-colors duration-200" />
                        <span className="font-medium text-foreground">{item.title}</span>
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroup>
          )}
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
