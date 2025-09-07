import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { Button } from "@/shared/components/ui/button"
import { UserPlus } from "lucide-react"
import { UserFilters } from "./UserFilters"
import { UserTable } from "./UserTable"
import { CreateUserDialog } from "../dialogs/CreateUserDialog"
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
    setCurrentPage,
    handleRoleChange,
    handleStatusChange,
    handlePageSizeChange,
    handleActivateUser,
    handleDeactivateUser,
    handleDeleteUser,
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

  return (
    <div className="flex flex-1 flex-col gap-4 p-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Users</h1>
          <p className="text-muted-foreground">
            Manage users, roles, and permissions
          </p>
        </div>
        <Button onClick={() => setShowCreateDialog(true)}>
          <UserPlus className="mr-2 h-4 w-4" />
          Add User
        </Button>
      </div>

      {/* Filters */}
      <UserFilters
        searchTerm={searchTerm}
        selectedRole={selectedRole}
        selectedStatus={selectedStatus}
        onSearchChange={setSearchTerm}
        onRoleChange={handleRoleChange}
        onStatusChange={handleStatusChange}
      />

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
        onDeleteUser={handleDeleteUser}
        onViewUser={handleViewUser}
        onEditUser={handleEditUser}
        onPageChange={setCurrentPage}
        onPageSizeChange={handlePageSizeChange}
        onCreateUser={() => setShowCreateDialog(true)}
        onClearFilters={clearFilters}
        hasFilters={searchTerm.trim() !== "" || selectedRole !== "all" || selectedStatus !== "all"}
      />

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