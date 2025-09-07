import { UserDetails } from '../components/UserDetails'
import type { User } from '../types'
import { UserRole } from '../types'

// Example of how to use the UserDetails component with mock data
export function UserDetailsExample() {
  const mockUser: User = {
    id: 1,
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    email: 'john.doe@example.com',
    role: UserRole.Student,
    isActive: true,
    profilePicture: undefined,
    bio: 'Passionate learner interested in technology and continuous improvement. Always eager to explore new concepts and apply them in real-world scenarios.',
    dateOfBirth: '1990-05-15',
    phoneNumber: '+1 (555) 123-4567',
    createdAt: '2023-01-15T10:00:00Z',
    updatedAt: '2024-01-10T15:30:00Z'
  }

  const handleEdit = () => {
    console.log('Edit user:', mockUser.id)
    // Implement edit functionality
  }

  const handleActivate = () => {
    console.log('Activate user:', mockUser.id)
    // Implement activate functionality
  }

  const handleDeactivate = () => {
    console.log('Deactivate user:', mockUser.id)
    // Implement deactivate functionality
  }

  const handleDelete = () => {
    console.log('Delete user:', mockUser.id)
    // Implement delete functionality
  }

  return (
    <div className="p-4 max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold mb-6">UserDetails Component Example</h1>
      
      <UserDetails
        user={mockUser}
        onEdit={handleEdit}
        onActivate={handleActivate}
        onDeactivate={handleDeactivate}
        onDelete={handleDelete}
      />
    </div>
  )
}
