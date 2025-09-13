# Copilot Instructions for Learnify.ai Frontend

## Project Overview
- **Stack:** React 18, TypeScript, Vite, Tailwind CSS, shadcn/ui, React Router
- **Architecture:** Vertical slice (features grouped by business domain)
- **Purpose:** Modern learning platform with dashboard, courses, profile, and authentication features

## Key Patterns & Structure
- **Features:** Each business domain (auth, courses, dashboard, profile, users) lives in `src/features/<feature>/` with its own `components/`, `hooks/`, `services/`, and `types/`.
- **Shared Resources:** Common UI and utilities are in `src/shared/` (not in features). UI components use shadcn/ui and are in `src/shared/components/ui/`.
- **Routing:** All routes are defined in `src/shared/router.tsx` and map to feature pages. Example routes: `/`, `/courses`, `/profile`, `/login`.
- **Layouts:** Main layouts are in `src/shared/layouts/` (e.g., `MainLayout.tsx`, `AppSidebar.tsx`).
- **Utilities:** General-purpose helpers are in `src/shared/lib/utils.ts` and `src/lib/utils.ts`.

## Developer Workflow
- **Install:** `npm install`
- **Dev Server:** `npm run dev` (Vite)
- **Lint:** ESLint config in `eslint.config.js` (supports React, TypeScript, shadcn/ui)
- **Type Checking:** Uses `tsconfig.json`, `tsconfig.app.json`, and `tsconfig.node.json`
- **Styling:** Tailwind CSS utility classes; shadcn/ui for consistent design

## Conventions & Practices

### **Vertical Slice Architecture**
- **Feature Isolation:** Keep all feature logic (components, hooks, services, types) together in the feature folder
- **Sub-feature Grouping:** Organize components by UI domains (e.g., `user-table/`, `user-details/`, `dialogs/`, `shared/`)
- **Clear Boundaries:** Each feature has a well-defined public API through `index.ts` exports

### **Component Organization Best Practices**
- **Page Components:** Use descriptive names ending with `Page` (e.g., `UsersListPage.tsx`, `UserDetailsPage.tsx`)
- **Sub-components:** Group related components in domain folders (`user-table/UserTable.tsx`, `user-table/UserTableRow.tsx`)
- **Shared Components:** Place feature-specific reusable components in `shared/` (e.g., `UserStatusBadge.tsx`, `LoadingSkeleton.tsx`)
- **Dialog Components:** Keep all modals/dialogs in `dialogs/` folder with descriptive names (`CreateUserDialog.tsx`, `DeleteUserDialog.tsx`)

### **Hook Patterns**
- **Management Hooks:** Use `use[Feature]Management` for CRUD operations and state (e.g., `useUserManagement.ts`)
- **Form Hooks:** Use `useCreate[Feature]Form` for form logic (e.g., `useCreateUserForm.ts`)
- **Utility Hooks:** Use `use[Feature]Utils` for helper functions (e.g., `useUserUtils.tsx`)
- **State Management:** Combine multiple state concerns in management hooks with clear loading states

### **Service Layer Patterns**
- **API Classes:** Use class-based services with instance methods (e.g., `UsersApiService`)
- **Private Request Method:** Implement a shared `request<T>()` method for DRY HTTP operations
- **Error Handling:** Implement consistent error handling in the private request method
- **Generic Responses:** Use `ApiResponse<T>` wrapper for consistent API responses
- **Instance Export:** Export service instances (e.g., `export const usersApi = new UsersApiService()`)
- **Query Parameters:** Use `URLSearchParams` for consistent query string building

### **TypeScript Best Practices**
- **Domain Types:** Define feature-specific types in dedicated files (e.g., `User`, `UserRole`, `UserListResponse`)
- **Enum Patterns:** Use object-style enums for constants (`UserRole = { Student: 1, Instructor: 2, Admin: 3 }`)
- **Request/Response Interfaces:** Separate types for API requests (`CreateUserRequest`) and responses (`UserResponse`)
- **Utility Types:** Create helper types for common patterns (`UserFilterRequest`, `UserSummary`)

### **Export Strategy**
- **Barrel Exports:** Use `index.ts` files for clean imports
- **Selective Exports:** Export only what other features need from the main feature `index.ts`
- **Component Grouping:** Group exports by functionality in component `index.ts` files
```typescript
// ✅ Good - Clean grouped exports
export { UsersListPage, UserTable, UserTableRow, UserFilters } from './user-table'
export { UserRoleBadge, UserStatusBadge } from './shared'
```

### **State Management Patterns**
- **Local State:** Use React hooks for component-specific state
- **Loading States:** Implement granular loading states (`loading`, `isCreating`, `isLoadingUser`)
- **Form State:** Separate form state from data fetching state
- **Error Boundaries:** Handle errors gracefully with user-friendly messages

### **UI Component Standards**
- **Badge Components:** Create feature-specific badge components for status/role display
- **Loading States:** Implement skeleton loaders for better UX
- **Pagination:** Use reusable pagination components for data tables
- **Responsive Design:** Use Tailwind responsive classes (`md:grid-cols-2`, `lg:grid-cols-3`)

### **Routing Best Practices**
- **Nested Routes:** Use parent-child route structure for feature hierarchies
- **Dynamic Routes:** Use parameters for detail pages (`/users/:userId`)
- **Route Organization:** Import page components directly in router, not entire features

