// Categories feature public API
export { CategoriesListPage, CategoryDetailsPage } from './components';
export { useCategoryManagement } from './hooks';
export type { 
  Category, 
  CategorySummary, 
  CreateCategoryRequest, 
  CategoryFilterRequest 
} from './types';
export { categoriesApi } from './services';