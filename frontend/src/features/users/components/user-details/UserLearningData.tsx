import { useState, useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/components/ui/card'
import { Badge } from '@/shared/components/ui/badge'
import { Button } from '@/shared/components/ui/button'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/shared/components/ui/tabs'
import { Progress } from '@/shared/components/ui/progress'
import { 
  BookOpen, 
  GraduationCap, 
  Clock, 
  CheckCircle, 
  XCircle, 
  TrendingUp,
  Users,
  Star,
  Calendar,
  Award,
  Target,
  Activity,
  Loader2,
  User as UserIcon
} from 'lucide-react'
import { usersApi } from '../../services'
import type { 
  User, 
  UserEnrollment, 
  UserQuizAttempt, 
  UserInstructedCourse,
  EnrollmentStatus 
} from '../../types'
import { formatDate, formatDuration } from '@/lib/utils'

interface UserLearningDataProps {
  user: User;
}

interface LearningStats {
  totalEnrollments: number;
  completedCourses: number;
  inProgressCourses: number;
  averageProgress: number;
  totalQuizAttempts: number;
  averageQuizScore: number;
  passedQuizzes: number;
  failedQuizzes: number;
}

export function UserLearningData({ user }: UserLearningDataProps) {
  const [enrollments, setEnrollments] = useState<UserEnrollment[]>([])
  const [quizAttempts, setQuizAttempts] = useState<UserQuizAttempt[]>([])
  const [instructedCourses, setInstructedCourses] = useState<UserInstructedCourse[]>([])
  const [stats, setStats] = useState<LearningStats | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [activeTab, setActiveTab] = useState('overview')

  useEffect(() => {
    loadLearningData()
  }, [user.id])

  const loadLearningData = async () => {
    try {
      setLoading(true)
      setError(null)

      // Load data based on user role
      const promises: Promise<any>[] = []

      // Always load enrollments and quiz attempts for all users
      promises.push(
        usersApi.getUserEnrollments(user.id, 1, 50),
        usersApi.getUserQuizAttempts(user.id, undefined, undefined, 1, 50)
      )

      // Load instructed courses for instructors
      if (user.role === 2) { // Instructor
        promises.push(usersApi.getUserInstructedCourses(user.id, undefined, 1, 50))
      }

      const results = await Promise.all(promises)
      
      const enrollmentsData = results[0]
      const quizAttemptsData = results[1]
      const instructedCoursesData = user.role === 2 ? results[2] : { courses: [] }

      setEnrollments(enrollmentsData.enrollments)
      setQuizAttempts(quizAttemptsData.quizAttempts)
      setInstructedCourses(instructedCoursesData.courses)

      // Calculate statistics
      const learningStats = calculateStats(
        enrollmentsData.enrollments,
        quizAttemptsData.quizAttempts
      )
      setStats(learningStats)

    } catch (err) {
      console.error('Failed to load learning data:', err)
      setError('Failed to load learning data')
      // Set mock data for development
      setMockData()
    } finally {
      setLoading(false)
    }
  }

  const calculateStats = (enrollments: UserEnrollment[], quizAttempts: UserQuizAttempt[]): LearningStats => {
    const completedCourses = enrollments.filter(e => e.status === 'Completed').length
    const inProgressCourses = enrollments.filter(e => e.status === 'Active').length
    const averageProgress = enrollments.length > 0 
      ? enrollments.reduce((sum, e) => sum + e.progress, 0) / enrollments.length 
      : 0

    const passedQuizzes = quizAttempts.filter(q => q.isPassed).length
    const failedQuizzes = quizAttempts.filter(q => !q.isPassed).length
    const averageQuizScore = quizAttempts.length > 0
      ? quizAttempts.reduce((sum, q) => sum + q.percentage, 0) / quizAttempts.length
      : 0

    return {
      totalEnrollments: enrollments.length,
      completedCourses,
      inProgressCourses,
      averageProgress,
      totalQuizAttempts: quizAttempts.length,
      averageQuizScore,
      passedQuizzes,
      failedQuizzes
    }
  }

  const setMockData = () => {
    const mockEnrollments: UserEnrollment[] = [
      {
        id: 1,
        courseId: 101,
        courseName: 'react-fundamentals',
        courseTitle: 'React Fundamentals',
        instructorName: 'John Smith',
        status: 'Active' as EnrollmentStatus,
        progress: 75,
        enrolledAt: '2024-01-15T10:00:00Z',
        lastAccessedAt: '2024-01-20T14:30:00Z',
        totalLessons: 20,
        completedLessons: 15
      },
      {
        id: 2,
        courseId: 102,
        courseName: 'javascript-advanced',
        courseTitle: 'Advanced JavaScript',
        instructorName: 'Sarah Johnson',
        status: 'Completed' as EnrollmentStatus,
        progress: 100,
        enrolledAt: '2023-12-01T10:00:00Z',
        completedAt: '2024-01-10T16:45:00Z',
        lastAccessedAt: '2024-01-10T16:45:00Z',
        totalLessons: 15,
        completedLessons: 15
      }
    ]

    const mockQuizAttempts: UserQuizAttempt[] = [
      {
        id: 1,
        quizId: 201,
        quizTitle: 'React Components Quiz',
        courseId: 101,
        courseName: 'React Fundamentals',
        score: 85,
        maxScore: 100,
        percentage: 85,
        isPassed: true,
        attemptedAt: '2024-01-18T10:00:00Z',
        completedAt: '2024-01-18T10:30:00Z',
        timeSpent: 1800,
        totalQuestions: 10,
        correctAnswers: 8
      },
      {
        id: 2,
        quizId: 202,
        quizTitle: 'JavaScript Functions Quiz',
        courseId: 102,
        courseName: 'Advanced JavaScript',
        score: 92,
        maxScore: 100,
        percentage: 92,
        isPassed: true,
        attemptedAt: '2024-01-05T14:00:00Z',
        completedAt: '2024-01-05T14:25:00Z',
        timeSpent: 1500,
        totalQuestions: 12,
        correctAnswers: 11
      }
    ]

    setEnrollments(mockEnrollments)
    setQuizAttempts(mockQuizAttempts)
    setStats(calculateStats(mockEnrollments, mockQuizAttempts))
  }

  const getStatusBadge = (status: EnrollmentStatus) => {
    const variants = {
      'Active': 'default',
      'Completed': 'secondary',
      'Dropped': 'destructive',
      'Suspended': 'outline'
    } as const

    const icons = {
      'Active': <Activity className="w-3 h-3" />,
      'Completed': <CheckCircle className="w-3 h-3" />,
      'Dropped': <XCircle className="w-3 h-3" />,
      'Suspended': <Clock className="w-3 h-3" />
    }

    return (
      <Badge variant={variants[status]} className="flex items-center gap-1">
        {icons[status]}
        {status}
      </Badge>
    )
  }

  if (loading) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center justify-center py-8">
            <div className="flex items-center gap-2">
              <Loader2 className="h-6 w-6 animate-spin" />
              <span>Loading learning data...</span>
            </div>
          </div>
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center py-8">
            <p className="text-lg font-medium">Failed to load learning data</p>
            <p className="text-muted-foreground">{error}</p>
            <Button onClick={loadLearningData} className="mt-4">
              Try Again
            </Button>
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    <div className="space-y-8">
      {/* Learning Statistics Overview with improved design */}
      {stats && (
        <div className="space-y-4">
          <div>
            <h3 className="text-lg font-semibold">Learning Overview</h3>
            <p className="text-sm text-muted-foreground">Key metrics and achievements</p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4">
            <Card className="border-l-4 border-l-blue-500 hover:shadow-md transition-shadow">
              <CardContent className="p-6">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-100 rounded-lg">
                    <BookOpen className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">Total Enrollments</p>
                    <p className="text-2xl font-bold">{stats.totalEnrollments}</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card className="border-l-4 border-l-green-500 hover:shadow-md transition-shadow">
              <CardContent className="p-6">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-green-100 rounded-lg">
                    <GraduationCap className="h-5 w-5 text-green-600" />
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">Completed Courses</p>
                    <p className="text-2xl font-bold">{stats.completedCourses}</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card className="border-l-4 border-l-orange-500 hover:shadow-md transition-shadow">
              <CardContent className="p-6">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-orange-100 rounded-lg">
                    <Target className="h-5 w-5 text-orange-600" />
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">Average Progress</p>
                    <p className="text-2xl font-bold">{Math.round(stats.averageProgress)}%</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card className="border-l-4 border-l-purple-500 hover:shadow-md transition-shadow">
              <CardContent className="p-6">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-purple-100 rounded-lg">
                    <Award className="h-5 w-5 text-purple-600" />
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">Avg Quiz Score</p>
                    <p className="text-2xl font-bold">{Math.round(stats.averageQuizScore)}%</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      )}

      {/* Detailed Learning Data with enhanced design */}
      <Card className="shadow-sm">
        <CardHeader className="pb-3">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <TrendingUp className="h-5 w-5 text-primary" />
              <CardTitle>Detailed Learning Data</CardTitle>
            </div>
          </div>
          <CardDescription>
            Comprehensive view of {user.fullName}'s learning activities and progress
          </CardDescription>
        </CardHeader>
        <CardContent className="pt-0">
          <Tabs value={activeTab} onValueChange={setActiveTab}>
            <TabsList className={`grid w-full ${user.role === 2 ? 'grid-cols-3' : 'grid-cols-2'} mb-6`}>
              <TabsTrigger value="enrollments" className="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
                <BookOpen className="h-4 w-4 mr-2" />
                Enrollments
              </TabsTrigger>
              <TabsTrigger value="quizzes" className="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
                <Award className="h-4 w-4 mr-2" />
                Quiz Performance
              </TabsTrigger>
              {user.role === 2 && (
                <TabsTrigger value="teaching" className="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
                  <Users className="h-4 w-4 mr-2" />
                  Teaching
                </TabsTrigger>
              )}
            </TabsList>

            <TabsContent value="enrollments" className="space-y-6 mt-6">
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <h3 className="text-lg font-semibold">Course Enrollments</h3>
                  {enrollments.length > 0 && (
                    <span className="text-sm text-muted-foreground">
                      {enrollments.length} course{enrollments.length !== 1 ? 's' : ''}
                    </span>
                  )}
                </div>
                {enrollments.length === 0 ? (
                  <div className="text-center py-8">
                    <BookOpen className="h-12 w-12 text-muted-foreground/50 mx-auto mb-4" />
                    <p className="text-lg font-medium text-muted-foreground">No enrollments found</p>
                    <p className="text-sm text-muted-foreground">This user hasn't enrolled in any courses yet.</p>
                  </div>
                ) : (
                  <div className="space-y-4">
                    {enrollments.map((enrollment) => (
                      <Card key={enrollment.id} className="hover:shadow-md transition-shadow border-l-4 border-l-blue-200">
                        <CardContent className="p-6">
                          <div className="flex items-start justify-between">
                            <div className="space-y-3 flex-1">
                              <div className="flex items-center gap-3">
                                <h4 className="font-semibold text-lg">{enrollment.courseTitle}</h4>
                                {getStatusBadge(enrollment.status)}
                              </div>
                              <p className="text-sm text-muted-foreground flex items-center gap-1">
                                <UserIcon className="w-4 h-4" />
                                Instructor: {enrollment.instructorName}
                              </p>
                              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-muted-foreground">
                                <span className="flex items-center gap-2">
                                  <Calendar className="w-4 h-4" />
                                  Enrolled: {formatDate(enrollment.enrolledAt)}
                                </span>
                                {enrollment.lastAccessedAt && (
                                  <span className="flex items-center gap-2">
                                    <Clock className="w-4 h-4" />
                                    Last accessed: {formatDate(enrollment.lastAccessedAt)}
                                  </span>
                                )}
                              </div>
                              <div className="space-y-3">
                                <div className="flex justify-between items-center text-sm">
                                  <span className="font-medium">Course Progress</span>
                                  <span className="font-bold text-primary">{enrollment.progress}%</span>
                                </div>
                                <Progress value={enrollment.progress} className="h-3" />
                                <div className="flex justify-between text-xs text-muted-foreground">
                                  <span>{enrollment.completedLessons} of {enrollment.totalLessons} lessons completed</span>
                                  <span>{enrollment.totalLessons - enrollment.completedLessons} remaining</span>
                                </div>
                              </div>
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                )}
              </div>
            </TabsContent>

            <TabsContent value="quizzes" className="space-y-6 mt-6">
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <h3 className="text-lg font-semibold">Quiz Performance</h3>
                  {quizAttempts.length > 0 && (
                    <span className="text-sm text-muted-foreground">
                      {quizAttempts.length} attempt{quizAttempts.length !== 1 ? 's' : ''}
                    </span>
                  )}
                </div>
                {quizAttempts.length === 0 ? (
                  <div className="text-center py-8">
                    <Award className="h-12 w-12 text-muted-foreground/50 mx-auto mb-4" />
                    <p className="text-lg font-medium text-muted-foreground">No quiz attempts found</p>
                    <p className="text-sm text-muted-foreground">This user hasn't attempted any quizzes yet.</p>
                  </div>
                ) : (
                  <div className="space-y-4">
                    {quizAttempts.map((attempt) => (
                      <Card key={attempt.id} className={`hover:shadow-md transition-shadow border-l-4 ${
                        attempt.isPassed ? 'border-l-green-200' : 'border-l-red-200'
                      }`}>
                        <CardContent className="p-6">
                          <div className="space-y-4">
                            <div className="flex items-center justify-between">
                              <div>
                                <h4 className="font-semibold text-lg">{attempt.quizTitle}</h4>
                                <p className="text-sm text-muted-foreground">
                                  Course: {attempt.courseName}
                                </p>
                              </div>
                              <Badge variant={attempt.isPassed ? 'default' : 'destructive'} className="text-sm">
                                {attempt.isPassed ? (
                                  <CheckCircle className="w-4 h-4 mr-1" />
                                ) : (
                                  <XCircle className="w-4 h-4 mr-1" />
                                )}
                                {attempt.isPassed ? 'Passed' : 'Failed'}
                              </Badge>
                            </div>
                            
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                              <div className="bg-muted/30 p-3 rounded-lg">
                                <p className="text-xs text-muted-foreground font-medium">Score</p>
                                <p className="text-sm font-bold">{attempt.score}/{attempt.maxScore}</p>
                                <p className="text-xs text-primary font-medium">({attempt.percentage}%)</p>
                              </div>
                              <div className="bg-muted/30 p-3 rounded-lg">
                                <p className="text-xs text-muted-foreground font-medium">Correct Answers</p>
                                <p className="text-sm font-bold">{attempt.correctAnswers}/{attempt.totalQuestions}</p>
                                <p className="text-xs text-muted-foreground">questions</p>
                              </div>
                              <div className="bg-muted/30 p-3 rounded-lg">
                                <p className="text-xs text-muted-foreground font-medium">Time Spent</p>
                                <p className="text-sm font-bold">{formatDuration(attempt.timeSpent)}</p>
                                <p className="text-xs text-muted-foreground">duration</p>
                              </div>
                              <div className="bg-muted/30 p-3 rounded-lg">
                                <p className="text-xs text-muted-foreground font-medium">Attempted</p>
                                <p className="text-sm font-bold">{formatDate(attempt.attemptedAt)}</p>
                                <p className="text-xs text-muted-foreground">date</p>
                              </div>
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                )}
              </div>
            </TabsContent>

            {user.role === 2 && (
              <TabsContent value="teaching" className="space-y-6 mt-6">
                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <h3 className="text-lg font-semibold">Instructed Courses</h3>
                    {instructedCourses.length > 0 && (
                      <span className="text-sm text-muted-foreground">
                        {instructedCourses.length} course{instructedCourses.length !== 1 ? 's' : ''}
                      </span>
                    )}
                  </div>
                  {instructedCourses.length === 0 ? (
                    <div className="text-center py-8">
                      <Users className="h-12 w-12 text-muted-foreground/50 mx-auto mb-4" />
                      <p className="text-lg font-medium text-muted-foreground">No instructed courses found</p>
                      <p className="text-sm text-muted-foreground">This instructor hasn't created any courses yet.</p>
                    </div>
                  ) : (
                    <div className="space-y-4">
                      {instructedCourses.map((course) => (
                        <Card key={course.id} className="hover:shadow-md transition-shadow border-l-4 border-l-indigo-200">
                          <CardContent className="p-6">
                            <div className="space-y-4">
                              <div className="flex items-start justify-between">
                                <div className="flex-1">
                                  <h4 className="font-semibold text-lg">{course.title}</h4>
                                  <p className="text-sm text-muted-foreground mt-1">{course.description}</p>
                                  <div className="flex items-center gap-2 mt-2">
                                    <span className="text-xs bg-muted px-2 py-1 rounded-full">
                                      {course.category}
                                    </span>
                                    <span className="text-xs text-muted-foreground">
                                      ${course.price}
                                    </span>
                                  </div>
                                </div>
                                <Badge variant={course.isPublished ? 'default' : 'outline'} className="ml-4">
                                  {course.isPublished ? 'Published' : 'Draft'}
                                </Badge>
                              </div>
                              
                              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                                <div className="flex items-center gap-2 bg-muted/30 p-3 rounded-lg">
                                  <Users className="w-4 h-4 text-blue-600" />
                                  <div>
                                    <p className="text-xs text-muted-foreground font-medium">Students</p>
                                    <p className="text-sm font-bold">{course.totalStudents}</p>
                                  </div>
                                </div>
                                <div className="flex items-center gap-2 bg-muted/30 p-3 rounded-lg">
                                  <Star className="w-4 h-4 text-yellow-600" />
                                  <div>
                                    <p className="text-xs text-muted-foreground font-medium">Rating</p>
                                    <p className="text-sm font-bold">{course.averageRating.toFixed(1)}/5</p>
                                  </div>
                                </div>
                                <div className="flex items-center gap-2 bg-muted/30 p-3 rounded-lg">
                                  <BookOpen className="w-4 h-4 text-green-600" />
                                  <div>
                                    <p className="text-xs text-muted-foreground font-medium">Lessons</p>
                                    <p className="text-sm font-bold">{course.totalLessons}</p>
                                  </div>
                                </div>
                                <div className="bg-muted/30 p-3 rounded-lg">
                                  <p className="text-xs text-muted-foreground font-medium">Created</p>
                                  <p className="text-sm font-bold">{formatDate(course.createdAt)}</p>
                                </div>
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      ))}
                    </div>
                  )}
                </div>
              </TabsContent>
            )}
          </Tabs>
        </CardContent>
      </Card>
    </div>
  )
}
