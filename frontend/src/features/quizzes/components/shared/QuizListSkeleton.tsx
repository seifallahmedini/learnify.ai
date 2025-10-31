import { Card, CardContent, CardHeader } from '@/shared/components/ui/card';

export function QuizListSkeleton() {
  return (
    <div className="space-y-4">
      {[...Array(3)].map((_, index) => (
        <Card key={index}>
          <CardHeader>
            <div className="flex items-start justify-between">
              <div className="flex-1 space-y-3">
                <div className="h-6 w-3/4 bg-gray-200 animate-pulse rounded" />
                <div className="h-4 w-full bg-gray-200 animate-pulse rounded" />
                <div className="h-4 w-2/3 bg-gray-200 animate-pulse rounded" />
                <div className="flex items-center gap-3">
                  <div className="h-5 w-20 bg-gray-200 animate-pulse rounded" />
                  <div className="h-5 w-16 bg-gray-200 animate-pulse rounded" />
                  <div className="h-5 w-24 bg-gray-200 animate-pulse rounded" />
                </div>
              </div>
              <div className="h-9 w-20 bg-gray-200 animate-pulse rounded ml-4" />
            </div>
          </CardHeader>
        </Card>
      ))}
    </div>
  );
}

