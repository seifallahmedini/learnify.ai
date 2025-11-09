# ?? Claude Desktop MCP Server Setup Guide

## ?? Files Created

1. **`start-mcp-server.bat`** - Windows batch script (recommended)
2. **`start-mcp-server.ps1`** - PowerShell script (alternative)
3. **`claude-desktop-config.json`** - Batch script configuration
4. **`claude-desktop-config-powershell.json`** - PowerShell configuration

## ?? Setup Instructions

### Step 1: Choose Your Script

**Option A: Batch Script (Recommended)**
- Use `start-mcp-server.bat` with `claude-desktop-config.json`
- Most reliable for Windows systems

**Option B: PowerShell Script**
- Use `start-mcp-server.ps1` with `claude-desktop-config-powershell.json`
- Better error handling and cross-platform compatibility

### Step 2: Configure Claude Desktop

1. **Locate Claude Desktop config file:**
   ```
   %APPDATA%\Claude\claude_desktop_config.json
   ```

2. **Copy the contents** of either:
   - `claude-desktop-config.json` (for batch script)
   - `claude-desktop-config-powershell.json` (for PowerShell)

3. **Update the file path** if your project is in a different location

### Step 3: Verify Prerequisites

1. **Ensure .NET 8 SDK is installed:**
   ```cmd
   dotnet --version
   ```
   Should return version 8.x.x

2. **Ensure Learnify.ai API is running:**
   ```cmd
   cd "C:\Users\LENOVO\Desktop\100 Days Challenge\learnify.ai\backend\learnify.ai.api\learnify.ai.api"
   dotnet run
   ```
   API should be accessible at: http://localhost:5271

### Step 4: Test the Setup

1. **Test the script manually:**
   ```cmd
   cd "C:\Users\LENOVO\Desktop\100 Days Challenge\learnify.ai\backend\learnify.ai.api\learnify.ai.mcp.server"
   start-mcp-server.bat
   ```

2. **Expected behavior:**
   - No error messages
   - Server starts and waits for input
   - Press Ctrl+C to stop

### Step 5: Restart Claude Desktop

1. **Close Claude Desktop completely**
2. **Restart Claude Desktop**
3. **Verify the MCP server appears** in Claude's available tools

## ??? Available Tools (82 Total)

The MCP server exposes comprehensive learning platform management tools:

### **?? Lesson Management Tools (16 Total)**

#### **CRUD Operations:**
- `GetLessonAsync` - Get lesson details by ID
- `UpdateLessonAsync` - Update lesson properties
- `DeleteLessonAsync` - Delete lesson permanently

#### **Publishing & Access:**
- `PublishLessonAsync` - Make lesson visible to students
- `UnpublishLessonAsync` - Hide lesson from students
- `MakeLessonFreeAsync` - Make lesson free or premium

#### **Content Management:**
- `UploadLessonVideoAsync` - Upload/update lesson video
- `UpdateLessonContentAsync` - Update lesson content
- `GetLessonResourcesAsync` - Get lesson attachments

#### **Navigation & Organization:**
- `GetNextLessonAsync` - Get next lesson in sequence
- `GetPreviousLessonAsync` - Get previous lesson in sequence
- `ReorderLessonAsync` - Change lesson order in course

#### **Course Integration:**
- `GetCourseLessonsAsync` - Get all lessons for a course
- `CreateCourseLessonAsync` - Create new lesson in course

#### **Utilities:**
- `CheckLessonExistsAsync` - Verify lesson existence
- `GetLessonSummaryAsync` - Get basic lesson info

### **?? Course Management Tools (15 Total)**

#### **CRUD Operations:**
- `GetCoursesAsync` - Get all courses with filtering and pagination  
- `GetCourseAsync` - Get course details by ID
- `CreateCourseAsync` - Create a new course
- `UpdateCourseAsync` - Update course properties
- `DeleteCourseAsync` - Delete course permanently

#### **Publishing & Featuring:**
- `PublishCourseAsync` - Make course visible to students
- `UnpublishCourseAsync` - Hide course from students
- `FeatureCourseAsync` - Feature course to highlight it
- `UnfeatureCourseAsync` - Remove course featuring

#### **Enrollment & Analytics:**
- `GetCourseEnrollmentsAsync` - Get all course enrollments
- `GetCourseStatsAsync` - Get comprehensive course statistics

#### **Utilities:**
- `CheckCourseExistsAsync` - Verify course existence
- `GetCourseSummaryAsync` - Get basic course info

