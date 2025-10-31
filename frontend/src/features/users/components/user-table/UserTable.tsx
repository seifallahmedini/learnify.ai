import { useState } from 'react';
import { Card, CardContent } from "@/shared/components/ui/card"
import { Button } from "@/shared/components/ui/button"
import { Checkbox } from "@/shared/components/ui/checkbox"
import { 
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table"
import { 
  Users, 
  UserPlus
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

  const isAllSelected = selectedUsers.length === users.length && users.length > 0;
  const isIndeterminate = selectedUsers.length > 0 && selectedUsers.length < users.length;

  if (users.length === 0 && !loading) {
    return (
      <Card className="border-dashed border-2 py-16">
        <CardContent className="text-center space-y-6">
          <div className="mx-auto h-16 w-16 rounded-full bg-muted flex items-center justify-center">
            <Users className="h-8 w-8 text-muted-foreground" />
          </div>
          <div className="space-y-2">
            <h3 className="text-xl font-semibold">
              {hasFilters ? "No users match your filters" : "No users yet"}
            </h3>
            <p className="text-muted-foreground max-w-md mx-auto">
              {hasFilters 
                ? "No users match your filters. Try adjusting filters or clear them to see all users."
                : "Get started by creating your first user to manage your community."
              }
            </p>
          </div>
          <div className="flex flex-col sm:flex-row gap-2 justify-center">
            {hasFilters && onClearFilters ? (
              <Button variant="outline" onClick={onClearFilters}>Clear Filters</Button>
            ) : null}
            {!hasFilters && onCreateUser ? (
              <Button onClick={onCreateUser} className="gap-2">
                <UserPlus className="h-4 w-4" />
                Create Your First User
              </Button>
            ) : null}
          </div>
        </CardContent>
      </Card>
    );
  }

  if (loading) {
    return <LoadingSkeleton rows={pageSize} />;
  }

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="w-[50px]">
              <Checkbox
                checked={isAllSelected}
                onCheckedChange={handleSelectAll}
                aria-label="Select all users"
                className="data-[state=indeterminate]:bg-primary data-[state=indeterminate]:text-primary-foreground"
                {...(isIndeterminate && { 'data-state': 'indeterminate' })}
              />
            </TableHead>
            <TableHead className="w-[300px]">User</TableHead>
            <TableHead>Role</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Joined</TableHead>
            <TableHead className="w-[70px]"></TableHead>
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
  )
}
