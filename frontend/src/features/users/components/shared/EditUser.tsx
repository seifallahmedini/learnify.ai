import { useState } from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'
import { Input } from '@/shared/components/ui/input'
import { Textarea } from '@/shared/components/ui/textarea'
import { Button } from '@/shared/components/ui/button'
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar'
import { Badge } from '@/shared/components/ui/badge'
import { Label } from '@/shared/components/ui/label'
import { Switch } from '@/shared/components/ui/switch'
import { Separator } from '@/shared/components/ui/separator'
import { 
  User,
  Mail,
  Phone,
  Calendar,
  Loader2,
  Save,
  X
} from 'lucide-react'
import { usersApi } from '../../services'
import type { User as UserType, UpdateUserRequest } from '../../types'
import { UserRoleBadge } from './UserRoleBadge'

interface EditUserProps {
  user: UserType
  open: boolean
  onOpenChange: (open: boolean) => void
  onUserUpdated: (updatedUser: UserType) => void
}

export function EditUser({ user, open, onOpenChange, onUserUpdated }: EditUserProps) {
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  
  // Form state
  const [firstName, setFirstName] = useState(user.firstName)
  const [lastName, setLastName] = useState(user.lastName)
  const [bio, setBio] = useState(user.bio || '')
  const [dateOfBirth, setDateOfBirth] = useState(user.dateOfBirth || '')
  const [phoneNumber, setPhoneNumber] = useState(user.phoneNumber || '')
  const [isActive, setIsActive] = useState(user.isActive)

  const getUserInitials = (fullName: string) => {
    return fullName
      .split(' ')
      .map(name => name.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2)
  }

  const validateForm = () => {
    if (!firstName.trim() || firstName.length < 2) {
      setError('First name must be at least 2 characters')
      return false
    }
    if (!lastName.trim() || lastName.length < 2) {
      setError('Last name must be at least 2 characters')  
      return false
    }
    if (bio.length > 500) {
      setError('Bio must be less than 500 characters')
      return false
    }
    if (phoneNumber && !/^[\+]?[1-9][\d]{0,15}$/.test(phoneNumber.replace(/[\s\-\(\)]/g, ''))) {
      setError('Please enter a valid phone number')
      return false
    }
    return true
  }

  const onSubmit = async () => {
    if (!validateForm()) {
      return
    }

    try {
      setIsLoading(true)
      setError(null)

      const updateData: UpdateUserRequest = {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        bio: bio.trim() || undefined,
        dateOfBirth: dateOfBirth || undefined,
        phoneNumber: phoneNumber.trim() || undefined,
        isActive
      }

      await usersApi.updateUser(user.id, updateData)
      
      // Update the user object with new data
      const updatedUser: UserType = {
        ...user,
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        fullName: `${firstName.trim()} ${lastName.trim()}`,
        bio: bio.trim() || undefined,
        dateOfBirth: dateOfBirth || undefined,
        phoneNumber: phoneNumber.trim() || undefined,
        isActive,
        updatedAt: new Date().toISOString()
      }

      onUserUpdated(updatedUser)
      onOpenChange(false)
    } catch (err) {
      console.error('Failed to update user:', err)
      setError('Failed to update user. Please try again.')
    } finally {
      setIsLoading(false)
    }
  }

  const handleCancel = () => {
    // Reset form to original values
    setFirstName(user.firstName)
    setLastName(user.lastName)
    setBio(user.bio || '')
    setDateOfBirth(user.dateOfBirth || '')
    setPhoneNumber(user.phoneNumber || '')
    setIsActive(user.isActive)
    setError(null)
    onOpenChange(false)
  }

  const formatDateForInput = (dateString: string) => {
    if (!dateString) return ''
    try {
      const date = new Date(dateString)
      return date.toISOString().split('T')[0]
    } catch {
      return ''
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <User className="h-5 w-5" />
            Edit User Profile
          </DialogTitle>
          <DialogDescription>
            Update user information and account settings.
          </DialogDescription>
        </DialogHeader>

        {/* User Preview */}
        <div className="flex items-center gap-4 p-4 bg-muted/30 rounded-lg">
          <Avatar className="h-16 w-16">
            <AvatarImage src={user.profilePicture} alt={user.fullName} />
            <AvatarFallback className="text-lg font-medium">
              {getUserInitials(user.fullName)}
            </AvatarFallback>
          </Avatar>
          <div className="space-y-1">
            <h3 className="font-semibold">{user.fullName}</h3>
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Mail className="h-3 w-3" />
              {user.email}
            </div>
            <div className="flex items-center gap-2">
              <UserRoleBadge role={user.role} />
              <Badge variant={user.isActive ? 'default' : 'secondary'} className="text-xs">
                ID: #{user.id}
              </Badge>
            </div>
          </div>
        </div>

        <Separator />

        {error && (
          <div className="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
            {error}
          </div>
        )}

        <div className="space-y-6">
          {/* Basic Information */}
          <div className="space-y-4">
            <h4 className="font-medium flex items-center gap-2">
              <User className="h-4 w-4" />
              Basic Information
            </h4>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="firstName">First Name *</Label>
                <Input
                  id="firstName"
                  placeholder="Enter first name"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                />
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="lastName">Last Name *</Label>
                <Input
                  id="lastName"
                  placeholder="Enter last name"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="bio">Biography</Label>
              <Textarea 
                id="bio"
                placeholder="Tell us about this user..."
                className="min-h-[100px]"
                value={bio}
                onChange={(e) => setBio(e.target.value)}
              />
              <p className="text-sm text-muted-foreground">
                Brief description about the user (max 500 characters)
              </p>
            </div>
          </div>

          <Separator />

          {/* Contact Information */}
          <div className="space-y-4">
            <h4 className="font-medium flex items-center gap-2">
              <Phone className="h-4 w-4" />
              Contact Information
            </h4>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="phoneNumber">Phone Number</Label>
                <Input 
                  id="phoneNumber"
                  placeholder="+1 (555) 123-4567"
                  type="tel"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                />
                <p className="text-sm text-muted-foreground">
                  Include country code if international
                </p>
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="dateOfBirth">Date of Birth</Label>
                <Input 
                  id="dateOfBirth"
                  type="date"
                  value={formatDateForInput(dateOfBirth)}
                  onChange={(e) => setDateOfBirth(e.target.value)}
                />
              </div>
            </div>
          </div>

          <Separator />

          {/* Account Settings */}
          <div className="space-y-4">
            <h4 className="font-medium flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              Account Settings
            </h4>
            
            <div className="flex flex-row items-center justify-between rounded-lg border p-4">
              <div className="space-y-0.5">
                <Label className="text-base">
                  Account Status
                </Label>
                <p className="text-sm text-muted-foreground">
                  When disabled, the user cannot access the platform
                </p>
              </div>
              <div className="flex items-center space-x-2">
                <Switch
                  id="isActive"
                  checked={isActive}
                  onCheckedChange={setIsActive}
                />
                <Label htmlFor="isActive" className="text-sm">
                  {isActive ? 'Active' : 'Inactive'}
                </Label>
              </div>
            </div>
          </div>

          {/* Read-only Information */}
          <div className="space-y-4">
            <h4 className="font-medium text-muted-foreground">Read-only Information</h4>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
              <div className="space-y-1">
                <label className="text-muted-foreground">Email Address</label>
                <div className="flex items-center gap-2 p-2 bg-muted/30 rounded">
                  <Mail className="h-3 w-3" />
                  {user.email}
                </div>
              </div>
              <div className="space-y-1">
                <label className="text-muted-foreground">User Role</label>
                <div className="p-2 bg-muted/30 rounded">
                  <UserRoleBadge role={user.role} />
                </div>
              </div>
              <div className="space-y-1">
                <label className="text-muted-foreground">Member Since</label>
                <div className="p-2 bg-muted/30 rounded">
                  {new Date(user.createdAt).toLocaleDateString()}
                </div>
              </div>
              <div className="space-y-1">
                <label className="text-muted-foreground">Last Updated</label>
                <div className="p-2 bg-muted/30 rounded">
                  {new Date(user.updatedAt).toLocaleDateString()}
                </div>
              </div>
            </div>
          </div>
        </div>

        <DialogFooter className="gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={handleCancel}
            disabled={isLoading}
          >
            <X className="h-4 w-4 mr-2" />
            Cancel
          </Button>
          <Button onClick={onSubmit} disabled={isLoading}>
            {isLoading ? (
              <Loader2 className="h-4 w-4 mr-2 animate-spin" />
            ) : (
              <Save className="h-4 w-4 mr-2" />
            )}
            {isLoading ? 'Saving...' : 'Save Changes'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
