import { useState, useEffect } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/shared/components/ui/dialog';
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/shared/components/ui/form';
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Switch } from '@/shared/components/ui/switch';
import { Edit, Video, Clock, Target, Link2 } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useLessonOperations } from '../../hooks/useLessonManagement';
import { formatLessonDuration, parseDurationToMinutes } from '../../lib';
import type { Lesson, LessonResource } from '../../types';

const editLessonSchema = z.object({
  title: z.string().min(1, 'Title is required').min(3, 'Title must be at least 3 characters'),
  description: z.string().min(1, 'Description is required'),
  content: z.string().min(1, 'Content is required'),
  videoUrl: z.string().url('Invalid URL').optional().or(z.literal('')),
  duration: z.string().min(1, 'Duration is required').refine(
    (val) => {
      const minutes = parseDurationToMinutes(val);
      return !isNaN(minutes) && minutes > 0;
    },
    { message: 'Invalid duration format (e.g., "1h 30m" or "90m")' }
  ),
  orderIndex: z.string().optional(),
  isFree: z.boolean().default(false),
  isPublished: z.boolean().default(false),
  learningObjectives: z.string().optional(),
  resources: z.string().optional(),
});

type EditLessonFormData = z.infer<typeof editLessonSchema>;

interface EditLessonDialogProps {
  lesson: Lesson;
  onLessonUpdated?: (lesson: Lesson) => void;
  trigger?: React.ReactNode;
}

export function EditLessonDialog({ lesson, onLessonUpdated, trigger }: EditLessonDialogProps) {
  const [open, setOpen] = useState(false);
  const { updateLesson, isLoading, error: apiError } = useLessonOperations();

  // Parse resources for display (convert JSON string to readable format)
  const parseResourcesForDisplay = (resourcesJson?: string): string => {
    if (!resourcesJson) return '';
    try {
      const resources = JSON.parse(resourcesJson) as LessonResource[];
      if (!Array.isArray(resources)) return '';
      // Format as JSON for editing
      return JSON.stringify(resources, null, 2);
    } catch {
      return '';
    }
  };

  const form = useForm<EditLessonFormData>({
    resolver: zodResolver(editLessonSchema),
    defaultValues: {
      title: lesson.title,
      description: lesson.description,
      content: lesson.content,
      videoUrl: lesson.videoUrl || '',
      duration: formatLessonDuration(lesson.duration),
      orderIndex: lesson.orderIndex.toString(),
      isFree: lesson.isFree,
      isPublished: lesson.isPublished,
      learningObjectives: lesson.learningObjectives || '',
      resources: parseResourcesForDisplay(lesson.resources),
    },
  });

  useEffect(() => {
    if (open) {
      form.reset({
        title: lesson.title,
        description: lesson.description,
        content: lesson.content,
        videoUrl: lesson.videoUrl || '',
        duration: formatLessonDuration(lesson.duration),
        orderIndex: lesson.orderIndex.toString(),
        isFree: lesson.isFree,
        isPublished: lesson.isPublished,
        learningObjectives: lesson.learningObjectives || '',
        resources: parseResourcesForDisplay(lesson.resources),
      });
    }
  }, [open, lesson, form]);

  const onSubmit = async (data: EditLessonFormData) => {
    try {
      // Validate and format resources JSON
      let resourcesJson: string | undefined = undefined;
      if (data.resources && data.resources.trim()) {
        try {
          const parsed = JSON.parse(data.resources);
          // Validate it's an array
          if (Array.isArray(parsed)) {
            resourcesJson = JSON.stringify(parsed);
          }
        } catch {
          // Invalid JSON, skip it
        }
      }

      const updated = await updateLesson(lesson.id, {
        title: data.title,
        description: data.description,
        content: data.content,
        videoUrl: data.videoUrl || undefined,
        duration: parseDurationToMinutes(data.duration),
        orderIndex: data.orderIndex ? parseInt(data.orderIndex, 10) : undefined,
        isFree: data.isFree,
        isPublished: data.isPublished,
        learningObjectives: data.learningObjectives?.trim() || undefined,
        resources: resourcesJson,
      });

      if (updated) {
        onLessonUpdated?.(updated);
        setOpen(false);
      }
    } catch (error) {
      // Error handled by hook
    }
  };

  const defaultTrigger = (
    <Button variant="outline" size="sm">
      <Edit className="h-4 w-4 mr-2" />
      Edit
    </Button>
  );

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || defaultTrigger}
      </DialogTrigger>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-2xl">
            <Edit className="h-6 w-6 text-primary" />
            Edit Lesson
          </DialogTitle>
          <DialogDescription>
            Update lesson details below.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Lesson Title *</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter lesson title" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description *</FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Enter lesson description" 
                      className="min-h-[100px]"
                      {...field} 
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="content"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Content *</FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Enter lesson content (Markdown supported)" 
                      className="min-h-[150px]"
                      {...field} 
                    />
                  </FormControl>
                  <FormDescription>
                    The main content of the lesson (Markdown format supported)
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="learningObjectives"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="flex items-center gap-2">
                    <Target className="h-4 w-4" />
                    Learning Objectives
                  </FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Enter learning objectives, separated by commas or newlines&#10;Example: Understand React hooks, Learn component composition, Master state management" 
                      className="min-h-[100px]"
                      {...field} 
                    />
                  </FormControl>
                  <FormDescription>
                    List learning objectives separated by commas or newlines
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="resources"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="flex items-center gap-2">
                    <Link2 className="h-4 w-4" />
                    Resources (JSON)
                  </FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder='[{"name": "Example Resource", "url": "https://example.com", "type": "external"}]' 
                      className="min-h-[120px] font-mono text-xs"
                      {...field} 
                    />
                  </FormControl>
                  <FormDescription>
                    JSON array format: Array of objects with name, url, and type (download|external|code)
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="videoUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      <Video className="h-4 w-4" />
                      Video URL
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="https://example.com/video.mp4" {...field} />
                    </FormControl>
                    <FormDescription>
                      Optional video URL for this lesson
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="duration"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      <Clock className="h-4 w-4" />
                      Duration *
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="1h 30m or 90m" {...field} />
                    </FormControl>
                    <FormDescription>
                      Format: "1h 30m" or "90m"
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="orderIndex"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Order Index</FormLabel>
                  <FormControl>
                    <Input type="number" placeholder="Order in course" {...field} />
                  </FormControl>
                  <FormDescription>
                    Position of this lesson in the course sequence
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="flex gap-6">
              <FormField
                control={form.control}
                name="isFree"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Free Lesson</FormLabel>
                      <FormDescription>
                        Make this lesson accessible without payment
                      </FormDescription>
                    </div>
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="isPublished"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Published</FormLabel>
                      <FormDescription>
                        Make this lesson visible to students
                      </FormDescription>
                    </div>
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />
            </div>

            {apiError && (
              <div className="p-3 text-sm text-red-600 bg-red-50 rounded-md">
                {apiError}
              </div>
            )}

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isLoading}>
                {isLoading ? 'Updating...' : 'Update Lesson'}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

