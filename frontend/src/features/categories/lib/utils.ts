// Category-specific utility functions

import type { CategorySummary, CategoryTreeNode } from '../types';

/**
 * Convert flat category list to tree structure
 * @param categories - Flat list of categories
 * @returns Tree structure with parent-child relationships
 */
export function buildCategoryTree(categories: CategorySummary[]): CategoryTreeNode[] {
  const categoryMap = new Map<number, CategoryTreeNode>();
  const roots: CategoryTreeNode[] = [];

  // Create nodes for all categories
  categories.forEach(category => {
    categoryMap.set(category.id, {
      category,
      children: [],
      level: 0,
    });
  });

  // Build tree structure
  categories.forEach(category => {
    const node = categoryMap.get(category.id);
    if (!node) return;

    if (category.parentName) {
      // Find parent by name (you might want to use parentId if available)
      const parent = categories.find(c => c.name === category.parentName);
      if (parent) {
        const parentNode = categoryMap.get(parent.id);
        if (parentNode) {
          node.level = parentNode.level + 1;
          parentNode.children.push(node);
          return;
        }
      }
    }

    // If no parent found or is root, add to roots
    roots.push(node);
  });

  return roots;
}

/**
 * Generate a URL-friendly slug from category name
 * @param name - Category name
 * @returns URL-friendly slug
 */
export function generateSlug(name: string): string {
  return name
    .toLowerCase()
    .trim()
    .replace(/[^\w\s-]/g, '') // Remove special characters
    .replace(/[\s_-]+/g, '-') // Replace spaces and underscores with hyphens
    .replace(/^-+|-+$/g, ''); // Remove leading/trailing hyphens
}

/**
 * Validate category slug
 * @param slug - Slug to validate
 * @returns True if valid, false otherwise
 */
export function isValidSlug(slug: string): boolean {
  const slugRegex = /^[a-z0-9]+(?:-[a-z0-9]+)*$/;
  return slugRegex.test(slug);
}

/**
 * Format category statistics for display
 * @param courseCount - Number of courses
 * @param studentCount - Number of students
 * @returns Formatted statistics object
 */
export function formatCategoryStats(courseCount: number, studentCount: number) {
  return {
    courses: {
      count: courseCount,
      label: courseCount === 1 ? 'course' : 'courses',
      formatted: `${courseCount} ${courseCount === 1 ? 'course' : 'courses'}`,
    },
    students: {
      count: studentCount,
      label: studentCount === 1 ? 'student' : 'students',
      formatted: `${studentCount} ${studentCount === 1 ? 'student' : 'students'}`,
    },
  };
}

/**
 * Get category color with opacity
 * @param color - Base color
 * @param opacity - Opacity value (0-1)
 * @returns Color with opacity
 */
export function getCategoryColorWithOpacity(color: string | undefined, opacity: number = 0.1): string {
  if (!color) return `rgba(100, 116, 139, ${opacity})`; // Default gray
  
  // If it's already an rgba/rgb color, return as is
  if (color.startsWith('rgb')) return color;
  
  // If it's a hex color, convert to rgba
  if (color.startsWith('#')) {
    const hex = color.replace('#', '');
    const r = parseInt(hex.substr(0, 2), 16);
    const g = parseInt(hex.substr(2, 2), 16);
    const b = parseInt(hex.substr(4, 2), 16);
    return `rgba(${r}, ${g}, ${b}, ${opacity})`;
  }
  
  // For named colors or other formats, return with default opacity
  return color;
}