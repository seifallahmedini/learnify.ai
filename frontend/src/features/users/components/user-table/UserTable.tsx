import { useState } from 'react';
import { Card } from "@/shared/components/ui/card"
import { Button } from "@/shared/components/ui/button"
import {
  Table,
  TableBody,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table"
import { 
  Users, 
  UserPlus,
  Edit,
  UserCheck,
  UserX,
  Trash2
} from "lucide-react"
import type { UserSummary } from "../../types"
import { UserTableRow } from "./UserTableRow"
import { LoadingSkeleton } from "../shared/LoadingSkeleton"

interface UserTableProps {
  users: UserSummary[]
  loading: boolean
  totalCount: number
  currentPage: number
  totalPages: number
  pageSize: number
  onActivateUser: (userId: number) => void
  onDeactivateUser: (userId: number) => void
  onDeleteUser: (userId: number) => void
  onViewUser?: (userId: number) => void
  onEditUser?: (userId: number) => void
  onPageChange: (page: number) => void
  onCreateUser?: () => void
  onClearFilters?: () => void
  hasFilters?: boolean
}

export function UserTable({
  users,
  loading,
  totalCount,
  currentPage,
  totalPages,
  pageSize,
  onActivateUser,
  onDeactivateUser,
  onDeleteUser,
  onViewUser,
  onEditUser,
  onPageChange,
  onCreateUser,
  onClearFilters,
  hasFilters = false,
}: UserTableProps) {
  const [selectedUsers, setSelectedUsers] = useState<number[]>([]);

  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      setSelectedUsers(users.map(user => user.id));
    } else {
      setSelectedUsers([]);
    }
  };

  const handleSelectUser = (userId: number, checked: boolean) => {
    if (checked) {
      setSelectedUsers(prev => [...prev, userId]);
    } else {
      setSelectedUsers(prev => prev.filter(id => id !== userId));
    }
  };

  const isAllSelected = users.length > 0 && selectedUsers.length === users.length;
  if (users.length === 0 && !loading) {
    return (
      <Card className="p-12">
        <div className="text-center space-y-4">
          <div className="mx-auto w-16 h-16 bg-muted rounded-full flex items-center justify-center">
            <Users className="w-8 h-8 text-muted-foreground" />
          </div>
          <div className="space-y-2">
            <h3 className="text-lg font-semibold text-foreground">
              {hasFilters ? "No users match your filters" : "No users found"}
            </h3>
            <p className="text-sm text-muted-foreground max-w-md mx-auto">
              {hasFilters 
                ? "Try adjusting your search terms or filters to find what you're looking for. You can also clear all filters to see all users."
                : "Get started by creating your first user account. Users can be students, instructors, or administrators."
              }
            </p>
          </div>
          {!hasFilters && onCreateUser && (
            <Button onClick={onCreateUser} size="lg" className="flex items-center gap-2">
              <UserPlus className="h-5 w-5" />
              Create First User
            </Button>
          )}
          {hasFilters && onClearFilters && (
            <Button variant="outline" onClick={onClearFilters} className="flex items-center gap-2">
              Clear All Filters
            </Button>
          )}
        </div>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      {loading ? (
        <LoadingSkeleton rows={pageSize} />
      ) : (
        <>
          {/* Enhanced Bulk Actions */}
          {selectedUsers.length > 0 && (
            <div className="flex items-center justify-between p-4 bg-primary/5 border border-primary/20 rounded-lg border-l-4 border-l-primary">
              <div className="flex items-center gap-3">
                <div className="flex items-center justify-center w-8 h-8 bg-primary/10 rounded-full">
                  <span className="text-sm font-semibold text-primary">{selectedUsers.length}</span>
                </div>
                <span className="text-sm font-medium">
                  {selectedUsers.length} user{selectedUsers.length > 1 ? 's' : ''} selected
                </span>
              </div>
              <div className="flex gap-2">
                <Button size="sm" variant="outline" className="h-8">
                  <Edit className="h-3 w-3 mr-1" />
                  Edit
                </Button>
                <Button size="sm" variant="outline" className="h-8">
                  <UserCheck className="h-3 w-3 mr-1" />
                  Activate
                </Button>
                <Button size="sm" variant="outline" className="h-8">
                  <UserX className="h-3 w-3 mr-1" />
                  Deactivate
                </Button>
                <Button size="sm" variant="destructive" className="h-8">
                  <Trash2 className="h-3 w-3 mr-1" />
                  Delete
                </Button>
              </div>
            </div>
          )}

          {/* Enhanced Table */}
           <div className="border rounded-lg overflow-hidden bg-card shadow-sm">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/50">
                  <TableHead className="w-12">
                    <div className="flex items-center justify-center">
                      <input
                        type="checkbox"
                        checked={isAllSelected}
                        onChange={(e) => handleSelectAll(e.target.checked)}
                        className="rounded border border-input w-4 h-4 accent-primary"
                        aria-label="Select all users"
                      />
                    </div>
                  </TableHead>
                  <TableHead className="font-semibold">User</TableHead>
                  <TableHead className="font-semibold">Role</TableHead>
                  <TableHead className="font-semibold">Status</TableHead>
                  <TableHead className="font-semibold">Joined</TableHead>
                  <TableHead className="w-12 font-semibold">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map((user) => (
                  <UserTableRow
                    key={user.id}
                    user={user}
                    selectedUsers={selectedUsers}
                    onSelectUser={handleSelectUser}
                    onActivate={onActivateUser}
                    onDeactivate={onDeactivateUser}
                    onDelete={onDeleteUser}
                    onView={onViewUser}
                    onEdit={onEditUser}
                  />
                ))}
              </TableBody>
            </Table>
          </div>

          {/* Enhanced Pagination */}
          <div className="flex items-center justify-between py-4 px-2">
            <div className="text-sm text-muted-foreground bg-muted/30 px-3 py-1.5 rounded-md border">
              Showing <span className="font-medium text-foreground">{((currentPage - 1) * pageSize) + 1}</span> to{' '}
              <span className="font-medium text-foreground">{Math.min(currentPage * pageSize, totalCount)}</span> of{' '}
              <span className="font-medium text-foreground">{totalCount}</span> users
            </div>
            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => currentPage > 1 && onPageChange(currentPage - 1)}
                disabled={currentPage <= 1}
                className="h-9"
              >
                Previous
              </Button>
              
              <div className="flex items-center gap-1">
                <span className="text-sm text-muted-foreground">Page</span>
                <div className="bg-primary text-primary-foreground px-2 py-1 rounded text-sm font-medium min-w-8 text-center">
                  {currentPage}
                </div>
                <span className="text-sm text-muted-foreground">of {totalPages}</span>
              </div>
              
              <Button
                variant="outline"
                size="sm"
                onClick={() => currentPage < totalPages && onPageChange(currentPage + 1)}
                disabled={currentPage >= totalPages}
                className="h-9"
              >
                Next
              </Button>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
