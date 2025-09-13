import { CourseGridCard } from './CourseGridCard';
import { CourseListCard } from './CourseListCard';
import type { CourseSummary } from '../../types';

type ViewMode = 'list' | 'grid';

interface CourseCardProps {
  course: CourseSummary;
  viewMode: ViewMode;
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

export function CourseCard({ course, viewMode, onView, onEdit, onDelete }: CourseCardProps) {
  const commonProps = {
    course,
    onView,
    onEdit,
    onDelete,
  };

  if (viewMode === 'list') {
    return <CourseListCard {...commonProps} />;
  }

  return <CourseGridCard {...commonProps} />;
}