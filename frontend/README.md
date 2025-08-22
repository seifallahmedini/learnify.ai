# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

# Learnify.ai - Learning Platform

A modern learning platform built with React, TypeScript, and Vite using vertical slice architecture.

## 🏗️ Project Structure

This project follows a **vertical slice architecture** where features are organized by business functionality rather than technical concerns.

```
src/
├── features/              # Feature modules (vertical slices)
│   ├── auth/             # Authentication feature
│   │   ├── components/   
│   │   │   └── LoginPage.tsx
│   │   └── index.ts      # Feature exports
│   ├── dashboard/        # Dashboard feature
│   │   ├── components/
│   │   │   └── DashboardPage.tsx
│   │   └── index.ts
│   ├── courses/          # Courses feature
│   │   ├── components/
│   │   │   └── CoursesPage.tsx
│   │   └── index.ts
│   └── profile/          # Profile feature
│       ├── components/
│       │   └── ProfilePage.tsx
│       └── index.ts
├── shared/               # Shared resources
│   ├── components/       # Shared UI components
│   │   └── ui/          # shadcn/ui components
│   ├── layouts/         # Layout components
│   │   ├── AppSidebar.tsx
│   │   └── MainLayout.tsx
│   ├── hooks/           # Custom hooks
│   │   └── use-mobile.ts
│   ├── lib/             # Utility functions
│   │   └── utils.ts
│   └── router.tsx       # Route configuration
├── App.tsx
└── main.tsx
```

## 🚀 Features

- **Dashboard**: Learning progress overview with stats and recent activity
- **Courses**: Browse and manage courses with search and filtering
- **Profile**: User profile management with learning statistics
- **Authentication**: Login/signup functionality
- **Responsive Design**: Mobile-first design with sidebar navigation

## 🛠️ Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **React Router** - Navigation
- **Tailwind CSS** - Styling
- **shadcn/ui** - UI components
- **Lucide React** - Icons

## 📦 Installation

```bash
npm install
```

## 🏃‍♂️ Development

```bash
npm run dev
```

## 🏛️ Architecture Benefits

### Vertical Slice Architecture
- **Feature Cohesion**: Related functionality is grouped together
- **Easy Navigation**: Features are self-contained and easy to find
- **Scalability**: New features can be added without affecting existing ones
- **Team Collaboration**: Different teams can work on different features independently

### Folder Structure Benefits
- **Clear Separation**: Business logic separated from shared utilities
- **Reusability**: Shared components and utilities can be used across features
- **Maintainability**: Easy to locate and modify specific functionality
- **Testing**: Features can be tested in isolation

## 🎨 UI Components

This project uses [shadcn/ui](https://ui.shadcn.com/) for consistent, accessible, and customizable UI components. All components are located in `src/shared/components/ui/`.

## 🔄 Routing

Routes are organized by features and configured in `src/shared/router.tsx`:

- `/` - Dashboard
- `/courses` - Courses listing
- `/profile` - User profile
- `/login` - Authentication

## 📝 Adding New Features

1. Create a new folder in `src/features/`
2. Add your components in `components/` subdirectory
3. Export main components from `index.ts`
4. Add routes in `src/shared/router.tsx`
5. Update sidebar navigation if needed

Example:
```bash
src/features/
└── new-feature/
    ├── components/
    │   └── NewFeaturePage.tsx
    └── index.ts
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default tseslint.config([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```
