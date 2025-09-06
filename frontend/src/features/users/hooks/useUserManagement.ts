import { useState, useEffect } from "react"
import type { UserSummary, UserListResponse, UserFilterRequest, CreateUserRequest, UserRole } from "../types"
import { usersApi } from "../services"

export function useUserManagement() {
  const [users, setUsers] = useState<UserSummary[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const [selectedRole, setSelectedRole] = useState<UserRole | "all">("all")
  const [selectedStatus, setSelectedStatus] = useState<"all" | "active" | "inactive">("all")
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [totalCount, setTotalCount] = useState(0)

  const pageSize = 10

  const loadUsers = async () => {
    try {
      setLoading(true)
      const filters: UserFilterRequest = {
        page: currentPage,
        pageSize,
      }

      if (searchTerm) filters.searchTerm = searchTerm
      if (selectedRole !== "all") filters.role = selectedRole as UserRole
      if (selectedStatus !== "all") filters.isActive = selectedStatus === "active"

      const response: UserListResponse = await usersApi.getUsers(filters)
      setUsers(response.users)
      setTotalCount(response.totalCount)
      setTotalPages(response.totalPages)
    } catch (error) {
      console.error("Failed to load users:", error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadUsers()
  }, [currentPage, selectedRole, selectedStatus])

  useEffect(() => {
    const debounceTimer = setTimeout(() => {
      if (currentPage === 1) {
        loadUsers()
      } else {
        setCurrentPage(1)
      }
    }, 500)

    return () => clearTimeout(debounceTimer)
  }, [searchTerm])

  const handleRoleChange = (role: string) => {
    if (role === "all") {
      setSelectedRole("all")
    } else {
      setSelectedRole(parseInt(role) as unknown as UserRole)
    }
    setCurrentPage(1)
  }

  const handleStatusChange = (status: string) => {
    setSelectedStatus(status as "all" | "active" | "inactive")
    setCurrentPage(1)
  }

  const handleActivateUser = async (userId: number) => {
    try {
      await usersApi.activateUser(userId)
      loadUsers()
    } catch (error) {
      console.error("Failed to activate user:", error)
    }
  }

  const handleDeactivateUser = async (userId: number) => {
    try {
      await usersApi.deactivateUser(userId)
      loadUsers()
    } catch (error) {
      console.error("Failed to deactivate user:", error)
    }
  }

  const handleDeleteUser = async (userId: number) => {
    if (confirm("Are you sure you want to delete this user?")) {
      try {
        await usersApi.deleteUser(userId)
        loadUsers()
      } catch (error) {
        console.error("Failed to delete user:", error)
      }
    }
  }

  const handleCreateUser = async (formData: CreateUserRequest) => {
    try {
      await usersApi.createUser(formData)
      loadUsers()
      return true
    } catch (error) {
      console.error('Error creating user:', error)
      return false
    }
  }

  const clearFilters = () => {
    setSearchTerm("")
    setSelectedRole("all")
    setSelectedStatus("all")
    setCurrentPage(1)
  }

  return {
    // State
    users,
    loading,
    searchTerm,
    selectedRole,
    selectedStatus,
    currentPage,
    totalPages,
    totalCount,
    pageSize,
    
    // Actions
    setSearchTerm,
    setCurrentPage,
    handleRoleChange,
    handleStatusChange,
    handleActivateUser,
    handleDeactivateUser,
    handleDeleteUser,
    handleCreateUser,
    clearFilters,
    loadUsers,
  }
}
