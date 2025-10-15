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

## ??? Available Tools (31 Total)

The MCP server exposes comprehensive lesson and course management tools:

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

## ?? Testing Commands

Once set up, test with Claude:

### **Lesson Management:**
1. **"What lesson management tools are available?"**
2. **"Get lesson with ID 1"**
3. **"Get all lessons for course 1"**
4. **"Check if lesson 5 exists"**
5. **"Create a new lesson for course 2"**

### **Course Management:**
1. **"What course management tools are available?"**
2. **"Get all published courses"**
3. **"Create a new beginner course for JavaScript"**
4. **"Get statistics for course 1"**
5. **"Find all courses by instructor 2"**
6. **"Publish course 3"**
7. **"Get featured courses under $50"**

### **Advanced Operations:**
1. **"Get course 1 with all its lessons"**
2. **"Find all unpublished courses and their lesson counts"**
3. **"Create a complete course structure with 5 lessons"**

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
- **API Integration**: Both features integrate with Learnify.ai API endpoints
- **Model Definitions**: Check `Features/*/Models/` for data structures

## ??? Architecture

The MCP server follows a **vertical slice architecture**:

```
learnify.ai.mcp.server/
??? Features/
?   ??? Lessons/           # Lesson management feature
?   ?   ??? Models/        # Lesson domain models
?   ?   ??? Services/      # LessonApiService with MCP tools
?   ?   ??? LessonFeature.cs
?   ??? Courses/           # Course management feature
?       ??? Models/        # Course domain models
?       ??? Services/      # CourseApiService with MCP tools
?       ??? CourseFeature.cs
??? Shared/                # Common infrastructure
?   ??? Models/           # Base API models
?   ??? Services/         # BaseApiService
??? Program.cs            # MCP server startup
```

## ?? Support

If you encounter issues:
1. Check the troubleshooting section above
2. Verify all prerequisites are met
3. Test the script manually before using with Claude
4. Ensure file paths match your actual directory structure
5. Check API connectivity to http://localhost:5271

The MCP server is now ready for comprehensive lesson and course management with Claude Desktop! ??