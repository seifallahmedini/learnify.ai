import { Card, CardContent, CardHeader } from '@/shared/components/ui/card';

export function QuizDetailsSkeleton() {
  return (
    <div className="space-y-6">
      {/* Header Skeleton */}
      <div className="space-y-4">
        <div className="h-10 w-32 bg-gray-200 animate-pulse rounded" />
        <div className="h-8 w-3/4 bg-gray-200 animate-pulse rounded" />
        <div className="h-4 w-full bg-gray-200 animate-pulse rounded" />
        <div className="h-4 w-2/3 bg-gray-200 animate-pulse rounded" />
      </div>

      {/* Main Content Skeleton */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          <Card>
            <CardHeader>
              <div className="h-6 w-48 bg-gray-200 animate-pulse rounded" />
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="h-4 w-full bg-gray-200 animate-pulse rounded" />
              <div className="h-4 w-5/6 bg-gray-200 animate-pulse rounded" />
              <div className="h-4 w-4/6 bg-gray-200 animate-pulse rounded" />
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <div className="h-6 w-56 bg-gray-200 animate-pulse rounded" />
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {[...Array(3)].map((_, index) => (
                  <div key={index} className="h-32 bg-gray-200 animate-pulse rounded" />
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <div className="h-5 w-32 bg-gray-200 animate-pulse rounded" />
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                <div className="h-16 bg-gray-200 animate-pulse rounded" />
                <div className="h-16 bg-gray-200 animate-pulse rounded" />
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}

