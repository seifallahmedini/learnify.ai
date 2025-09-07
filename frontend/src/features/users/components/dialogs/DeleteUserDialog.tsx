import { useState, useEffect } from 'react'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog'
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar'
import { Badge } from '@/shared/components/ui/badge'
import { 
  Trash2,
  AlertTriangle,
  Loader2,
  User,
  Mail
} from 'lucide-react'
import { usersApi } from '../../services'
import type { User as UserType } from '../../types'
import { UserRoleBadge } from '../shared/UserRoleBadge'

interface DeleteUserDialogProps {
  user: UserType | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onUserDeleted: (deletedUserId: number) => void
}

export function DeleteUserDialog({ 
  user, 
  open, 
  onOpenChange, 
  onUserDeleted 
}: DeleteUserDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // Reset error state when dialog opens/closes or user changes
  useEffect(() => {
    if (!open || !user) {
      setError(null)
      setIsDeleting(false)
    }
  }, [open, user])

  const getUserInitials = (fullName: string) => {
    return fullName
      .split(' ')
      .map(name => name.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2)
  }

  const handleDelete = async () => {
    if (!user) return

    try {
      setIsDeleting(true)
      setError(null)

      await usersApi.deleteUser(user.id)
      
      onUserDeleted(user.id)
      onOpenChange(false)
    } catch (err) {
      console.error('Failed to delete user:', err)
      setError('Failed to delete user. Please try again.')
    } finally {
      setIsDeleting(false)
    }
  }

  const handleCancel = () => {
    setError(null)
    onOpenChange(false)
  }

  return (
    <AlertDialog open={open && !!user} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-md">
        {user && (
          <>
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center gap-2 text-red-600">
                <AlertTriangle className="h-5 w-5" />
                Delete User Account
              </AlertDialogTitle>
              <AlertDialogDescription className="space-y-4">
                <div className="text-sm text-muted-foreground">
                  Are you sure you want to permanently delete this user account? This action cannot be undone.
                </div>
                
                {/* User Preview */}
                <div className="flex items-center gap-3 p-3 bg-red-50 border border-red-200 rounded-lg">
                  <Avatar className="h-12 w-12">
                    <AvatarImage src={user.profilePicture} alt={user.fullName} />
                    <AvatarFallback className="text-sm font-medium bg-red-100 text-red-700">
                      {getUserInitials(user.fullName)}
                    </AvatarFallback>
                  </Avatar>
                  <div className="flex-1 space-y-1">
                    <h4 className="font-medium text-sm">{user.fullName}</h4>
                    <div className="flex items-center gap-2 text-xs text-muted-foreground">
                      <Mail className="h-3 w-3" />
                      {user.email}
                    </div>
                    <div className="flex items-center gap-2">
                      <UserRoleBadge role={user.role} />
                      <Badge 
                        variant={user.isActive ? 'default' : 'secondary'} 
                        className="text-xs"
                      >
                        ID: #{user.id}
                      </Badge>
                    </div>
                  </div>
                </div>

                {/* Warning Information */}
                <div className="space-y-2 text-sm">
                  <div className="font-medium text-red-700">This will permanently delete:</div>
                  <ul className="list-disc list-inside space-y-1 text-muted-foreground ml-2">
                    <li>User profile and personal information</li>
                    <li>Course enrollments and progress data</li>
                    <li>Quiz attempts and assessment results</li>
                    <li>Reviews and ratings submitted by the user</li>
                    <li>Any content created by the user</li>
                  </ul>
                </div>

                <div className="p-3 bg-amber-50 border border-amber-200 rounded-lg">
                  <div className="flex items-start gap-2">
                    <AlertTriangle className="h-4 w-4 text-amber-600 mt-0.5 flex-shrink-0" />
                    <div className="text-xs text-amber-800">
                      <div className="font-medium">Alternative Options:</div>
                      <div className="mt-1">
                        Consider <strong>deactivating</strong> the user account instead of deleting it. 
                        This preserves data while preventing access to the platform.
                      </div>
                    </div>
                  </div>
                </div>

                {error && (
                  <div className="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
                    {error}
                  </div>
                )}
              </AlertDialogDescription>
            </AlertDialogHeader>
            
            <AlertDialogFooter className="gap-2">
              <AlertDialogCancel 
                onClick={handleCancel}
                disabled={isDeleting}
                className="flex items-center gap-2"
              >
                <User className="h-4 w-4" />
                Keep User
              </AlertDialogCancel>
              <AlertDialogAction
                onClick={handleDelete}
                disabled={isDeleting}
                className="bg-red-600 hover:bg-red-700 focus:ring-red-600 flex items-center gap-2"
              >
                {isDeleting ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  <Trash2 className="h-4 w-4" />
                )}
                {isDeleting ? 'Deleting...' : 'Delete User'}
              </AlertDialogAction>
            </AlertDialogFooter>
          </>
        )}
      </AlertDialogContent>
    </AlertDialog>
  )
}
