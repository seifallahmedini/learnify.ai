import { useState } from "react"
import { Button } from "@/shared/components/ui/button"
import { UserPlus } from "lucide-react"
import { UserFilters } from "./UserFilters"
import { UserTable } from "./UserTable"
import { CreateUserDialog } from "./CreateUserDialog"
import { useUserManagement } from "../hooks/useUserManagement"
import { useCreateUserForm } from "../hooks/useCreateUserForm"

export function UsersListPage() {
  const [showCreateDialog, setShowCreateDialog] = useState(false)
  const [isCreating, setIsCreating] = useState(false)

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
    handleActivateUser,
    handleDeactivateUser,
    handleDeleteUser,
    handleCreateUser,
    clearFilters,
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
        onPageChange={setCurrentPage}
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
    </div>
  )
}