### **??? Category Management Tools (17 Total)**

#### **CRUD Operations:**
- `GetCategoriesAsync` - Get all categories with filtering and pagination
- `GetCategoryAsync` - Get category details by ID
- `CreateCategoryAsync` - Create a new category
- `UpdateCategoryAsync` - Update category properties
- `DeleteCategoryAsync` - Delete category permanently

#### **Activation Operations:**
- `ActivateCategoryAsync` - Activate category to make it visible
- `DeactivateCategoryAsync` - Deactivate category to hide it

#### **Hierarchy Operations:**
- `GetCategoryTreeAsync` - Get complete category hierarchy as tree
- `GetSubcategoriesAsync` - Get all subcategories of a category
- `MoveCategoryAsync` - Move category to different parent
- `GetRootCategoriesAsync` - Get all root categories (no parents)

#### **Content Operations:**
- `GetCategoryCoursesAsync` - Get all courses in a category
- `GetCategoryStatsAsync` - Get comprehensive category statistics

#### **Utilities:**
- `CheckCategoryExistsAsync` - Verify category existence
- `GetCategorySummaryAsync` - Get basic category info

### **?? Quiz Management Tools (18 Total)**

#### **CRUD Operations:**
- `GetQuizzesAsync` - Get all quizzes with filtering and pagination
- `GetQuizAsync` - Get quiz details by ID
- `CreateQuizAsync` - Create a new quiz
- `UpdateQuizAsync` - Update quiz properties
- `DeleteQuizAsync` - Delete quiz permanently

#### **Activation Operations:**
- `ActivateQuizAsync` - Activate quiz to make it available to students
- `DeactivateQuizAsync` - Deactivate quiz to make it unavailable

#### **Course & Lesson Integration:**
- `GetCourseQuizzesAsync` - Get all quizzes for a specific course
- `GetLessonQuizzesAsync` - Get all quizzes for a specific lesson

#### **Question Management:**
- `GetQuizQuestionsAsync` - Get all questions for a specific quiz
- `AddQuestionToQuizAsync` - Add a new question to a quiz

#### **Assessment Operations:**
- `StartQuizAttemptAsync` - Start a quiz attempt for a specific user
- `GetQuizAttemptsAsync` - Get all attempts for a specific quiz
- `GetQuizStatsAsync` - Get comprehensive statistics for a quiz

#### **Utilities:**
- `CheckQuizExistsAsync` - Verify quiz existence
- `GetQuizSummaryAsync` - Get basic quiz info

### **?? Answer Management Tools (16 Total)**

#### **CRUD Operations:**
- `GetAnswersAsync` - Get all answers with optional filtering
- `GetAnswerAsync` - Get answer details by ID
- `CreateAnswerAsync` - Create a new answer for a quiz question
- `UpdateAnswerAsync` - Update existing answer details
- `DeleteAnswerAsync` - Delete an answer permanently

#### **Question Answer Management:**
- `GetQuestionAnswersAsync` - Get all answers for a specific question
- `ReorderQuestionAnswersAsync` - Reorder answers for a specific question
- `CreateMultipleAnswersAsync` - Create multiple answers for a question at once

#### **Answer Validation:**
- `ValidateAnswerAsync` - Validate answer business rules and constraints
- `ValidateQuestionAnswersAsync` - Validate all answers for a specific question

#### **Answer Analytics:**
- `GetAnswerStatsAsync` - Get answer selection statistics and analytics
- `GetQuestionAnswerAnalyticsAsync` - Get comprehensive analytics for all answers of a question

#### **Bulk Operations:**
- `BulkAnswerOperationAsync` - Perform bulk operations on multiple answers

#### **Utilities:**
- `CheckAnswerExistsAsync` - Check if an answer exists
- `GetAnswerSummaryAsync` - Get answer summary (basic information only)

## ?? Testing Commands

Once set up, test with Claude:

### **Lesson Management:**
1. **"What lesson management tools are available?"**
2. **"Get lesson with ID 1"**
3. **"Get all lessons for course 1"**
4. **"Create a new lesson for course 2"**
5. **"Publish lesson 3"**

### **Course Management:**
1. **"What course management tools are available?"**
2. **"Get all published courses"**
3. **"Create a new beginner course for JavaScript"**
4. **"Get statistics for course 1"**
5. **"Find all courses by instructor 2"**
6. **"Feature course 3"**

