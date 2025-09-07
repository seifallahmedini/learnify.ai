import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'
import { Button } from '@/shared/components/ui/button'
import { Separator } from '@/shared/components/ui/separator'
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar'
import { 
  User, 
  Mail, 
  Phone, 
  Calendar, 
  Clock,
  Edit,
  UserCheck,
  UserX,
  Trash2,
  Copy,
  CheckCircle
} from 'lucide-react'
import { useState } from 'react'
import type { User as UserType } from '../../types'
import { UserRoleBadge } from '../shared/UserRoleBadge'
import { UserStatusBadge } from '../shared/UserStatusBadge'

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
    <div className={`space-y-4 ${className}`}>
      {/* Compact User Profile Header */}
      <Card className="border-l-4 border-l-primary">
        <CardContent className="p-6">
          <div className="flex flex-col items-center text-center space-y-4">
            <div className="relative">
              <Avatar className="h-20 w-20">
                <AvatarImage src={user.profilePicture} alt={user.fullName} />
                <AvatarFallback className="text-lg font-medium">
                  {getUserInitials(user.fullName)}
                </AvatarFallback>
              </Avatar>
              <div className={`absolute -bottom-1 -right-1 h-6 w-6 rounded-full border-2 border-background flex items-center justify-center ${
                user.isActive ? 'bg-green-500' : 'bg-gray-400'
              }`}>
                {user.isActive ? (
                  <UserCheck className="h-3 w-3 text-white" />
                ) : (
                  <UserX className="h-3 w-3 text-white" />
                )}
              </div>
            </div>
            
            <div className="space-y-2 w-full">
              <h2 className="text-xl font-bold">{user.fullName}</h2>
              <div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
                <Mail className="h-3 w-3" />
                <span className="truncate">{user.email}</span>
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-5 w-5 p-0"
                  onClick={() => copyToClipboard(user.email, 'email')}
                >
                  {copiedField === 'email' ? (
                    <CheckCircle className="h-3 w-3 text-green-600" />
                  ) : (
                    <Copy className="h-3 w-3" />
                  )}
                </Button>
              </div>
              
              <div className="flex flex-col items-center gap-2">
                <div className="flex items-center gap-2">
                  <UserRoleBadge role={user.role} />
                  <UserStatusBadge isActive={user.isActive} />
                </div>
                <span className="text-xs text-muted-foreground">
                  Joined {getRelativeTime(user.createdAt)}
                </span>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Essential Information */}
      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="flex items-center gap-2 text-base">
            <User className="h-4 w-4" />
            Essential Info
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="space-y-3">
            <InfoItem 
              label="User ID" 
              value={`#${user.id}`} 
              copyable 
            />
            
            {user.phoneNumber && (
              <InfoItem 
                label="Phone" 
                value={user.phoneNumber} 
                icon={Phone}
                copyable 
              />
            )}
            
            {user.dateOfBirth && (
              <InfoItem 
                label="Birth Date" 
                value={formatDate(user.dateOfBirth)} 
                icon={Calendar}
              />
            )}
            
            <InfoItem 
              label="Member Since" 
              value={formatDate(user.createdAt)}
              icon={Clock}
            />
          </div>
        </CardContent>
      </Card>

      {/* Biography */}
      {user.bio && (
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-base">About</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-muted-foreground leading-relaxed">
              {user.bio}
            </p>
          </CardContent>
        </Card>
      )}

      {/* Quick Actions - Compact */}
      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            {onEdit && (
              <Button 
                variant="outline" 
                onClick={onEdit}
                className="w-full justify-start"
                size="sm"
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
                  className="w-full justify-start text-orange-600 hover:text-orange-700"
                  size="sm"
                >
                  <UserX className="h-4 w-4 mr-2" />
                  Deactivate
                </Button>
              )
            ) : (
              onActivate && (
                <Button 
                  variant="outline" 
                  onClick={onActivate}
                  className="w-full justify-start text-green-600 hover:text-green-700"
                  size="sm"
                >
                  <UserCheck className="h-4 w-4 mr-2" />
                  Activate
                </Button>
              )
            )}

            {onDelete && (
              <>
                <Separator className="my-2" />
                <Button 
                  variant="outline" 
                  onClick={onDelete}
                  className="w-full justify-start text-red-600 hover:text-red-700"
                  size="sm"
                >
                  <Trash2 className="h-4 w-4 mr-2" />
                  Delete User
                </Button>
              </>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
