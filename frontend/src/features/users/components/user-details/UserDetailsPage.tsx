import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { Button } from '@/shared/components/ui/button'
import { ArrowLeft, Loader2 } from 'lucide-react'
import { UserDetails } from './UserDetails'
import { UserLearningData } from './UserLearningData'
import { usersApi } from '../../services'
import type { User } from '../../types'

export function UserDetailsPage() {
  const { userId } = useParams<{ userId: string }>()
  const navigate = useNavigate()
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadUser = async () => {
      if (!userId) {
        setError('User ID is required')
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        
        // Try to load from API, fallback to mock data
        let userData: User
        try {
          const response = await usersApi.getUserById(parseInt(userId))
          userData = response
        } catch (apiError) {
          console.warn('API failed, using mock data:', apiError)
          // Fallback to mock data
          userData = {
            id: parseInt(userId),
            firstName: 'John',
            lastName: 'Doe',
            fullName: 'John Doe',
            email: 'john.doe@example.com',
            role: 1, // Student
            isActive: true,
            profilePicture: undefined,
            bio: 'This is a sample user profile. The API connection will be implemented later.',
            dateOfBirth: '1990-05-15',
            phoneNumber: '+1234567890',
            createdAt: '2023-01-15T10:00:00Z',
            updatedAt: '2024-01-10T15:30:00Z'
          }
        }
        
        setUser(userData)
      } catch (err) {
        console.error('Failed to load user:', err)
        setError('Failed to load user details')
      } finally {
        setLoading(false)
      }
    }

    loadUser()
  }, [userId])

  const handleEdit = () => {
    // Navigate to edit page or open edit dialog
    console.log('Edit user:', user?.id)
    // navigate(`/users/${user?.id}/edit`)
  }

  const handleActivate = async () => {
    if (!user) return
    
    try {
      await usersApi.activateUser(user.id)
      setUser({ ...user, isActive: true })
    } catch (error) {
      console.error('Failed to activate user:', error)
    }
  }

  const handleDeactivate = async () => {
    if (!user) return
    
    try {
      await usersApi.deactivateUser(user.id)
      setUser({ ...user, isActive: false })
    } catch (error) {
      console.error('Failed to deactivate user:', error)
    }
  }

  const handleDelete = async () => {
    if (!user) return
    
    const confirmed = confirm(`Are you sure you want to delete user "${user.fullName}"?`)
    if (!confirmed) return
    
    try {
      await usersApi.deleteUser(user.id)
      navigate('/users')
    } catch (error) {
      console.error('Failed to delete user:', error)
    }
  }

  const handleBack = () => {
    navigate('/users')
  }

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50/40 flex items-center justify-center">
        <div className="text-center space-y-4">
          <div className="flex items-center justify-center">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
          <div>
            <h3 className="text-lg font-medium">Loading user details...</h3>
            <p className="text-sm text-muted-foreground">Please wait while we fetch the information</p>
          </div>
        </div>
      </div>
    )
  }

  if (error || !user) {
    return (
      <div className="min-h-screen bg-gray-50/40 flex items-center justify-center">
        <div className="text-center space-y-6 max-w-md mx-auto px-4">
          <div className="space-y-2">
            <h3 className="text-xl font-semibold">User not found</h3>
            <p className="text-muted-foreground">{error || 'The requested user could not be found.'}</p>
          </div>
          <Button onClick={handleBack} variant="outline">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Users
          </Button>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gray-50/40">
      {/* Header Section with Better Spacing */}
      <div className="sticky top-0 z-10 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 border-b">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <Button variant="ghost" size="sm" onClick={handleBack} className="hover:bg-muted">
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to Users
              </Button>
              <div className="h-6 w-px bg-border" />
              <div>
                <h1 className="text-2xl font-bold tracking-tight">User Details</h1>
                <p className="text-sm text-muted-foreground">
                  View and manage {user.fullName}'s information and learning progress
                </p>
              </div>
            </div>
            
            {/* Quick Actions in Header */}
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" onClick={handleEdit}>
                Edit Profile
              </Button>
              {user.isActive ? (
                <Button variant="outline" size="sm" onClick={handleDeactivate}>
                  Deactivate
                </Button>
              ) : (
                <Button variant="default" size="sm" onClick={handleActivate}>
                  Activate
                </Button>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Main Content with Improved Layout */}
      <div className="container mx-auto px-4 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          {/* User Profile Section - Sidebar on large screens */}
          <div className="lg:col-span-4 xl:col-span-3">
            <div className="sticky top-24">
              <UserDetails
                user={user}
                onEdit={handleEdit}
                onActivate={handleActivate}
                onDeactivate={handleDeactivate}
                onDelete={handleDelete}
              />
            </div>
          </div>

          {/* Learning Data Section - Main content area */}
          <div className="lg:col-span-8 xl:col-span-9">
            <div className="space-y-6">
              {/* Section Header */}
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-xl font-semibold tracking-tight">Learning Progress</h2>
                  <p className="text-sm text-muted-foreground">
                    Track {user.fullName}'s educational journey and achievements
                  </p>
                </div>
              </div>

              {/* Learning Data Component */}
              <UserLearningData user={user} />
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
