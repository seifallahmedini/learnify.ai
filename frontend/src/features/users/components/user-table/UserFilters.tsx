import { useState } from 'react';
import { Card, CardContent } from "@/shared/components/ui/card"
import { Input } from "@/shared/components/ui/input"
import { Label } from "@/shared/components/ui/label"
import { Button } from "@/shared/components/ui/button"
import { Badge } from "@/shared/components/ui/badge"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select"
import { Search, Filter, RotateCcw, ChevronDown, ChevronUp, X } from "lucide-react"
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
  const [isExpanded, setIsExpanded] = useState(false);

  const getActiveFiltersCount = () => {
    let count = 0;
    if (searchTerm) count++;
    if (selectedRole !== "all") count++;
    if (selectedStatus !== "all") count++;
    return count;
  };

  const resetFilters = () => {
    onSearchChange('');
    onRoleChange('all');
    onStatusChange('all');
  };

  const getRoleLabel = (role: UserRole | "all") => {
    if (role === "all") return "All Roles";
    switch (role.toString()) {
      case "1": return "Students";
      case "2": return "Instructors";
      case "3": return "Admins";
      default: return "Unknown";
    }
  };

  const getStatusLabel = (status: "all" | "active" | "inactive") => {
    switch (status) {
      case "all": return "All Status";
      case "active": return "Active";
      case "inactive": return "Inactive";
      default: return "Unknown";
    }
  };

  const activeFiltersCount = getActiveFiltersCount();

  return (
    <div className="space-y-3">
      {/* Compact Header with Quick Filters */}
      <div className="flex items-center justify-between p-3 bg-muted/30 rounded-lg border border-l-4 border-l-primary">
        <div className="flex items-center gap-3">
          <button
            onClick={() => setIsExpanded(!isExpanded)}
            className="flex items-center gap-2 text-sm font-medium hover:text-primary transition-colors"
          >
            <Filter className="h-4 w-4 text-primary" />
            Filters
            {activeFiltersCount > 0 && (
              <Badge variant="secondary" className="bg-primary text-primary-foreground text-xs px-1.5 py-0.5">
                {activeFiltersCount}
              </Badge>
            )}
            {isExpanded ? (
              <ChevronUp className="h-3 w-3 text-muted-foreground" />
            ) : (
              <ChevronDown className="h-3 w-3 text-muted-foreground" />
            )}
          </button>
        </div>
        
        {/* Quick Action Buttons */}
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={resetFilters}
            className="h-7 px-2 text-xs text-muted-foreground hover:text-foreground"
          >
            <RotateCcw className="h-3 w-3" />
          </Button>
        </div>
      </div>

      {/* Compact Active Filters Bar */}
      {!isExpanded && activeFiltersCount > 0 && (
        <div className="flex flex-wrap gap-1.5 px-3 py-2 bg-muted/20 rounded border border-dashed">
          {searchTerm && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-blue-100 text-blue-800 border-blue-200 hover:bg-blue-200 transition-colors">
              Search: {searchTerm}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-blue-900" 
                onClick={() => onSearchChange('')}
              />
            </Badge>
          )}
          {selectedRole !== "all" && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-purple-100 text-purple-800 border-purple-200 hover:bg-purple-200 transition-colors">
              Role: {getRoleLabel(selectedRole)}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-purple-900" 
                onClick={() => onRoleChange('all')}
              />
            </Badge>
          )}
          {selectedStatus !== "all" && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-green-100 text-green-800 border-green-200 hover:bg-green-200 transition-colors">
              Status: {getStatusLabel(selectedStatus)}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-green-900" 
                onClick={() => onStatusChange('all')}
              />
            </Badge>
          )}
        </div>
      )}

      {/* Compact Expanded Content */}
      {isExpanded && (
        <Card className="shadow-sm border-l-4 border-l-primary">
          <CardContent className="p-4 space-y-4">
            {/* Search Input */}
            <div className="space-y-1.5">
              <Label className="text-xs font-medium text-muted-foreground">Search Users</Label>
              <div className="relative">
                <Search className="absolute left-2 top-2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Search by name, email..."
                  value={searchTerm}
                  onChange={(e) => onSearchChange(e.target.value)}
                  className="h-8 text-sm pl-8"
                />
              </div>
            </div>

            {/* Role and Status Filters */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
              <div className="space-y-1.5">
                <Label className="text-xs font-medium text-muted-foreground">Role</Label>
                <Select 
                  value={selectedRole === "all" ? "all" : selectedRole.toString()} 
                  onValueChange={onRoleChange}
                >
                  <SelectTrigger className="h-8 text-sm">
                    <SelectValue placeholder="All roles" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Roles</SelectItem>
                    <SelectItem value="1">Students</SelectItem>
                    <SelectItem value="2">Instructors</SelectItem>
                    <SelectItem value="3">Admins</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-1.5">
                <Label className="text-xs font-medium text-muted-foreground">Status</Label>
                <Select value={selectedStatus} onValueChange={onStatusChange}>
                  <SelectTrigger className="h-8 text-sm">
                    <SelectValue placeholder="All statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Status</SelectItem>
                    <SelectItem value="active">Active</SelectItem>
                    <SelectItem value="inactive">Inactive</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            {/* Action Button */}
            <div className="flex gap-2 pt-2 border-t border-border/50">
              <Button variant="outline" onClick={resetFilters} size="sm" className="h-8">
                <RotateCcw className="h-3 w-3 mr-1" />
                Reset Filters
              </Button>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
