import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'
import { Button } from '@/shared/components/ui/button'
import { Separator } from '@/shared/components/ui/separator'
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar'
import { 
  User, 
  Mail, 
  Phone, 
  Calendar, 
  Shield, 
  Clock,
  Edit,
  MoreHorizontal,
  UserCheck,
  UserX,
  Trash2,
  Copy,
  CheckCircle
} from 'lucide-react'
import { 
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator
} from '@/shared/components/ui/dropdown-menu'
import { useState } from 'react'
import type { User as UserType } from '../types'
import { UserRoleBadge } from './UserRoleBadge'
import { UserStatusBadge } from './UserStatusBadge'

interface UserDetailsProps {
  user: UserType
  onEdit?: () => void
  onActivate?: () => void
  onDeactivate?: () => void
  onDelete?: () => void
  className?: string
}

export function UserDetails({ 
  user, 
  onEdit, 
  onActivate, 
  onDeactivate, 
  onDelete,
  className = '' 
}: UserDetailsProps) {
  const [copiedField, setCopiedField] = useState<string | null>(null)

  const getUserInitials = (fullName: string) => {
    return fullName
      .split(' ')
      .map(name => name.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2)
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
  }

  const getRelativeTime = (dateString: string) => {
    const date = new Date(dateString)
    const now = new Date()
    const diffInDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24))
    
    if (diffInDays === 0) return 'Today'
    if (diffInDays === 1) return 'Yesterday'
    if (diffInDays < 7) return `${diffInDays} days ago`
    if (diffInDays < 30) return `${Math.floor(diffInDays / 7)} weeks ago`
    if (diffInDays < 365) return `${Math.floor(diffInDays / 30)} months ago`
    return `${Math.floor(diffInDays / 365)} years ago`
  }

  const copyToClipboard = async (text: string, fieldName: string) => {
    try {
      await navigator.clipboard.writeText(text)
      setCopiedField(fieldName)
      setTimeout(() => setCopiedField(null), 2000)
    } catch (err) {
      console.error('Failed to copy:', err)
    }
  }

  const InfoItem = ({ 
    label, 
    value, 
    icon: Icon, 
    copyable = false, 
    className: itemClassName = '' 
  }: {
    label: string
    value: string | React.ReactNode
    icon?: any
    copyable?: boolean
    className?: string
  }) => (
    <div className={`space-y-2 ${itemClassName}`}>
      <label className="text-sm font-medium text-muted-foreground">
        {label}
      </label>
      <div className="flex items-center space-x-2">
        {Icon && <Icon className="h-4 w-4 text-muted-foreground" />}
        <div className="flex-1 text-sm font-medium">{value}</div>
        {copyable && typeof value === 'string' && (
          <Button
            variant="ghost"
            size="sm"
            className="h-6 w-6 p-0"
            onClick={() => copyToClipboard(value, label)}
          >
            {copiedField === label ? (
              <CheckCircle className="h-3 w-3 text-green-600" />
            ) : (
              <Copy className="h-3 w-3" />
            )}
          </Button>
        )}
      </div>
    </div>
  )

  return (
    <div className={`space-y-6 ${className}`}>
      {/* User Profile Header */}
      <Card>
        <CardHeader>
          <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-6">
            <div className="flex flex-col sm:flex-row items-start gap-6">
              <div className="relative">
                <Avatar className="h-20 w-20">
                  <AvatarImage src={user.profilePicture} alt={user.fullName} />
                  <AvatarFallback className="text-lg font-medium">
                    {getUserInitials(user.fullName)}
                  </AvatarFallback>
                </Avatar>
                <div className={`absolute -bottom-1 -right-1 h-5 w-5 rounded-full border-2 border-background ${
                  user.isActive ? 'bg-green-500' : 'bg-gray-400'
                }`} />
              </div>
              
              <div className="space-y-2 flex-1">
                <div className="space-y-1">
                  <h1 className="text-2xl font-bold">{user.fullName}</h1>
                  <div className="flex items-center gap-2 text-muted-foreground">
                    <Mail className="h-4 w-4" />
                    <span>{user.email}</span>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-6 w-6 p-0"
                      onClick={() => copyToClipboard(user.email, 'email')}
                    >
                      {copiedField === 'email' ? (
                        <CheckCircle className="h-3 w-3 text-green-600" />
                      ) : (
                        <Copy className="h-3 w-3" />
                      )}
                    </Button>
                  </div>
                </div>
                
                <div className="flex flex-wrap items-center gap-3">
                  <UserRoleBadge role={user.role} />
                  <UserStatusBadge isActive={user.isActive} />
                  <span className="text-sm text-muted-foreground">
                    Joined {getRelativeTime(user.createdAt)}
                  </span>
                </div>
              </div>
            </div>
            
            <div className="flex gap-2">
              {onEdit && (
                <Button variant="outline" size="sm" onClick={onEdit}>
                  <Edit className="h-4 w-4 mr-2" />
                  Edit
                </Button>
              )}
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="outline" size="sm" className="gap-2">
                    <MoreHorizontal className="h-4 w-4" />
                    More
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-48">
                  {user.isActive ? (
                    onDeactivate && (
                      <DropdownMenuItem onClick={onDeactivate} className="text-orange-600">
                        <UserX className="mr-2 h-4 w-4" />
                        Deactivate User
                      </DropdownMenuItem>
                    )
                  ) : (
                    onActivate && (
                      <DropdownMenuItem onClick={onActivate} className="text-green-600">
                        <UserCheck className="mr-2 h-4 w-4" />
                        Activate User
                      </DropdownMenuItem>
                    )
                  )}
                  <DropdownMenuSeparator />
                  {onDelete && (
                    <DropdownMenuItem onClick={onDelete} className="text-red-600">
                      <Trash2 className="mr-2 h-4 w-4" />
                      Delete User
                    </DropdownMenuItem>
                  )}
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </div>
        </CardHeader>
      </Card>

      {/* Personal Information */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5" />
              Personal Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <InfoItem 
                label="First Name" 
                value={user.firstName} 
                copyable 
              />
              <InfoItem 
                label="Last Name" 
                value={user.lastName} 
                copyable 
              />
            </div>
            
            <InfoItem 
              label="Email Address" 
              value={user.email} 
              icon={Mail}
              copyable 
            />
            
            {user.phoneNumber && (
              <InfoItem 
                label="Phone Number" 
                value={user.phoneNumber} 
                icon={Phone}
                copyable 
              />
            )}
            
            {user.dateOfBirth && (
              <InfoItem 
                label="Date of Birth" 
                value={formatDate(user.dateOfBirth)} 
                icon={Calendar}
              />
            )}
            
            {user.bio && (
              <>
                <Separator />
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground">
                    Bio
                  </label>
                  <p className="text-sm p-3 rounded-md bg-muted/50">
                    {user.bio}
                  </p>
                </div>
              </>
            )}
          </CardContent>
        </Card>

        {/* Account Information */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Shield className="h-5 w-5" />
              Account Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <InfoItem 
                label="User ID" 
                value={`#${user.id}`} 
                copyable 
              />
              <InfoItem 
                label="Role" 
                value={<UserRoleBadge role={user.role} showIcon={false} />}
              />
            </div>
            
            <InfoItem 
              label="Account Status" 
              value={<UserStatusBadge isActive={user.isActive} showIcon={false} />}
            />
            
            <Separator />
            
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <InfoItem 
                label="Member Since" 
                value={formatDate(user.createdAt)}
                icon={Clock}
              />
              <InfoItem 
                label="Last Updated" 
                value={formatDate(user.updatedAt)}
                icon={Clock}
              />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <Card>
        <CardHeader>
          <CardTitle>Quick Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-3">
            {onEdit && (
              <Button 
                variant="outline" 
                onClick={onEdit}
              >
                <Edit className="h-4 w-4 mr-2" />
                Edit Profile
              </Button>
            )}
            {user.isActive ? (
              onDeactivate && (
                <Button 
                  variant="outline" 
                  onClick={onDeactivate}
                >
                  <UserX className="h-4 w-4 mr-2" />
                  Deactivate User
                </Button>
              )
            ) : (
              onActivate && (
                <Button 
                  variant="outline" 
                  onClick={onActivate}
                >
                  <UserCheck className="h-4 w-4 mr-2" />
                  Activate User
                </Button>
              )
            )}
            {onDelete && (
              <Button 
                variant="outline" 
                onClick={onDelete}
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Delete User
              </Button>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
