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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { Switch } from '@/shared/components/ui/switch';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { 
  BookOpen, 
  Users, 
  Clock, 
  DollarSign, 
  Target, 
  Settings, 
  X,
  Save,
  AlertCircle
} from 'lucide-react';
import { useEditCourseForm } from '../../hooks';
import { type Course, CourseLevel } from '../../types';
import { getCourseLevelLabel } from '../../lib';

interface EditCourseDialogProps {
  course: Course | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCourseUpdated?: (course: Course) => void;
  trigger?: React.ReactNode;
}

export function EditCourseDialog({ course, open, onOpenChange, onCourseUpdated, trigger }: EditCourseDialogProps) {
  // Only render if we have a course to edit
  if (!course || course.id === 0) {
    return null;
  }

  const { form, isSubmitting, error, submitForm } = useEditCourseForm(
    course,
    (updatedCourse) => {
      onCourseUpdated?.(updatedCourse);
      onOpenChange(false);
    }
  );

  const handleSubmit = async (data: any) => {
    try {
      await submitForm(data);
    } catch (error) {
      // Error is handled by the form hook
    }
  };

  const handleClose = () => {
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      {trigger && (
        <DialogTrigger asChild>
          {trigger}
        </DialogTrigger>
      )}
      <DialogContent className="w-[75vw] max-w-[1400px] min-w-[800px] max-h-[90vh] overflow-hidden flex flex-col">
        <DialogHeader className="space-y-6 pb-8 relative overflow-hidden">
          {/* Background Pattern */}
          <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-blue-500/5 -z-10" />
          
          <div className="flex items-center justify-between relative">
            <div className="flex items-center space-x-4">
              <div className="h-14 w-14 rounded-2xl bg-gradient-to-br from-primary/20 to-primary/10 flex items-center justify-center border border-primary/20 shadow-sm">
                <BookOpen className="h-7 w-7 text-primary" />
              </div>
              <div className="space-y-1">
                <DialogTitle className="text-2xl font-bold tracking-tight bg-gradient-to-r from-foreground to-foreground/80 bg-clip-text">
                  Edit Course
                </DialogTitle>
                <DialogDescription className="text-base text-muted-foreground">
                  Seamlessly update course details and settings
                </DialogDescription>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <Badge variant="outline" className="bg-gradient-to-r from-blue-50 to-blue-100 text-blue-700 border-blue-200 px-4 py-2 font-medium shadow-sm">
                ID: {course.id}
              </Badge>
            </div>
          </div>
          
          {/* Enhanced Course Preview */}
          <div className="bg-gradient-to-r from-card via-card/80 to-card rounded-2xl p-6 border border-border/50 shadow-sm backdrop-blur-sm">
            <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 items-center">
              <div className="lg:col-span-8 space-y-3">
                <div className="flex items-center space-x-2">
                  <div className="h-2 w-2 rounded-full bg-green-500 animate-pulse" />
                  <span className="text-sm font-medium text-muted-foreground uppercase tracking-wider">Currently Editing</span>
                </div>
                <h4 className="text-xl font-bold text-foreground leading-tight">{course.title}</h4>
                <p className="text-sm text-muted-foreground opacity-80 leading-relaxed">{course.shortDescription}</p>
              </div>
              <div className="lg:col-span-4 grid grid-cols-2 gap-4">
                <div className="text-center p-4 bg-gradient-to-br from-primary/10 to-primary/5 rounded-xl border border-primary/20">
                  <Users className="h-5 w-5 text-primary mx-auto mb-2" />
                  <p className="text-2xl font-bold text-foreground">{course.totalStudents}</p>
                  <p className="text-xs text-muted-foreground font-medium">Students</p>
                </div>
                <div className="text-center p-4 bg-gradient-to-br from-green-500/10 to-green-500/5 rounded-xl border border-green-500/20">
                  <DollarSign className="h-5 w-5 text-green-600 mx-auto mb-2" />
                  <p className="text-2xl font-bold text-foreground">${course.price}</p>
                  <p className="text-xs text-muted-foreground font-medium">Price</p>
                </div>
              </div>
            </div>
          </div>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto px-2 py-2">
          <div className="max-w-none mx-auto">
            <Form {...form}>
              <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-12">
              {/* Basic Information Section */}
              <div className="space-y-8 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-green-500/5 to-transparent rounded-2xl -z-10" />
                  <div className="flex items-center space-x-4 p-6 rounded-2xl border border-green-500/20 bg-gradient-to-r from-green-50/50 to-transparent">
                    <div className="h-12 w-12 rounded-2xl bg-gradient-to-br from-green-100 to-green-50 flex items-center justify-center border border-green-200 shadow-sm">
                      <BookOpen className="h-6 w-6 text-green-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-xl font-bold text-foreground flex items-center space-x-2">
                        <span>Basic Information</span>
                        <div className="h-1 w-12 bg-gradient-to-r from-green-500 to-green-300 rounded-full" />
                      </h3>
                      <p className="text-sm text-muted-foreground mt-1">Essential course details and descriptions</p>
                    </div>
                    <Badge variant="secondary" className="bg-green-100 text-green-700 border-green-200 px-3 py-1">Required</Badge>
                  </div>
                </div>
                <div className="space-y-8">
                  <FormField
                    control={form.control}
                    name="title"
                    render={({ field }) => (
                      <FormItem className="space-y-3">
                        <FormLabel className="text-base font-semibold text-foreground flex items-center space-x-2">
                          <span>Course Title</span>
                          <span className="text-red-500">*</span>
                        </FormLabel>
                        <FormControl>
                          <div className="relative group">
                            <Input 
                              placeholder="Enter a compelling and descriptive course title..." 
                              className="h-14 text-base border-2 rounded-xl bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-lg focus:shadow-primary/10 group-hover:border-border/80"
                              {...field} 
                            />
                            <div className="absolute inset-0 bg-gradient-to-r from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-xl pointer-events-none" />
                          </div>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <div className="grid grid-cols-1 xl:grid-cols-2 gap-8">
                    <FormField
                      control={form.control}
                      name="shortDescription"
                      render={({ field }) => (
                        <FormItem className="space-y-3">
                          <FormLabel className="text-base font-semibold text-foreground flex items-center space-x-2">
                            <span>Short Description</span>
                            <span className="text-red-500">*</span>
                          </FormLabel>
                          <FormControl>
                            <div className="relative group">
                              <Textarea 
                                placeholder="Write a compelling brief description that attracts students and highlights key benefits..."
                                className="min-h-[130px] resize-none border-2 rounded-xl bg-gradient-to-br from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-lg focus:shadow-primary/10 pr-20"
                                maxLength={500}
                                {...field} 
                              />
                              <div className="absolute bottom-4 right-4 bg-background/90 backdrop-blur-sm px-3 py-1.5 rounded-lg border border-border/50 text-xs font-medium text-muted-foreground shadow-sm">
                                {field.value?.length || 0}/500
                              </div>
                              <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-focus-within:opacity-100 transition-opacity duration-300 rounded-xl pointer-events-none" />
                            </div>
                          </FormControl>
                          <FormDescription className="text-sm text-muted-foreground">
                            This appears in course listings and search results
                          </FormDescription>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name="description"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-sm font-medium text-foreground">Full Description *</FormLabel>
                          <FormControl>
                            <Textarea 
                              placeholder="Detailed description of the course content and benefits"
                              className="min-h-[100px] resize-none"
                              {...field} 
                            />
                          </FormControl>
                          <FormDescription className="text-xs">
                            Provide comprehensive details about what students will learn
                          </FormDescription>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>
              </div>

              <Separator className="my-8" />

              {/* Course Details Section */}
              <div className="space-y-8 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-blue-500/5 to-transparent rounded-2xl -z-10" />
                  <div className="flex items-center space-x-4 p-6 rounded-2xl border border-blue-500/20 bg-gradient-to-r from-blue-50/50 to-transparent">
                    <div className="h-12 w-12 rounded-2xl bg-gradient-to-br from-blue-100 to-blue-50 flex items-center justify-center border border-blue-200 shadow-sm">
                      <Settings className="h-6 w-6 text-blue-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-xl font-bold text-foreground flex items-center space-x-2">
                        <span>Course Details</span>
                        <div className="h-1 w-12 bg-gradient-to-r from-blue-500 to-blue-300 rounded-full" />
                      </h3>
                      <p className="text-sm text-muted-foreground mt-1">Pricing, duration, and enrollment configuration</p>
                    </div>
                    <Badge variant="secondary" className="bg-blue-100 text-blue-700 border-blue-200 px-3 py-1">Settings</Badge>
                  </div>
                </div>
                
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6">
                  <FormField
                    control={form.control}
                    name="level"
                    render={({ field }) => (
                      <FormItem className="space-y-3">
                        <FormLabel className="text-sm font-semibold text-foreground flex items-center space-x-2">
                          <Target className="h-4 w-4 text-primary" />
                          <span>Level</span>
                          <span className="text-red-500">*</span>
                        </FormLabel>
                        <Select onValueChange={(value) => field.onChange(parseInt(value))} value={field.value?.toString()}>
                          <FormControl>
                            <SelectTrigger className="h-12 border-2 rounded-xl bg-gradient-to-r from-background to-background/50 transition-all duration-300 focus:border-primary/50 focus:shadow-lg focus:shadow-primary/10">
                              <SelectValue placeholder="Choose difficulty" />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {Object.entries(CourseLevel).map(([, value]) => (
                              <SelectItem key={value} value={value.toString()}>
                                {getCourseLevelLabel(value)}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="durationHours"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-sm font-medium text-foreground">Duration (Hours) *</FormLabel>
                        <FormControl>
                          <div className="relative">
                            <Clock className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                            <Input 
                              type="number"
                              step="0.5"
                              min="0.5"
                              placeholder="8.0"
                              className="h-11 pl-10"
                              {...field}
                            />
                          </div>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="price"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-sm font-medium text-foreground">Price (USD) *</FormLabel>
                        <FormControl>
                          <div className="relative">
                            <DollarSign className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                            <Input 
                              type="number"
                              step="0.01"
                              min="0"
                              placeholder="99.00"
                              className="h-11 pl-10"
                              {...field}
                            />
                          </div>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="discountPrice"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-sm font-medium text-foreground">Discount Price</FormLabel>
                        <FormControl>
                          <div className="relative">
                            <DollarSign className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                            <Input 
                              type="number"
                              step="0.01"
                              min="0"
                              placeholder="79.00"
                              className="h-11 pl-10"
                              {...field}
                            />
                          </div>
                        </FormControl>
                        <FormDescription className="text-xs">
                          Optional promotional pricing
                        </FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={form.control}
                  name="maxStudents"
                  render={({ field }) => (
                    <FormItem className="max-w-sm">
                      <FormLabel className="text-sm font-medium text-foreground">Maximum Students</FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Users className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                          <Input 
                            type="number"
                            min="1"
                            placeholder="Unlimited"
                            className="h-11 pl-10"
                            {...field}
                          />
                        </div>
                      </FormControl>
                      <FormDescription className="text-xs">
                        Leave empty for unlimited enrollment
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <Separator className="my-8" />

              {/* Learning Content Section */}
              <div className="space-y-6">
                <div className="flex items-center space-x-3">
                  <div className="h-8 w-8 rounded-lg bg-purple-100 flex items-center justify-center">
                    <Target className="h-4 w-4 text-purple-600" />
                  </div>
                  <div>
                    <h3 className="text-lg font-semibold text-foreground">Learning Content</h3>
                    <p className="text-sm text-muted-foreground">Define what students will learn and prerequisites</p>
                  </div>
                </div>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <FormField
                    control={form.control}
                    name="learningObjectives"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-sm font-medium text-foreground">Learning Objectives *</FormLabel>
                        <FormControl>
                          <Textarea 
                            placeholder="• Master React fundamentals&#10;• Build responsive web applications&#10;• Understand state management&#10;• Deploy to production"
                            className="min-h-[140px] resize-none font-mono text-sm"
                            {...field} 
                          />
                        </FormControl>
                        <FormDescription className="text-xs">
                          List what students will achieve after completing this course
                        </FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="prerequisites"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-sm font-medium text-foreground">Prerequisites</FormLabel>
                        <FormControl>
                          <Textarea 
                            placeholder="• Basic HTML/CSS knowledge&#10;• JavaScript fundamentals&#10;• Familiarity with command line&#10;• No prior React experience needed"
                            className="min-h-[140px] resize-none font-mono text-sm"
                            {...field} 
                          />
                        </FormControl>
                        <FormDescription className="text-xs">
                          What knowledge or skills should students have beforehand?
                        </FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              <Separator className="my-8" />

              {/* Publication Settings Section */}
              <div className="space-y-8 relative">
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-r from-orange-500/5 to-transparent rounded-2xl -z-10" />
                  <div className="flex items-center space-x-4 p-6 rounded-2xl border border-orange-500/20 bg-gradient-to-r from-orange-50/50 to-transparent">
                    <div className="h-12 w-12 rounded-2xl bg-gradient-to-br from-orange-100 to-orange-50 flex items-center justify-center border border-orange-200 shadow-sm">
                      <Settings className="h-6 w-6 text-orange-600" />
                    </div>
                    <div className="flex-1">
                      <h3 className="text-xl font-bold text-foreground flex items-center space-x-2">
                        <span>Publication Settings</span>
                        <div className="h-1 w-12 bg-gradient-to-r from-orange-500 to-orange-300 rounded-full" />
                      </h3>
                      <p className="text-sm text-muted-foreground mt-1">Visibility and promotional controls</p>
                    </div>
                    <Badge variant="secondary" className="bg-orange-100 text-orange-700 border-orange-200 px-3 py-1">Publishing</Badge>
                  </div>
                </div>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                  <FormField
                    control={form.control}
                    name="isPublished"
                    render={({ field }) => (
                      <FormItem className="group">
                        <div className="flex items-center justify-between p-6 border-2 border-border rounded-2xl bg-gradient-to-br from-card via-card/80 to-card/60 transition-all duration-300 hover:border-green-500/30 hover:shadow-lg hover:shadow-green-500/10">
                          <div className="space-y-2">
                            <FormLabel className="text-base font-semibold text-foreground flex items-center space-x-3">
                              <div className={`h-3 w-3 rounded-full transition-colors duration-300 ${field.value ? 'bg-green-500 shadow-lg shadow-green-500/30' : 'bg-gray-400'}`} />
                              <span>Published Status</span>
                            </FormLabel>
                            <FormDescription className="text-sm text-muted-foreground">
                              {field.value ? '✨ Course is live and discoverable by students' : '📝 Course is in draft mode and hidden from students'}
                            </FormDescription>
                          </div>
                          <FormControl>
                            <div className="relative">
                              <Switch
                                checked={field.value}
                                onCheckedChange={field.onChange}
                                className="data-[state=checked]:bg-green-500 scale-125 transition-transform duration-300 hover:scale-[1.3]"
                              />
                            </div>
                          </FormControl>
                        </div>
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="isFeatured"
                    render={({ field }) => (
                      <FormItem className="flex items-center justify-between p-4 border border-border rounded-lg bg-card">
                        <div className="space-y-1">
                          <FormLabel className="text-sm font-medium text-foreground">Featured Course</FormLabel>
                          <FormDescription className="text-xs text-muted-foreground">
                            {field.value ? 'Course will be highlighted prominently' : 'Regular course listing'}
                          </FormDescription>
                        </div>
                        <FormControl>
                          <Switch
                            checked={field.value}
                            onCheckedChange={field.onChange}
                            className="data-[state=checked]:bg-blue-500"
                          />
                        </FormControl>
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              {/* Error Display */}
              {error && (
                <div className="flex items-start space-x-3 p-4 bg-red-50 border border-red-200 rounded-lg">
                  <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
                  <div>
                    <h4 className="text-sm font-medium text-red-800">Error updating course</h4>
                    <p className="text-sm text-red-700 mt-1">{error}</p>
                  </div>
                </div>
              )}
            </form>
          </Form>
          </div>
        </div>

        {/* Modern Fluid Footer */}
        <div className="relative border-t-2 border-gradient-to-r from-border/50 to-border bg-gradient-to-r from-muted/40 via-muted/20 to-muted/40 px-8 py-6 mt-8">
          <div className="absolute inset-0 bg-gradient-to-r from-primary/5 via-transparent to-blue-500/5 opacity-50" />
          <DialogFooter className="gap-6 flex justify-between items-center relative">
            <div className="flex items-center space-x-3 text-sm text-muted-foreground">
              <div className="h-2 w-2 rounded-full bg-primary animate-pulse" />
              <span className="font-medium">Changes will be saved automatically</span>
            </div>
            <div className="flex items-center gap-4">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isSubmitting}
                className="h-12 px-8 rounded-xl border-2 transition-all duration-300 hover:border-red-300 hover:text-red-600 hover:bg-red-50 hover:shadow-lg"
              >
                <X className="h-4 w-4 mr-2" />
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isSubmitting}
                className="h-12 px-10 rounded-xl bg-gradient-to-r from-primary via-primary to-primary/90 hover:from-primary/90 hover:via-primary hover:to-primary shadow-lg hover:shadow-xl transition-all duration-300 hover:scale-105"
                onClick={form.handleSubmit(handleSubmit)}
              >
              {isSubmitting ? (
                <>
                  <div className="h-4 w-4 mr-2 animate-spin rounded-full border-2 border-white border-t-transparent" />
                  Updating...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4 mr-2" />
                  Update Course
                </>
              )}
            </Button>
            </div>
          </DialogFooter>
        </div>
      </DialogContent>
    </Dialog>
  );
}