import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/components/ui/card"
import { Button } from "@/shared/components/ui/button"
import { Badge } from "@/shared/components/ui/badge"
import { Input } from "@/shared/components/ui/input"
import { Clock, Users, Star, Search, Filter } from "lucide-react"

const courses = [
  {
    id: 1,
    title: "Advanced React Patterns",
    description: "Master advanced React concepts including custom hooks, context patterns, and performance optimization.",
    instructor: "Sarah Johnson",
    duration: "8 hours",
    students: 1234,
    rating: 4.8,
    level: "Advanced",
    price: "$89",
    image: "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=300&h=200&fit=crop"
  },
  {
    id: 2,
    title: "TypeScript Fundamentals",
    description: "Learn TypeScript from basics to advanced features for better JavaScript development.",
    instructor: "Mike Chen",
    duration: "12 hours",
    students: 2156,
    rating: 4.9,
    level: "Beginner",
    price: "$69",
    image: "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=300&h=200&fit=crop"
  },
  {
    id: 3,
    title: "Node.js Backend Development",
    description: "Build scalable backend applications with Node.js, Express, and MongoDB.",
    instructor: "Alex Rodriguez",
    duration: "15 hours",
    students: 987,
    rating: 4.7,
    level: "Intermediate",
    price: "$129",
    image: "https://images.unsplash.com/photo-1627398242454-45a1465c2479?w=300&h=200&fit=crop"
  },
  {
    id: 4,
    title: "CSS Grid & Flexbox Mastery",
    description: "Master modern CSS layout techniques for responsive web design.",
    instructor: "Emma Wilson",
    duration: "6 hours",
    students: 1567,
    rating: 4.6,
    level: "Intermediate",
    price: "$49",
    image: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=200&fit=crop"
  },
  {
    id: 5,
    title: "GraphQL Complete Guide",
    description: "Learn GraphQL from scratch including queries, mutations, and real-world applications.",
    instructor: "David Kim",
    duration: "10 hours",
    students: 743,
    rating: 4.8,
    level: "Advanced",
    price: "$99",
    image: "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=300&h=200&fit=crop"
  },
  {
    id: 6,
    title: "JavaScript ES6+ Features",
    description: "Explore modern JavaScript features and best practices for contemporary development.",
    instructor: "Lisa Park",
    duration: "7 hours",
    students: 2890,
    rating: 4.7,
    level: "Beginner",
    price: "$59",
    image: "https://images.unsplash.com/photo-1579468118864-1b9ea3c0db4a?w=300&h=200&fit=crop"
  }
]

function getLevelColor(level: string) {
  switch (level) {
    case 'Beginner':
      return 'bg-green-100 text-green-800'
    case 'Intermediate':
      return 'bg-yellow-100 text-yellow-800'
    case 'Advanced':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

export function CoursesPage() {
  return (
    <div className="flex flex-1 flex-col gap-4 p-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Courses</h1>
          <p className="text-muted-foreground">
            Discover and enroll in courses to expand your skills
          </p>
        </div>
        <Button>Create Course</Button>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input placeholder="Search courses..." className="pl-8" />
        </div>
        <Button variant="outline">
          <Filter className="mr-2 h-4 w-4" />
          Filters
        </Button>
      </div>

      {/* Courses Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        {courses.map((course) => (
          <Card key={course.id} className="overflow-hidden">
            <div className="aspect-video bg-muted">
              <img 
                src={course.image} 
                alt={course.title}
                className="w-full h-full object-cover"
              />
            </div>
            <CardHeader>
              <div className="flex items-start justify-between">
                <div className="space-y-1">
                  <CardTitle className="text-lg">{course.title}</CardTitle>
                  <p className="text-sm text-muted-foreground">by {course.instructor}</p>
                </div>
                <Badge className={getLevelColor(course.level)} variant="secondary">
                  {course.level}
                </Badge>
              </div>
              <CardDescription className="line-clamp-2">
                {course.description}
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex items-center justify-between text-sm text-muted-foreground mb-4">
                <div className="flex items-center">
                  <Clock className="mr-1 h-4 w-4" />
                  {course.duration}
                </div>
                <div className="flex items-center">
                  <Users className="mr-1 h-4 w-4" />
                  {course.students.toLocaleString()}
                </div>
                <div className="flex items-center">
                  <Star className="mr-1 h-4 w-4 fill-yellow-400 text-yellow-400" />
                  {course.rating}
                </div>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-2xl font-bold">{course.price}</span>
                <Button>Enroll Now</Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  )
}