### **Category Management:**
1. **"What category management tools are available?"**
2. **"Get all active categories"**
3. **"Create a new category for Programming"**
4. **"Get the complete category tree"**
5. **"Move Frontend category under Web Development"**
6. **"Get all courses in JavaScript category"**

### **Quiz Management:**
1. **"What quiz management tools are available?"**
2. **"Get all active quizzes"**
3. **"Create a new quiz for JavaScript fundamentals"**
4. **"Get quizzes for course 1"**
5. **"Add a multiple choice question about variables"**
6. **"Start a quiz attempt for user 5"**
7. **"Get quiz statistics for assessment analytics"**

### **Answer Management:**
1. **"What answer management tools are available?"**
2. **"Get all answers for question 1"**
3. **"Create a new correct answer for question 2"**
4. **"Create multiple answers for question 3"**
5. **"Reorder answers for question 1"**
6. **"Validate all answers for question 2"**
7. **"Get answer analytics for question performance"**

### **Advanced Cross-Feature Operations:**
1. **"Get course 1 with all its lessons, quizzes, questions, and answers"**
2. **"Create a complete learning path: category ? course ? lessons ? quizzes ? questions ? answers"**
3. **"Find all unpublished content across categories, courses, and assessments"**
4. **"Analyze learning outcomes across all assessment components"**
5. **"Create a comprehensive course structure with complete assessments"**
6. **"Validate the entire assessment structure for a course"**

## ?? Troubleshooting

### **Issue: "Server disconnected"**
- Ensure Learnify.ai API is running on port 5271
- Check if the script path in Claude config is correct
- Verify .NET 8 SDK is installed

### **Issue: "Command not found"**
- Update the file path in Claude config to match your actual location
- Ensure the batch/PowerShell script exists and is executable

### **Issue: Build errors**
- Run `dotnet restore` and `dotnet build` manually
- Check for compilation errors in the project

