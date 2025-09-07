import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/components/ui/card"
import { Button } from "@/shared/components/ui/button"
import {
  Table,
  TableBody,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table"
import { Users, UserPlus } from "lucide-react"
import type { UserSummary } from "../types"
import { UserTableRow } from "./UserTableRow"
import { LoadingSkeleton } from "./LoadingSkeleton"
import { Pagination } from "./Pagination"

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
  onPageSizeChange?: (pageSize: number) => void
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
  onPageSizeChange,
  onCreateUser,
  onClearFilters,
  hasFilters = false,
}: UserTableProps) {
  const EmptyState = () => (
    <div className="flex flex-col items-center justify-center py-16 px-4" role="status" aria-live="polite">
      <div className="rounded-full bg-primary/10 p-6 mb-6">
        <Users className="h-10 w-10 text-primary" />
      </div>
      <h3 className="text-xl font-semibold text-center mb-3">
        {hasFilters ? "No users match your filters" : "No users found"}
      </h3>
      <p className="text-sm text-muted-foreground text-center mb-8 max-w-md leading-relaxed">
        {hasFilters 
          ? "Try adjusting your search terms or filters to find what you're looking for. You can also clear all filters to see all users."
          : "Get started by creating your first user account. Users can be students, instructors, or administrators."
        }
      </p>
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
  )

  return (
    <Card>
      <CardHeader>
        <CardTitle>Users ({totalCount})</CardTitle>
        <CardDescription>
          Page {currentPage} of {totalPages}
        </CardDescription>
      </CardHeader>
      <CardContent>
        {loading ? (
          <LoadingSkeleton rows={pageSize} />
        ) : users.length === 0 ? (
          <EmptyState />
        ) : (
          <>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>User</TableHead>
                  <TableHead>Role</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Joined</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map((user) => (
                  <UserTableRow
                    key={user.id}
                    user={user}
                    onActivate={onActivateUser}
                    onDeactivate={onDeactivateUser}
                    onDelete={onDeleteUser}
                    onView={onViewUser}
                    onEdit={onEditUser}
                  />
                ))}
              </TableBody>
            </Table>

            {/* Enhanced Pagination */}
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              totalItems={totalCount}
              pageSize={pageSize}
              onPageChange={onPageChange}
              onPageSizeChange={onPageSizeChange}
              showPageSizeSelector={true}
              pageSizeOptions={[5, 10, 20, 50]}
            />
          </>
        )}
      </CardContent>
    </Card>
  )
}
