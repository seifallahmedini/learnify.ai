# ??? Category Management MCP Feature

## Overview

The Category Management MCP feature provides comprehensive category management capabilities for the Learnify.ai platform through a Model Context Protocol (MCP) server. This feature exposes all category-related API endpoints as structured tools that AI assistants can use to manage categories, category hierarchies, and category-course relationships.

## Architecture

### Vertical Slice Structure

```
Features/Categories/
??? Models/
?   ??? CategoryModels.cs        # Category domain models and DTOs
??? Services/
?   ??? CategoryApiService.cs    # MCP tools with API integration
??? CategoryFeature.cs          # Feature registration
```

### Key Components

- **CategoryApiService**: MCP tool provider with `[McpServerToolType]` attribute
- **CategoryModels**: Domain models, request/response DTOs, and hierarchy models
- **CategoryFeature**: Dependency injection registration

## Available MCP Tools (17 Total)

### ?? **Category CRUD Operations**

#### `GetCategoriesAsync`
**Description**: Get all categories with optional filtering and pagination  
**Parameters**:
- `isActive` (optional): Filter by active status
- `parentCategoryId` (optional): Filter by parent category ID
- `rootOnly` (optional): Show only root categories (no parent)
- `searchTerm` (optional): Search in name/description
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `GetCategoryAsync`
**Description**: Get category details by ID  
**Parameters**: 
- `categoryId`: The category ID

#### `CreateCategoryAsync`
**Description**: Create a new category  
**Required Parameters**:
- `name`: Category name
- `description`: Category description

**Optional Parameters**:
- `iconUrl`: Category icon URL
- `parentCategoryId`: Parent category (null for root)
- `isActive`: Active status (default: true)

#### `UpdateCategoryAsync`
**Description**: Update existing category details  
**Parameters**:
- `categoryId`: The category ID to update
- All category fields (optional): name, description, iconUrl, parentCategoryId, isActive

#### `DeleteCategoryAsync`
**Description**: Delete a category permanently  
**Parameters**:
- `categoryId`: The category ID to delete

### ?? **Category Activation Operations**

#### `ActivateCategoryAsync`
**Description**: Activate a category to make it visible  
**Parameters**:
- `categoryId`: The category ID to activate

#### `DeactivateCategoryAsync`
**Description**: Deactivate a category to hide it  
**Parameters**:
- `categoryId`: The category ID to deactivate

### ?? **Category Hierarchy Operations**

#### `GetCategoryTreeAsync`
**Description**: Get complete category hierarchy as a tree structure  
**Parameters**:
- `activeOnly` (optional): Include only active categories

#### `GetSubcategoriesAsync`
**Description**: Get all subcategories of a specific category  
**Parameters**:
- `categoryId`: The parent category ID
- `activeOnly` (optional): Include only active subcategories
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `MoveCategoryAsync`
**Description**: Move a category to a different parent or make it a root category  
**Parameters**:
- `categoryId`: The category ID to move
- `newParentCategoryId` (optional): New parent (null for root)

#### `GetRootCategoriesAsync`
**Description**: Get all root categories (categories without parents)  
**Parameters**:
- `activeOnly` (optional): Include only active categories
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

### ?? **Category Content Operations**

#### `GetCategoryCoursesAsync`
**Description**: Get all courses in a specific category  
**Parameters**:
- `categoryId`: The category ID
- `publishedOnly` (optional): Include only published courses
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `GetCategoryStatsAsync`
**Description**: Get comprehensive statistics for a category  
**Parameters**:
- `categoryId`: The category ID

**Returns**: Course counts, subcategory counts, enrollments, revenue, etc.

### ?? **Utility Operations**

#### `CheckCategoryExistsAsync`
**Description**: Check if a category exists  
**Parameters**:
- `categoryId`: The category ID to check

#### `GetCategorySummaryAsync`
**Description**: Get category summary (basic information only)  
**Parameters**:
- `categoryId`: The category ID

## Data Models

