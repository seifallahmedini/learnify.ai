import { useState } from "react"
import type { CreateUserRequest } from "../types"
import { UserRole } from "../types"

export function useCreateUserForm() {
  const [formData, setFormData] = useState<CreateUserRequest>({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: UserRole.Student,
    bio: '',
  })

  const handleFormChange = (field: keyof CreateUserRequest, value: string) => {
    setFormData(prev => ({
      ...prev,
      [field]: field === 'role' ? (parseInt(value) as unknown as UserRole) : value
    }))
  }

  const resetForm = () => {
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      role: UserRole.Student,
      bio: '',
    })
  }

  const isFormValid = () => {
    return formData.firstName.trim() !== '' &&
           formData.lastName.trim() !== '' &&
           formData.email.trim() !== '' &&
           formData.password.trim() !== '' &&
           formData.password.length >= 6
  }

  return {
    formData,
    handleFormChange,
    resetForm,
    isFormValid,
  }
}
