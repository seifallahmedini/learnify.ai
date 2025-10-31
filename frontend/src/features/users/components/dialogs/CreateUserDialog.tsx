import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/shared/components/ui/dialog"
import { Button } from "@/shared/components/ui/button"
import { Input } from "@/shared/components/ui/input"
import { Label } from "@/shared/components/ui/label"
import { Textarea } from "@/shared/components/ui/textarea"
import { Badge } from "@/shared/components/ui/badge"
import { Separator } from "@/shared/components/ui/separator"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select"
import { 
  User, 
  Mail, 
  Lock, 
  Shield,
  FileText,
  X,
  Save,
  Sparkles
} from "lucide-react"
import type { CreateUserRequest } from "../../types"

interface CreateUserDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  formData: CreateUserRequest
  onFormChange: (field: keyof CreateUserRequest, value: string) => void
  onSubmit: () => void
  isLoading?: boolean
}

export function CreateUserDialog({
  open,
  onOpenChange,
  formData,
  onFormChange,
  onSubmit,
  isLoading = false,
}: CreateUserDialogProps) {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit()
  }

  const getRoleLabel = (role: string) => {
    switch (role) {
      case "1": return "Student"
      case "2": return "Instructor"
      case "3": return "Admin"
      default: return "Student"
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="w-[90vw] max-w-[900px] min-w-[600px] max-h-[95vh] overflow-hidden flex flex-col">
        <DialogHeader className="pb-6 border-b border-border/50">
          <div className="flex items-center space-x-3">
            <div className="h-10 w-10 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-md">
              <Sparkles className="h-5 w-5 text-white" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold tracking-tight">
                Create New User
              </DialogTitle>
              <DialogDescription className="text-sm text-muted-foreground mt-1">
                Add a new member to your learning community
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto px-2 py-2">
          <div className="max-w-none mx-auto">
            <form onSubmit={handleSubmit} className="space-y-6">
              {/* Personal Information Section */}
              <div className="space-y-4 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-blue-500/5 to-transparent rounded-xl -z-10" />
                  <div className="flex items-center space-x-3 p-4 rounded-xl border border-blue-500/20 bg-gradient-to-r from-blue-50/50 to-transparent">
                    <div className="h-9 w-9 rounded-xl bg-gradient-to-br from-blue-100 to-blue-50 flex items-center justify-center border border-blue-200 shadow-sm">
                      <User className="h-4 w-4 text-blue-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-base font-bold text-foreground flex items-center space-x-2">
                        <span>Personal Information</span>
                        <div className="h-0.5 w-8 bg-gradient-to-r from-blue-500 to-blue-300 rounded-full" />
                      </h3>
                      <p className="text-xs text-muted-foreground mt-0.5">Essential user details</p>
                    </div>
                    <Badge variant="secondary" className="bg-blue-100 text-blue-700 border-blue-200 px-2 py-0.5 text-xs">Required</Badge>
                  </div>
                </div>
                
                <div className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label className="text-sm font-semibold text-foreground flex items-center space-x-1.5">
                        <User className="h-3.5 w-3.5 text-primary" />
                        <span>First Name</span>
                        <span className="text-red-500">*</span>
                      </Label>
                      <div className="relative group">
                        <Input 
                          placeholder="Enter first name..." 
                          className="h-11 text-sm border-2 rounded-lg bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-md focus:shadow-primary/10 group-hover:border-border/80"
                          value={formData.firstName}
                          onChange={(e) => onFormChange('firstName', e.target.value)}
                          required
                        />
                        <div className="absolute inset-0 bg-gradient-to-r from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-lg pointer-events-none" />
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label className="text-sm font-semibold text-foreground flex items-center space-x-1.5">
                        <User className="h-3.5 w-3.5 text-primary" />
                        <span>Last Name</span>
                        <span className="text-red-500">*</span>
                      </Label>
                      <div className="relative group">
                        <Input 
                          placeholder="Enter last name..." 
                          className="h-11 text-sm border-2 rounded-lg bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-md focus:shadow-primary/10 group-hover:border-border/80"
                          value={formData.lastName}
                          onChange={(e) => onFormChange('lastName', e.target.value)}
                          required
                        />
                        <div className="absolute inset-0 bg-gradient-to-r from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-lg pointer-events-none" />
                      </div>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label className="text-sm font-semibold text-foreground flex items-center space-x-1.5">
                      <Mail className="h-3.5 w-3.5 text-primary" />
                      <span>Email Address</span>
                      <span className="text-red-500">*</span>
                    </Label>
                    <div className="relative group">
                      <Input 
                        type="email"
                        placeholder="user@example.com" 
                        className="h-11 text-sm border-2 rounded-lg bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-md focus:shadow-primary/10 group-hover:border-border/80"
                        value={formData.email}
                        onChange={(e) => onFormChange('email', e.target.value)}
                        required
                      />
                      <div className="absolute inset-0 bg-gradient-to-r from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-lg pointer-events-none" />
                    </div>
                    <p className="text-xs text-muted-foreground">
                      Login instructions will be sent to this email
                    </p>
                  </div>
                </div>
              </div>

              <Separator className="my-4" />

              {/* Account Security Section */}
              <div className="space-y-4 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-green-500/5 to-transparent rounded-xl -z-10" />
                  <div className="flex items-center space-x-3 p-4 rounded-xl border border-green-500/20 bg-gradient-to-r from-green-50/50 to-transparent">
                    <div className="h-9 w-9 rounded-xl bg-gradient-to-br from-green-100 to-green-50 flex items-center justify-center border border-green-200 shadow-sm">
                      <Lock className="h-4 w-4 text-green-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-base font-bold text-foreground flex items-center space-x-2">
                        <span>Account Security</span>
                        <div className="h-0.5 w-8 bg-gradient-to-r from-green-500 to-green-300 rounded-full" />
                      </h3>
                      <p className="text-xs text-muted-foreground mt-0.5">Password settings</p>
                    </div>
                    <Badge variant="secondary" className="bg-green-100 text-green-700 border-green-200 px-2 py-0.5 text-xs">Security</Badge>
                  </div>
                </div>
                
                <div className="space-y-2">
                  <Label className="text-sm font-semibold text-foreground flex items-center space-x-1.5">
                    <Lock className="h-3.5 w-3.5 text-primary" />
                    <span>Password</span>
                    <span className="text-red-500">*</span>
                  </Label>
                  <div className="relative group">
                    <Input 
                      type="password"
                      placeholder="Enter password (min 6 characters)" 
                      className="h-11 text-sm border-2 rounded-lg bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-md focus:shadow-primary/10 group-hover:border-border/80"
                      value={formData.password}
                      onChange={(e) => onFormChange('password', e.target.value)}
                      required
                      minLength={6}
                    />
                    <div className="absolute inset-0 bg-gradient-to-r from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-lg pointer-events-none" />
                  </div>
                  <p className="text-xs text-muted-foreground">
                    Minimum 6 characters. User can change this after first login.
                  </p>
                </div>
              </div>

              <Separator className="my-4" />

              {/* Role & Permissions Section */}
              <div className="space-y-4 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-purple-500/5 to-transparent rounded-xl -z-10" />
                  <div className="flex items-center space-x-3 p-4 rounded-xl border border-purple-500/20 bg-gradient-to-r from-purple-50/50 to-transparent">
                    <div className="h-9 w-9 rounded-xl bg-gradient-to-br from-purple-100 to-purple-50 flex items-center justify-center border border-purple-200 shadow-sm">
                      <Shield className="h-4 w-4 text-purple-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-base font-bold text-foreground flex items-center space-x-2">
                        <span>Role & Permissions</span>
                        <div className="h-0.5 w-8 bg-gradient-to-r from-purple-500 to-purple-300 rounded-full" />
                      </h3>
                      <p className="text-xs text-muted-foreground mt-0.5">Define user access level</p>
                    </div>
                    <Badge variant="secondary" className="bg-purple-100 text-purple-700 border-purple-200 px-2 py-0.5 text-xs">Access</Badge>
                  </div>
                </div>
                
                <div className="space-y-2">
                  <Label className="text-sm font-semibold text-foreground flex items-center space-x-1.5">
                    <Shield className="h-3.5 w-3.5 text-primary" />
                    <span>User Role</span>
                    <span className="text-red-500">*</span>
                  </Label>
                  <Select 
                    value={formData.role.toString()} 
                    onValueChange={(value) => onFormChange('role', value)}
                  >
                    <SelectTrigger className="h-11 text-sm border-2 rounded-lg bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-md focus:shadow-primary/10">
                      <SelectValue placeholder="Select user role" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="1">
                        <div className="flex items-center gap-2">
                          <span>🎓</span>
                          <span>Student - Access to enrolled courses</span>
                        </div>
                      </SelectItem>
                      <SelectItem value="2">
                        <div className="flex items-center gap-2">
                          <span>👨‍🏫</span>
                          <span>Instructor - Create and manage courses</span>
                        </div>
                      </SelectItem>
                      <SelectItem value="3">
                        <div className="flex items-center gap-2">
                          <span>🛡️</span>
                          <span>Admin - Full system access</span>
                        </div>
                      </SelectItem>
                    </SelectContent>
                  </Select>
                  <p className="text-xs text-muted-foreground">
                    Selected: <span className="font-medium text-foreground">{getRoleLabel(formData.role.toString())}</span>
                  </p>
                </div>
              </div>

              <Separator className="my-4" />

              {/* Additional Information Section */}
              <div className="space-y-4 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-orange-500/5 to-transparent rounded-xl -z-10" />
                  <div className="flex items-center space-x-3 p-4 rounded-xl border border-orange-500/20 bg-gradient-to-r from-orange-50/50 to-transparent">
                    <div className="h-9 w-9 rounded-xl bg-gradient-to-br from-orange-100 to-orange-50 flex items-center justify-center border border-orange-200 shadow-sm">
                      <FileText className="h-4 w-4 text-orange-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-base font-bold text-foreground flex items-center space-x-2">
                        <span>Additional Information</span>
                        <div className="h-0.5 w-8 bg-gradient-to-r from-orange-500 to-orange-300 rounded-full" />
                      </h3>
                      <p className="text-xs text-muted-foreground mt-0.5">Optional profile details</p>
                    </div>
                    <Badge variant="secondary" className="bg-orange-100 text-orange-700 border-orange-200 px-2 py-0.5 text-xs">Optional</Badge>
                  </div>
                </div>
                
                <div className="space-y-2">
                  <Label className="text-sm font-semibold text-foreground flex items-center space-x-1.5">
                    <FileText className="h-3.5 w-3.5 text-primary" />
                    <span>Bio</span>
                  </Label>
                  <div className="relative group">
                    <Textarea 
                      placeholder="Tell us about this user (optional)..." 
                      className="min-h-[80px] resize-none border-2 rounded-lg bg-gradient-to-br from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-md focus:shadow-primary/10 text-sm"
                      value={formData.bio || ''}
                      onChange={(e) => onFormChange('bio', e.target.value)}
                    />
                    <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-lg pointer-events-none" />
                  </div>
                  <p className="text-xs text-muted-foreground">
                    A brief description for this user
                  </p>
                </div>
              </div>
            </form>
          </div>
        </div>

        {/* Compact Modern Footer */}
        <div className="relative border-t border-gradient-to-r from-border/50 to-border bg-gradient-to-r from-muted/40 via-muted/20 to-muted/40 px-6 py-4 mt-4">
          <div className="absolute inset-0 bg-gradient-to-r from-blue-500/5 via-transparent to-purple-500/5 opacity-50" />
          <DialogFooter className="gap-4 flex justify-between items-center relative">
            <div className="flex items-center space-x-2 text-xs text-muted-foreground">
              <div className="h-1.5 w-1.5 rounded-full bg-blue-500 animate-pulse" />
              <span className="font-medium">Ready to create user</span>
            </div>
            <div className="flex items-center gap-3">
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
                disabled={isLoading}
                className="h-10 px-6 rounded-lg border-2 transition-all duration-300 hover:border-red-300 hover:text-red-600 hover:bg-red-50 hover:shadow-md text-sm"
              >
                <X className="h-3.5 w-3.5 mr-1.5" />
                Cancel
              </Button>
              <Button 
                type="submit" 
                disabled={isLoading}
                className="h-10 px-8 rounded-lg bg-gradient-to-r from-blue-600 via-blue-500 to-blue-600 hover:from-blue-500 hover:via-blue-600 hover:to-blue-500 shadow-md hover:shadow-lg transition-all duration-300 hover:scale-105 text-sm"
                onClick={handleSubmit}
              >
                {isLoading ? (
                  <>
                    <div className="h-3.5 w-3.5 mr-1.5 animate-spin rounded-full border-2 border-white border-t-transparent" />
                    Creating...
                  </>
                ) : (
                  <>
                    <Save className="h-3.5 w-3.5 mr-1.5" />
                    Create User
                  </>
                )}
              </Button>
            </div>
          </DialogFooter>
        </div>
      </DialogContent>
    </Dialog>
  )
}