## Integration Points
- **API Calls:** Use `src/lib/api-client.ts` for HTTP requests
- **Icons:** Use Lucide React for icons
- **State Management:** Local state via React hooks; no global state library

## Communication Between Features

### **Event-Driven Communication**
```typescript
// shared/events/userEvents.ts
export const userEvents = {
  USER_CREATED: 'user:created',
  USER_UPDATED: 'user:updated',
} as const;

// features/users/hooks/useUserEvents.ts
export const useUserEvents = () => {
  const emitUserCreated = (user: User) => {
    window.dispatchEvent(new CustomEvent(userEvents.USER_CREATED, { 
      detail: { user } 
    }));
  };
  // ...
};
```

### **Context for Global State**
```typescript
// shared/context/AppContext.tsx
export const AppContext = createContext();

// Features consume but don't create global state
```

## Mobile-First Considerations

```typescript
// shared/hooks/useResponsive.ts
export const useResponsive = () => {
  // Responsive logic
};

// Each feature adapts its components
// features/users/components/user-table/UsersListPage.tsx
const { isMobile } = useResponsive();
return isMobile ? <UserCardGrid /> : <UserTable />;
```

## Examples

### **Adding a New Feature (Complete Pattern)**
```typescript
// 1. Create feature structure
src/features/new-feature/
├── components/
│   ├── feature-list/          # Main listing components
│   │   ├── FeatureListPage.tsx
│   │   ├── FeatureTable.tsx
│   │   ├── FeatureFilters.tsx
│   │   └── index.ts
│   ├── feature-details/       # Detail/view components
│   │   ├── FeatureDetailsPage.tsx
│   │   └── index.ts
│   ├── dialogs/              # Modal components
│   │   ├── CreateFeatureDialog.tsx
│   │   ├── DeleteFeatureDialog.tsx
│   │   └── index.ts
│   ├── shared/               # Feature-specific shared components
│   │   ├── FeatureStatusBadge.tsx
│   │   ├── LoadingSkeleton.tsx
│   │   └── index.ts
│   └── index.ts
├── hooks/
│   ├── useFeatureManagement.ts
│   ├── useCreateFeatureForm.ts
│   └── index.ts
├── services/
│   ├── featureService.ts
│   └── index.ts
├── types/
│   ├── feature.types.ts
│   └── index.ts
└── index.ts

// 2. Export from feature index.ts
export { FeatureListPage, FeatureDetailsPage } from './components'
export * from './hooks'
export * from './types'
export { featureService } from './services'

// 3. Add routes in router.tsx
import { FeatureListPage, FeatureDetailsPage } from '@/features/new-feature'

{
  path: "features",
  element: <FeatureListPage />,
},
{
  path: "features/:featureId",
  element: <FeatureDetailsPage />,
}

// 4. Update sidebar navigation in AppSidebar.tsx
```

### **Component Patterns**
```typescript
// ✅ Page Component Pattern
export function FeatureListPage() {
  const [filters, setFilters] = useState({})
  const { features, loading, error } = useFeatureManagement()
  
  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Features</h1>
        <Button onClick={handleCreate}>Create Feature</Button>
      </div>
      <FeatureFilters onFiltersChange={setFilters} />
      {loading ? <LoadingSkeleton /> : <FeatureTable features={features} />}
    </div>
  )
}

// ✅ Badge Component Pattern
export function FeatureStatusBadge({ status }: { status: FeatureStatus }) {
  const config = {
    active: { label: 'Active', variant: 'default' as const },
    inactive: { label: 'Inactive', variant: 'secondary' as const }
  }
  
  return <Badge variant={config[status].variant}>{config[status].label}</Badge>
}
```

### **Hook Patterns**
```typescript
// ✅ Management Hook Pattern
export function useFeatureManagement() {
  const [features, setFeatures] = useState<Feature[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  
  const loadFeatures = async () => {
    try {
      setLoading(true)
      const response = await featureService.getFeatures()
      setFeatures(response.data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }
  
  return { features, loading, error, loadFeatures, createFeature, updateFeature, deleteFeature }
}
```

### **Service Patterns**
```typescript
// ✅ Service Class Pattern
interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

class FeatureApiService {
  private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      throw new Error(`API request failed: ${response.statusText}`);
    }

    const result: ApiResponse<T> = await response.json();
    return result.data;
  }

  async getFeatures(filters?: FeatureFilterRequest): Promise<FeatureListResponse> {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = queryString ? `?${queryString}` : '';
    return this.request<FeatureListResponse>(endpoint);
  }

  async createFeature(data: CreateFeatureRequest): Promise<FeatureResponse> {
    return this.request<FeatureResponse>('', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateFeature(id: number, data: UpdateFeatureRequest): Promise<FeatureResponse> {
    return this.request<FeatureResponse>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async deleteFeature(id: number): Promise<{ success: boolean }> {
    return this.request<{ success: boolean }>(`/${id}`, {
      method: 'DELETE',
    });
  }
}

// Export instance for use in hooks and components
export const featureApi = new FeatureApiService();
```

## References
- See `README.md` for architecture and workflow details
- See `src/shared/router.tsx` for routing patterns
- See `src/shared/components/ui/` for UI conventions

---
If any section is unclear or missing, please provide feedback to improve these instructions.