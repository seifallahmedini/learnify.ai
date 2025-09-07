# EditUser Component Documentation

## Overview
The `EditUser` component is a comprehensive dialog component for editing user profiles in the Learnify.ai application. It provides a user-friendly interface for updating user information including personal details, contact information, and account settings.

## Features

### ✅ User Information Editing
- **Basic Information**: First name, last name, and biography
- **Contact Details**: Phone number and date of birth
- **Account Settings**: Active/inactive status toggle
- **Profile Preview**: Shows current user avatar and basic info

### ✅ Form Validation
- Client-side validation for all fields
- Required field validation for first and last name
- Phone number format validation
- Character limits (bio max 500 characters)
- Real-time error display

### ✅ User Experience
- Modal dialog interface with backdrop
- Responsive design (works on mobile and desktop)
- Loading states during save operations
- Error handling with user-friendly messages
- Form reset on cancel

### ✅ Read-only Information Display
- Email address (non-editable)
- User role (non-editable)
- Member since date
- Last updated timestamp

## Component Structure

```tsx
interface EditUserProps {
  user: User
  open: boolean
  onOpenChange: (open: boolean) => void
  onUserUpdated: (updatedUser: User) => void
}
```

## Usage Example

```tsx
import { EditUser } from '@/features/users/components/shared'

function UserManagement() {
  const [showEditDialog, setShowEditDialog] = useState(false)
  const [selectedUser, setSelectedUser] = useState<User | null>(null)

  const handleEditUser = async (userId: number) => {
    const user = await usersApi.getUserById(userId)
    setSelectedUser(user)
    setShowEditDialog(true)
  }

  const handleUserUpdated = async (updatedUser: User) => {
    // Refresh user list or update local state
    await refreshUsers()
  }

  return (
    <div>
      {/* Trigger button */}
      <Button onClick={() => handleEditUser(123)}>
        Edit User
      </Button>

      {/* Edit dialog */}
      {selectedUser && (
        <EditUser
          user={selectedUser}
          open={showEditDialog}
          onOpenChange={setShowEditDialog}
          onUserUpdated={handleUserUpdated}
        />
      )}
    </div>
  )
}
```

## Integration

The EditUser component is already integrated into:

### UsersListPage
- Available through the "Edit User" option in the actions dropdown menu
- Automatically loads user data when edit is triggered
- Refreshes the user list after successful updates

### Components Used
- **Dialog Components**: `Dialog`, `DialogContent`, `DialogHeader`, `DialogFooter`
- **Form Components**: `Input`, `Textarea`, `Label`, native checkbox
- **UI Components**: `Button`, `Avatar`, `Badge`, `Separator`
- **Icons**: From `lucide-react`

## API Integration

### Required API Methods
```typescript
// Get user by ID for editing
usersApi.getUserById(id: number): Promise<User>

// Update user data
usersApi.updateUser(id: number, data: UpdateUserRequest): Promise<UserResponse>
```

### Data Types
```typescript
interface UpdateUserRequest {
  firstName: string
  lastName: string
  bio?: string
  dateOfBirth?: string
  phoneNumber?: string
  isActive: boolean
}
```

## Form Fields

### Editable Fields
1. **First Name** (required, 2-50 characters)
2. **Last Name** (required, 2-50 characters)
3. **Biography** (optional, max 500 characters)
4. **Phone Number** (optional, format validated)
5. **Date of Birth** (optional, date picker)
6. **Account Status** (active/inactive toggle)

### Read-only Fields
1. **Email Address** - Cannot be changed through this form
2. **User Role** - Requires separate role management
3. **Member Since** - Account creation date
4. **Last Updated** - Automatically updated on save

## Validation Rules

| Field | Rules |
|-------|-------|
| First Name | Required, 2-50 characters |
| Last Name | Required, 2-50 characters |
| Biography | Optional, max 500 characters |
| Phone Number | Optional, international format validation |
| Date of Birth | Optional, valid date |
| Account Status | Boolean toggle |

## Error Handling

- **Network Errors**: Shows "Failed to update user. Please try again."
- **Validation Errors**: Individual field error messages
- **Loading States**: Prevents multiple submissions
- **Form Reset**: Clears errors and resets to original values on cancel

## Styling

- Uses Tailwind CSS classes for styling
- Responsive grid layout for form fields
- Consistent with application design system
- Proper spacing and typography hierarchy

## Accessibility

- Proper form labels and descriptions
- Keyboard navigation support
- Screen reader compatible
- Focus management in dialog

## Future Enhancements

### Potential Improvements
1. **Profile Picture Upload**: Add avatar editing capability
2. **Role Management**: Admin interface for changing user roles
3. **Password Reset**: Integrate password change functionality
4. **Audit Trail**: Show edit history and changelog
5. **Bulk Edit**: Select and edit multiple users
6. **Advanced Validation**: Server-side validation feedback

### Integration Opportunities
1. **User Details Page**: Add edit button to user profile view
2. **Admin Dashboard**: Quick edit from user statistics
3. **Settings Page**: User self-edit functionality
4. **Mobile App**: Responsive design optimization

## Technical Notes

- Component uses controlled form inputs for better state management
- Implements proper TypeScript types for all props and data
- Follows React best practices for state updates and event handling
- Uses async/await pattern for API calls
- Includes proper error boundaries and loading states

This component provides a solid foundation for user management in the Learnify.ai application and can be easily extended or customized for specific requirements.