### Category Model
```csharp
public record CategoryModel(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    int? ParentCategoryId,
    string? ParentCategoryName,
    bool IsActive,
    int CourseCount,
    int SubcategoryCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### Category Hierarchy Model
```csharp
public record CategoryHierarchyModel(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    bool IsActive,
    int CourseCount,
    IEnumerable<CategoryHierarchyModel> Subcategories
);
```

### Category Statistics Model
```csharp
public record CategoryStatsModel(
    int CategoryId,
    string CategoryName,
    int TotalCourses,
    int PublishedCourses,
    int UnpublishedCourses,
    int TotalSubcategories,
    int ActiveSubcategories,
    int TotalEnrollments,
    decimal TotalRevenue,
    DateTime LastUpdated
);
```

### Response Formats

#### Success Response
```json
{
  "success": true,
  "data": { /* category data */ },
  "message": "Operation completed successfully"
}
```

#### Error Response
```json
{
  "success": false,
  "message": "Detailed error message"
}
```

## Usage Examples

### Basic Category Operations

**Get all categories:**
```
Tool: GetCategoriesAsync
Parameters: { "page": 1, "pageSize": 10 }
```

**Get active root categories:**
```
Tool: GetRootCategoriesAsync
Parameters: { "activeOnly": true, "page": 1, "pageSize": 20 }
```

**Create a new category:**
```
Tool: CreateCategoryAsync
Parameters: {
  "name": "Web Development",
  "description": "Courses related to web development",
  "iconUrl": "https://example.com/web-icon.png",
  "isActive": true
}
```

**Create a subcategory:**
```
Tool: CreateCategoryAsync
Parameters: {
  "name": "Frontend Development",
  "description": "Frontend web development courses",
  "parentCategoryId": 1,
  "isActive": true
}
```

### Category Hierarchy Management

**Get complete category tree:**
```
Tool: GetCategoryTreeAsync
Parameters: { "activeOnly": true }
```

**Get subcategories:**
```
Tool: GetSubcategoriesAsync
Parameters: { "categoryId": 1, "activeOnly": true }
```

**Move category to different parent:**
```
Tool: MoveCategoryAsync
Parameters: {
  "categoryId": 5,
  "newParentCategoryId": 2
}
```

**Make category a root category:**
```
Tool: MoveCategoryAsync
Parameters: {
  "categoryId": 5,
  "newParentCategoryId": null
}
```

### Category Content Management

**Get courses in category:**
```
Tool: GetCategoryCoursesAsync
Parameters: {
  "categoryId": 1,
  "publishedOnly": true,
  "page": 1,
  "pageSize": 20
}
```

**Get category statistics:**
```
Tool: GetCategoryStatsAsync
Parameters: { "categoryId": 1 }
```

### Category State Management

**Activate category:**
```
Tool: ActivateCategoryAsync
Parameters: { "categoryId": 1 }
```

**Deactivate category:**
```
Tool: DeactivateCategoryAsync
Parameters: { "categoryId": 1 }
```

### Search and Discovery

**Search categories:**
```
Tool: GetCategoriesAsync
Parameters: {
  "searchTerm": "programming",
  "isActive": true,
  "page": 1,
  "pageSize": 10
}
```

**Find subcategories of specific parent:**
```
Tool: GetCategoriesAsync
Parameters: {
  "parentCategoryId": 2,
  "isActive": true
}
```

## API Endpoint Mapping

The MCP tools map directly to the Learnify.ai API endpoints:

| MCP Tool | HTTP Method | API Endpoint |
|----------|-------------|--------------|
| `GetCategoriesAsync` | GET | `/api/categories` |
| `GetCategoryAsync` | GET | `/api/categories/{id}` |
| `CreateCategoryAsync` | POST | `/api/categories` |
| `UpdateCategoryAsync` | PUT | `/api/categories/{id}` |
| `DeleteCategoryAsync` | DELETE | `/api/categories/{id}` |
| `ActivateCategoryAsync` | PUT | `/api/categories/{id}/activate` |
| `DeactivateCategoryAsync` | PUT | `/api/categories/{id}/deactivate` |
| `GetCategoryTreeAsync` | GET | `/api/categories/tree` |
| `GetSubcategoriesAsync` | GET | `/api/categories/{id}/subcategories` |
| `MoveCategoryAsync` | PUT | `/api/categories/{id}/move` |
| `GetCategoryCoursesAsync` | GET | `/api/categories/{id}/courses` |
| `GetCategoryStatsAsync` | GET | `/api/categories/{id}/stats` |

## Integration

### Prerequisites
- Learnify.ai API running on `http://localhost:5271`
- .NET 8 SDK
- Category feature registered in `Program.cs`

### Configuration
```csharp
// Program.cs
builder.Services.AddCategoryFeature();
```

### Error Handling
All tools implement comprehensive error handling:
- Input validation
- API communication errors
- Not found scenarios
- Hierarchy validation (circular references)
- Permission/authorization errors

## Hierarchy Management Best Practices

### Category Structure Design
- **Keep hierarchies shallow** (2-3 levels maximum)
- **Use meaningful names** that reflect content organization
- **Maintain consistent naming** conventions
- **Plan for scalability** when designing category trees

### Category Operations
- **Validate hierarchy integrity** before moves
- **Check for circular references** when setting parents
- **Consider course migration** when deleting categories
- **Maintain active/inactive state** consistency

### Performance Considerations
- **Use pagination** for large category lists
- **Cache frequently accessed** category trees
- **Filter by active status** to reduce data transfer
- **Use summary endpoints** for lightweight operations

## Testing Commands

Test the category MCP tools with Claude:

### **Basic Operations:**
1. **"Get all active categories"**
2. **"Create a new category for Programming"**
3. **"Get the complete category tree"**
4. **"Find all root categories"**

### **Hierarchy Operations:**
1. **"Get subcategories of Web Development"**
2. **"Move Frontend category under Web Development"**
3. **"Make React a subcategory of Frontend"**

### **Content Operations:**
1. **"Get all courses in JavaScript category"**
2. **"Get statistics for Programming category"**
3. **"Find published courses in Web Development"**

### **Advanced Operations:**
1. **"Reorganize the entire category structure"**
2. **"Find categories with no courses"**
3. **"Get the most popular categories by enrollment"**

## Category Hierarchy Example

```
Technology (Root)
??? Programming
?   ??? Web Development
?   ?   ??? Frontend
?   ?   ?   ??? React
?   ?   ?   ??? Vue.js
?   ?   ?   ??? Angular
?   ?   ??? Backend
?   ?       ??? Node.js
?   ?       ??? Python
?   ?       ??? Java
?   ??? Mobile Development
?       ??? iOS
?       ??? Android
??? Design
    ??? UI/UX Design
    ??? Graphic Design
```

## Best Practices

### Category Creation
- **Choose descriptive names** that users will understand
- **Write clear descriptions** explaining category purpose
- **Use appropriate icons** for visual identification
- **Plan hierarchy structure** before creating categories

### Hierarchy Management
- **Avoid deep nesting** (3 levels maximum recommended)
- **Group related categories** logically
- **Consider user navigation** when organizing
- **Maintain consistency** in naming and structure

### Category Maintenance
- **Regular cleanup** of unused categories
- **Monitor course distribution** across categories
- **Update descriptions** as content evolves
- **Deactivate** rather than delete when possible

The Category MCP feature provides comprehensive category management with powerful hierarchy operations while maintaining consistency with the lesson and course features! ???