import { useState, useEffect, useCallback } from 'react';

interface UseSelectionManagerProps<T> {
  items: T[];
  getItemId: (item: T) => number;
  onSelectionChange?: (selectedIds: number[]) => void;
}

export function useSelectionManager<T>({
  items,
  getItemId,
  onSelectionChange,
}: UseSelectionManagerProps<T>) {
  const [selectedIds, setSelectedIds] = useState<number[]>([]);

  // Computed properties
  const isAllSelected = selectedIds.length === items.length && items.length > 0;
  const isIndeterminate = selectedIds.length > 0 && selectedIds.length < items.length;
  const selectedCount = selectedIds.length;
  const totalCount = items.length;

  // Selection handlers
  const handleSelectAll = useCallback((checked: boolean) => {
    const newSelection = checked ? items.map(getItemId) : [];
    setSelectedIds(newSelection);
    onSelectionChange?.(newSelection);
  }, [items, getItemId, onSelectionChange]);

  const handleItemSelection = useCallback((itemId: number, checked: boolean) => {
    const newSelection = checked
      ? [...selectedIds, itemId]
      : selectedIds.filter(id => id !== itemId);
    
    setSelectedIds(newSelection);
    onSelectionChange?.(newSelection);
  }, [selectedIds, onSelectionChange]);

  const handleBulkSelection = useCallback((newSelectedIds: number[]) => {
    setSelectedIds(newSelectedIds);
    onSelectionChange?.(newSelectedIds);
  }, [onSelectionChange]);

  const clearSelection = useCallback(() => {
    setSelectedIds([]);
    onSelectionChange?.([]);
  }, [onSelectionChange]);

  const isSelected = useCallback((itemId: number) => {
    return selectedIds.includes(itemId);
  }, [selectedIds]);

  // Keyboard shortcuts
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      // Clear selection with Escape
      if (event.key === 'Escape' && selectedIds.length > 0) {
        clearSelection();
      }
      
      // Select All with Ctrl/Cmd + A (only when not in input field)
      if ((event.ctrlKey || event.metaKey) && event.key === 'a' && items.length > 0) {
        const target = event.target as HTMLElement;
        if (!target.closest('input, textarea, [contenteditable]')) {
          event.preventDefault();
          handleSelectAll(!isAllSelected);
        }
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [selectedIds.length, items.length, isAllSelected, handleSelectAll, clearSelection]);

  return {
    // State
    selectedIds,
    
    // Computed properties
    isAllSelected,
    isIndeterminate,
    selectedCount,
    totalCount,
    
    // Actions
    handleSelectAll,
    handleItemSelection,
    handleBulkSelection,
    clearSelection,
    isSelected,
    
    // Utilities
    setSelectedIds,
  };
}