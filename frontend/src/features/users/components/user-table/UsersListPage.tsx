import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { Button } from "@/shared/components/ui/button"
import { Input } from "@/shared/components/ui/input"
import { Search } from "lucide-react"
import { UserPlus } from "lucide-react"
import { UserFilters } from "./UserFilters"
import { UserTable } from "./UserTable"
import { CreateUserDialog } from "../dialogs/CreateUserDialog"
import { DeleteUserDialog } from "../dialogs/DeleteUserDialog"
import { EditUser } from "../shared/EditUser"
import { useUserManagement } from "../../hooks/useUserManagement"
import { useCreateUserForm } from "../../hooks/useCreateUserForm"
import { usersApi } from "../../services"
import type { User } from "../../types"
 

export function UsersListPage() {
  const navigate = useNavigate()
  const [showCreateDialog, setShowCreateDialog] = useState(false)
  const [isCreating, setIsCreating] = useState(false)
  
  // Edit user state
  const [showEditDialog, setShowEditDialog] = useState(false)
  const [selectedUser, setSelectedUser] = useState<User | null>(null)
  const [isLoadingUser, setIsLoadingUser] = useState(false)
  
  // Delete user state
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)
  const [userToDelete, setUserToDelete] = useState<User | null>(null)

  const {
    users,
    loading,
    searchTerm,
    selectedRole,
    selectedStatus,
    currentPage,
    totalPages,
    totalCount,
    pageSize,
    setSearchTerm,
    handleRoleChange,
    handleStatusChange,
    handleActivateUser,
    handleDeactivateUser,
    handleCreateUser,
    clearFilters,
    loadUsers,
  } = useUserManagement()

  const {
    formData,
    handleFormChange,
    resetForm,
    isFormValid,
  } = useCreateUserForm()

  const handleCreateUserSubmit = async () => {
    if (!isFormValid()) {
      return
    }

    setIsCreating(true)
    const success = await handleCreateUser(formData)
    
    if (success) {
      setShowCreateDialog(false)
      resetForm()
    }
    
    setIsCreating(false)
  }

  const handleCreateDialogClose = (open: boolean) => {
    setShowCreateDialog(open)
    if (!open) {
      resetForm()
    }
  }

  const handleViewUser = (userId: number) => {
    navigate(`/users/${userId}`)
  }

  const handleEditUser = async (userId: number) => {
    try {
      setIsLoadingUser(true)
      const user = await usersApi.getUserById(userId)
      if (user) {
        setSelectedUser(user)
        setShowEditDialog(true)
      }
    } catch (error) {
      console.error('Failed to load user for editing:', error)
    } finally {
      setIsLoadingUser(false)
    }
  }

  const handleUserUpdated = async (_updatedUser: User) => {
    // Refresh the users list to show updated data
    await loadUsers()
  }

  const handleEditDialogClose = (open: boolean) => {
    setShowEditDialog(open)
    if (!open) {
      setSelectedUser(null)
    }
  }

  const handleDeleteUserClick = async (userId: number) => {
    try {
      setIsLoadingUser(true)
      const user = await usersApi.getUserById(userId)
      if (user) {
        setUserToDelete(user)
        setShowDeleteDialog(true)
      }
    } catch (error) {
      console.error('Failed to load user for deletion:', error)
    } finally {
      setIsLoadingUser(false)
    }
  }

  const handleUserDeleted = async (_deletedUserId: number) => {
    // Refresh the users list after deletion
    await loadUsers()
  }

  const handleDeleteDialogClose = (open: boolean) => {
    setShowDeleteDialog(open)
    if (!open) {
      setUserToDelete(null)
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row gap-4 sm:items-center sm:justify-between">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Users</h1>
          <p className="text-muted-foreground">
            Manage users, roles, and permissions • {loading ? 'loading…' : `${totalCount} users`}
          </p>
        </div>
        <div className="flex items-center gap-3">
          <Button onClick={() => setShowCreateDialog(true)} className="gap-2">
            <UserPlus className="h-4 w-4" />
            Add User
          </Button>
        </div>
      </div>

      {/* Search and Filters - Inline Design */}
      <div className="flex flex-col gap-3">
        <div className="flex flex-col sm:flex-row gap-3 items-start sm:items-center">
          {/* Search Field */}
          <div className="relative flex-1 w-full">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground pointer-events-none" />
            <Input
              placeholder="Search users by name or email..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 pr-4 h-9 w-full"
            />
          </div>

          {/* Filters - Role and Status on the right */}
          <UserFilters
            selectedRole={selectedRole}
            selectedStatus={selectedStatus}
            onRoleChange={handleRoleChange}
            onStatusChange={handleStatusChange}
          />
        </div>
      </div>

      {/* Results Summary */}
      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>
          Showing {users.length} of {totalCount} users
          {searchTerm && (
            <span className="ml-1">
              for "<span className="text-foreground font-medium">{searchTerm}</span>"
            </span>
          )}
        </span>
      </div>

      {/* Users Table */}
      <UserTable
        users={users}
        loading={loading}
        totalCount={totalCount}
        currentPage={currentPage}
        totalPages={totalPages}
        pageSize={pageSize}
        onActivateUser={handleActivateUser}
        onDeactivateUser={handleDeactivateUser}
        onDeleteUser={handleDeleteUserClick}
        onViewUser={handleViewUser}
        onEditUser={handleEditUser}
        onCreateUser={() => setShowCreateDialog(true)}
        onClearFilters={clearFilters}
        hasFilters={searchTerm.trim() !== "" || selectedRole !== "all" || selectedStatus !== "all"}
      />

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-center space-x-4 py-4">
          <div className="flex items-center space-x-2 text-sm text-muted-foreground">
            <span>Page {currentPage} of {totalPages}</span>
          </div>
        </div>
      )}

      {/* Create User Dialog */}
      <CreateUserDialog
        open={showCreateDialog}
        onOpenChange={handleCreateDialogClose}
        formData={formData}
        onFormChange={handleFormChange}
        onSubmit={handleCreateUserSubmit}
        isLoading={isCreating}
      />

      {/* Edit User Dialog */}
      {selectedUser && (
        <EditUser
          user={selectedUser}
          open={showEditDialog}
          onOpenChange={handleEditDialogClose}
          onUserUpdated={handleUserUpdated}
        />
      )}

      {/* Delete User Dialog */
      }
      <DeleteUserDialog
        user={userToDelete}
        open={showDeleteDialog}
        onOpenChange={handleDeleteDialogClose}
        onUserDeleted={handleUserDeleted}
      />
      
      {/* Loading state for edit user */}
      {isLoadingUser && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-4 rounded-lg">
            Loading user data...
          </div>
        </div>
      )}
    </div>
  )
}