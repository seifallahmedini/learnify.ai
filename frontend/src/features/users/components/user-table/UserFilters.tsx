import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/components/ui/card"
import { Input } from "@/shared/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select"
import { Search } from "lucide-react"
import type { UserRole } from "../../types"

interface UserFiltersProps {
  searchTerm: string
  selectedRole: UserRole | "all"
  selectedStatus: "all" | "active" | "inactive"
  onSearchChange: (value: string) => void
  onRoleChange: (role: string) => void
  onStatusChange: (status: string) => void
}

export function UserFilters({
  searchTerm,
  selectedRole,
  selectedStatus,
  onSearchChange,
  onRoleChange,
  onStatusChange,
}: UserFiltersProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Filters</CardTitle>
        <CardDescription>Filter users by role, status, and search</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex flex-col gap-4 md:flex-row md:items-center">
          <div className="relative flex-1">
            <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search users..."
              value={searchTerm}
              onChange={(e) => onSearchChange(e.target.value)}
              className="pl-8"
            />
          </div>
          <Select 
            value={selectedRole === "all" ? "all" : selectedRole.toString()} 
            onValueChange={onRoleChange}
          >
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Select role" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Roles</SelectItem>
              <SelectItem value="1">Students</SelectItem>
              <SelectItem value="2">Instructors</SelectItem>
              <SelectItem value="3">Admins</SelectItem>
            </SelectContent>
          </Select>
          <Select value={selectedStatus} onValueChange={onStatusChange}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Select status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Status</SelectItem>
              <SelectItem value="active">Active</SelectItem>
              <SelectItem value="inactive">Inactive</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </CardContent>
    </Card>
  )
}
