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
  onPageChange,
  onCreateUser,
  onClearFilters,
  hasFilters = false,
}: UserTableProps) {
  const handlePreviousPage = () => {
    onPageChange(Math.max(1, currentPage - 1))
  }

  const handleNextPage = () => {
    onPageChange(Math.min(totalPages, currentPage + 1))
  }

  const LoadingSkeleton = () => (
    <div className="space-y-3">
      {Array.from({ length: 5 }).map((_, index) => (
        <div key={index} className="flex items-center space-x-4 py-3 border-b">
          <div className="h-8 w-8 rounded-full bg-muted animate-pulse" />
          <div className="flex-1 space-y-2">
            <div className="h-4 w-32 bg-muted animate-pulse rounded" />
            <div className="h-3 w-48 bg-muted animate-pulse rounded" />
          </div>
          <div className="h-6 w-20 bg-muted animate-pulse rounded-full" />
          <div className="h-6 w-16 bg-muted animate-pulse rounded-full" />
          <div className="h-4 w-24 bg-muted animate-pulse rounded" />
          <div className="h-8 w-8 bg-muted animate-pulse rounded" />
        </div>
      ))}
    </div>
  )

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
          <LoadingSkeleton />
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
                  />
                ))}
              </TableBody>
            </Table>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="flex items-center justify-between pt-4">
                <div className="text-sm text-muted-foreground">
                  Showing {((currentPage - 1) * pageSize) + 1} to {Math.min(currentPage * pageSize, totalCount)} of {totalCount} users
                </div>
                <div className="flex items-center space-x-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={handlePreviousPage}
                    disabled={currentPage === 1}
                  >
                    Previous
                  </Button>
                  <span className="text-sm">
                    Page {currentPage} of {totalPages}
                  </span>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={handleNextPage}
                    disabled={currentPage === totalPages}
                  >
                    Next
                  </Button>
                </div>
              </div>
            )}
          </>
        )}
      </CardContent>
    </Card>
  )
}