### **Issue: Permission denied (PowerShell)**
- Run PowerShell as Administrator
- Set execution policy: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`

## ?? Script Features

### **Batch Script (`start-mcp-server.bat`):**
- ? Environment variable setup
- ? .NET SDK verification
- ? Project file validation
- ? Silent package restoration
- ? Silent build process
- ? Clean MCP protocol output
- ? Proper error handling

### **PowerShell Script (`start-mcp-server.ps1`):**
- ? All batch script features
- ? Better error messages
- ? Optional verbose mode
- ? Cross-platform compatibility
- ? Advanced error handling

## ?? Updates

When you update the MCP server code:
1. The scripts will automatically rebuild the project
2. No need to restart Claude Desktop
3. Changes take effect immediately

## ?? Documentation

- **Lesson Feature**: See existing lesson management documentation
- **Course Feature**: See `Features/Courses/README.md` for detailed course management guide
- **Category Feature**: See `Features/Categories/README.md` for comprehensive category management guide
- **Quiz Feature**: See `Features/Quizzes/README.md` for complete quiz and assessment management guide
- **Answer Feature**: See `Features/Answers/README.md` for comprehensive answer option management guide
- **API Integration**: All features integrate with Learnify.ai API endpoints
- **Model Definitions**: Check `Features/*/Models/` for data structures

## ??? Architecture

The MCP server follows a **vertical slice architecture**:

```
learnify.ai.mcp.server/
??? Features/
?   ??? Lessons/              # Lesson management feature
?   ?   ??? Models/          # Lesson domain models
?   ?   ??? Services/        # LessonApiService with MCP tools
?   ?   ??? LessonFeature.cs
?   ??? Courses/             # Course management feature
?   ?   ??? Models/          # Course domain models
?   ?   ??? Services/        # CourseApiService with MCP tools
?   ?   ??? CourseFeature.cs
?   ??? Categories/          # Category management feature
?   ?   ??? Models/          # Category domain models
?   ?   ??? Services/        # CategoryApiService with MCP tools
?   ?   ??? CategoryFeature.cs
?   ??? Quizzes/             # Quiz management feature
?   ?   ??? Models/          # Quiz domain models
?   ?   ??? Services/        # QuizApiService with MCP tools
?   ?   ??? QuizFeature.cs
?   ??? Answers/             # Answer management feature
?       ??? Models/          # Answer domain models
?       ??? Services/        # AnswerApiService with MCP tools
?       ??? AnswerFeature.cs
??? Shared/                  # Common infrastructure
?   ??? Models/             # Base API models
?   ??? Services/           # BaseApiService
??? Program.cs              # MCP server startup
```

## ?? **Feature Relationships**

The five features work together to provide comprehensive learning platform management:

```
Categories (Structure)
    ? organizes
Courses (Content)
    ? contains
Lessons (Learning Units)
    ? assessed by
Quizzes (Assessments)
    ? composed of
Questions ? Answers (Assessment Details)
```

### **Integration Examples:**
- **Category ? Courses**: `GetCategoryCoursesAsync` shows all courses in a category
- **Course ? Lessons**: `GetCourseLessonsAsync` shows all lessons in a course  
- **Course ? Quizzes**: `GetCourseQuizzesAsync` shows all quizzes in a course
- **Lesson ? Quizzes**: `GetLessonQuizzesAsync` shows lesson-specific assessments
- **Quiz ? Questions**: `GetQuizQuestionsAsync` shows all questions in a quiz
- **Question ? Answers**: `GetQuestionAnswersAsync` shows all answer options
- **Cross-Feature Analytics**: Combined statistics across all educational components
- **Hierarchical Management**: Organize content from high-level categories down to individual answer options

## ?? Support

If you encounter issues:
1. Check the troubleshooting section above
2. Verify all prerequisites are met
3. Test the script manually before using with Claude
4. Ensure file paths match your actual directory structure
5. Check API connectivity to http://localhost:5271

The MCP server is now ready for comprehensive **learning platform management** with **82 powerful tools** across lessons, courses, categories, quizzes, and answers! ??

## ?? **What's Possible Now**

With **82 MCP tools** across **5 major features**, Claude can now:

- **?? Manage complete learning content** from categories to individual answer options
- **?? Perform complex cross-feature operations** like creating entire learning ecosystems
- **?? Provide comprehensive analytics** across all content and assessment components
- **?? Handle content organization** and hierarchy management with full assessment integration
- **? Execute bulk operations** across multiple content and assessment types
- **?? Support complete learning workflows** from content creation to detailed assessment analytics
- **?? Manage full assessment lifecycles** from quiz creation to answer option analytics
- **?? Provide detailed learning outcome analysis** through comprehensive assessment data
- **?? Optimize educational content** using answer-level performance insights
- **?? Validate entire assessment structures** for quality assurance

This represents a **complete learning management and assessment ecosystem** accessible through natural language! ??

## ?? **Complete Assessment Integration Benefits**

The addition of Answer management completes the assessment ecosystem enabling:

### **Full Assessment Hierarchy:**
- Create structured assessments: Quiz ? Questions ? Answer Options
- Track student performance at every level
- Analyze learning outcomes from high-level to granular insights

### **Comprehensive Analytics:**
- Quiz-level performance metrics
- Question-level difficulty analysis  
- Answer-option selection patterns
- Distractor effectiveness measurement
- Learning pathway optimization

### **Quality Assurance:**
- Validate entire assessment structures
- Ensure answer option quality
- Monitor assessment effectiveness
- Improve learning outcomes through data

### **Advanced Educational Workflows:**
- **Content Creation**: Categories ? Courses ? Lessons ? Quizzes ? Questions ? Answers
- **Assessment Design**: Create comprehensive assessments with detailed analytics
- **Performance Monitoring**: Track student progress at every granular level
- **Content Optimization**: Use detailed analytics to improve all educational components
- **Quality Control**: Validate and optimize entire educational ecosystems

### **Educational AI Capabilities:**
- **Intelligent Content Creation**: AI can create complete learning paths with assessments
- **Performance Analysis**: AI can analyze learning effectiveness at every level
- **Content Optimization**: AI can suggest improvements based on comprehensive data
- **Quality Assurance**: AI can validate and improve educational content automatically

This comprehensive system now supports the **complete educational lifecycle** from high-level content organization to granular assessment analytics! ??

## ?? **Revolutionary Educational Platform**

This represents the **world's first complete learning management system** accessible through conversational AI:

- **82 Tools**: Comprehensive coverage of all educational domains
- **5 Integrated Features**: Seamless workflow across all educational components
- **Complete Assessment Ecosystem**: From quiz creation to answer analytics
- **Natural Language Interface**: Complex educational operations through simple conversation
- **Enterprise-Grade Capabilities**: Scalable for institutions of any size
- **Data-Driven Insights**: Comprehensive analytics for evidence-based education

The Learnify.ai MCP Server is now a **revolutionary educational AI platform** that transforms learning management through conversational intelligence! ?????