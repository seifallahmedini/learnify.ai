import { useState } from 'react';
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
import { Plus, BookOpen, Video, Clock } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useCreateLesson } from '../../hooks/useLessonManagement';
import { formatLessonDuration, parseDurationToMinutes } from '../../lib';

const createLessonSchema = z.object({
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
  isFree: z.boolean().default(false),
  isPublished: z.boolean().default(false),
});

type CreateLessonFormData = z.infer<typeof createLessonSchema>;

interface CreateLessonDialogProps {
  courseId: number;
  onLessonCreated?: (lesson: any) => void;
  trigger?: React.ReactNode;
}

export function CreateLessonDialog({ courseId, onLessonCreated, trigger }: CreateLessonDialogProps) {
  const [open, setOpen] = useState(false);
  const { createLesson, isLoading, error: apiError } = useCreateLesson();

  const form = useForm<CreateLessonFormData>({
    resolver: zodResolver(createLessonSchema),
    defaultValues: {
      title: '',
      description: '',
      content: '',
      videoUrl: '',
      duration: '30m',
      isFree: false,
      isPublished: false,
    },
  });

  const onSubmit = async (data: CreateLessonFormData) => {
    try {
      const lesson = await createLesson(courseId, {
        title: data.title,
        description: data.description,
        content: data.content,
        videoUrl: data.videoUrl || undefined,
        duration: parseDurationToMinutes(data.duration),
        isFree: data.isFree,
        isPublished: data.isPublished,
      });

      if (lesson) {
        onLessonCreated?.(lesson);
        setOpen(false);
        form.reset();
      }
    } catch (error) {
      // Error handled by hook
    }
  };

  const defaultTrigger = (
    <Button>
      <Plus className="h-4 w-4 mr-2" />
      Create Lesson
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
            <BookOpen className="h-6 w-6 text-primary" />
            Create New Lesson
          </DialogTitle>
          <DialogDescription>
            Add a new lesson to your course. Fill in the details below.
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
                      placeholder="Enter lesson content" 
                      className="min-h-[150px]"
                      {...field} 
                    />
                  </FormControl>
                  <FormDescription>
                    The main content of the lesson
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
                      <FormLabel className="text-base">Publish Immediately</FormLabel>
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
                {isLoading ? 'Creating...' : 'Create Lesson'}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

