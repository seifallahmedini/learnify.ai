# 💡 Answer Management MCP Feature

## Overview

The Answer Management MCP feature provides comprehensive answer option management capabilities for the Learnify.ai platform through a Model Context Protocol (MCP) server. This feature exposes all answer-related API endpoints as structured tools that AI assistants can use to manage quiz question answers, answer validation, analytics, and bulk operations.

## Architecture

### Vertical Slice Structure

```
Features/Answers/
├── Models/
│   └── AnswerModels.cs          # Answer domain models and DTOs
├── Services/
│   └── AnswerApiService.cs      # MCP tools with API integration
└── AnswerFeature.cs            # Feature registration
```

### Key Components

- **AnswerApiService**: MCP tool provider with `[McpServerToolType]` attribute
- **AnswerModels**: Domain models, request/response DTOs, and analytics models
- **AnswerFeature**: Dependency injection registration

## Available MCP Tools (16 Total)

### 🔧 **Answer CRUD Operations**

#### `GetAnswersAsync`
**Description**: Get all answers with optional filtering  
**Parameters**:
- `questionId` (optional): Filter by question ID
- `isCorrect` (optional): Filter by correct/incorrect answers
- `searchTerm` (optional): Search in answer text

#### `GetAnswerAsync`
**Description**: Get answer details by ID  
**Parameters**: 
- `answerId`: The answer ID

#### `CreateAnswerAsync`
**Description**: Create a new answer for a quiz question  
**Required Parameters**:
- `questionId`: Question ID
- `answerText`: Answer text
- `isCorrect`: Whether this is the correct answer

**Optional Parameters**:
- `orderIndex`: Display order (default: 0)

#### `UpdateAnswerAsync`
**Description**: Update existing answer details  
**Parameters**:
- `answerId`: The answer ID to update
- All answer fields (optional): answerText, isCorrect, orderIndex

#### `DeleteAnswerAsync`
**Description**: Delete an answer permanently  
**Parameters**:
- `answerId`: The answer ID to delete

### 🎯 **Question Answer Management**

#### `GetQuestionAnswersAsync`
**Description**: Get all answers for a specific question  
**Parameters**:
- `questionId`: The question ID

#### `ReorderQuestionAnswersAsync`
**Description**: Reorder answers for a specific question  
**Parameters**:
- `questionId`: The question ID
- `answerIdsOrder`: Comma-separated list of answer IDs in new order

#### `CreateMultipleAnswersAsync`
**Description**: Create multiple answers for a question at once  
**Parameters**:
- `questionId`: The question ID
- `answersJson`: JSON array of answers with text, isCorrect, and orderIndex

### ✅ **Answer Validation**

#### `ValidateAnswerAsync`
**Description**: Validate answer business rules and constraints  
**Parameters**:
- `answerId`: The answer ID

#### `ValidateQuestionAnswersAsync`
**Description**: Validate all answers for a specific question  
**Parameters**:
- `questionId`: The question ID

### 📊 **Answer Analytics**

#### `GetAnswerStatsAsync`
**Description**: Get answer selection statistics and analytics  
**Parameters**:
- `answerId`: The answer ID

#### `GetQuestionAnswerAnalyticsAsync`
**Description**: Get comprehensive analytics for all answers of a question  
**Parameters**:
- `questionId`: The question ID

### 🔄 **Bulk Operations**

#### `BulkAnswerOperationAsync`
**Description**: Perform bulk operations on multiple answers  
**Parameters**:
- `answerIds`: Comma-separated list of answer IDs
- `operation`: Operation type (1=Delete, 2=MarkCorrect, 3=MarkIncorrect, 4=Reorder)

### 🔍 **Utility Operations**

#### `CheckAnswerExistsAsync`
**Description**: Check if an answer exists  
**Parameters**:
- `answerId`: The answer ID to check

#### `GetAnswerSummaryAsync`
**Description**: Get answer summary (basic information only)  
**Parameters**:
- `answerId`: The answer ID

## Data Models

### Answer Model
```csharp
public record AnswerModel(
    int Id,
    int QuestionId,
    string QuestionText,
    string AnswerText,
    bool IsCorrect,
    int OrderIndex,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### Answer Statistics Model
```csharp
public record AnswerStatsModel(
    int AnswerId,
    string AnswerText,
    int TimesSelected,
    int TimesCorrect,
    decimal SelectionRate,
    decimal CorrectRate,
    bool IsCorrectAnswer
);
```

### Question Answer Analytics Model
```csharp
public record QuestionAnswerAnalyticsModel(
    int QuestionId,
    string QuestionText,
    int TotalAttempts,
    IEnumerable<AnswerStatsModel> AnswerStats,
    int MostSelectedAnswerId,
    decimal CorrectAnswerRate
);
```

### Bulk Operation Types
```csharp
public enum BulkAnswerOperation
{
    Delete = 1,          // Delete multiple answers
    MarkCorrect = 2,     // Mark answers as correct
    MarkIncorrect = 3,   // Mark answers as incorrect
    Reorder = 4          // Reorder multiple answers
}
```

### Response Formats

#### Success Response
```json
{
  "success": true,
  "data": { /* answer data */ },
  "message": "Operation completed successfully"
}
```

#### Error Response
```json
{
  "success": false,
  "message": "Detailed error message"
}
```

## Usage Examples

### Basic Answer Operations

**Get all answers:**
```
Tool: GetAnswersAsync
Parameters: {}
```

**Get answers for a specific question:**
```
Tool: GetQuestionAnswersAsync
Parameters: { "questionId": 1 }
```

**Create a new answer:**
```
Tool: CreateAnswerAsync
Parameters: {
  "questionId": 1,
  "answerText": "JavaScript is a programming language",
  "isCorrect": true,
  "orderIndex": 1
}
```

**Update answer text:**
```
Tool: UpdateAnswerAsync
Parameters: {
  "answerId": 1,
  "answerText": "JavaScript is a dynamic programming language",
  "isCorrect": true
}
```

### Answer Management

**Create multiple answers at once:**
```
Tool: CreateMultipleAnswersAsync
Parameters: {
  "questionId": 1,
  "answersJson": "[
    {\"answerText\": \"True\", \"isCorrect\": true, \"orderIndex\": 1},
    {\"answerText\": \"False\", \"isCorrect\": false, \"orderIndex\": 2}
  ]"
}
```

**Reorder answers:**
```
Tool: ReorderQuestionAnswersAsync
Parameters: {
  "questionId": 1,
  "answerIdsOrder": "3,1,2,4"
}
```

### Validation and Analytics

**Validate answer:**
```
Tool: ValidateAnswerAsync
Parameters: { "answerId": 1 }
```

**Validate all question answers:**
```
Tool: ValidateQuestionAnswersAsync
Parameters: { "questionId": 1 }
```

**Get answer statistics:**
```
Tool: GetAnswerStatsAsync
Parameters: { "answerId": 1 }
```

**Get question analytics:**
```
Tool: GetQuestionAnswerAnalyticsAsync
Parameters: { "questionId": 1 }
```

### Bulk Operations

**Delete multiple answers:**
```
Tool: BulkAnswerOperationAsync
Parameters: {
  "answerIds": "1,2,3",
  "operation": 1
}
```

**Mark multiple answers as correct:**
```
Tool: BulkAnswerOperationAsync
Parameters: {
  "answerIds": "1,4",
  "operation": 2
}
```

### Search and Discovery

**Search answers by text:**
```
Tool: GetAnswersAsync
Parameters: {
  "searchTerm": "javascript",
  "isCorrect": true
}
```

**Find incorrect answers for a question:**
```
Tool: GetAnswersAsync
Parameters: {
  "questionId": 1,
  "isCorrect": false
}
```

## API Endpoint Mapping

The MCP tools map directly to the Learnify.ai API endpoints:

| MCP Tool | HTTP Method | API Endpoint |
|----------|-------------|--------------|
| `GetAnswersAsync` | GET | `/api/answers` |
| `GetAnswerAsync` | GET | `/api/answers/{id}` |
| `CreateAnswerAsync` | POST | `/api/answers` |
| `UpdateAnswerAsync` | PUT | `/api/answers/{id}` |
| `DeleteAnswerAsync` | DELETE | `/api/answers/{id}` |
| `GetQuestionAnswersAsync` | GET | `/api/answers/question/{questionId}` |
| `ReorderQuestionAnswersAsync` | PUT | `/api/answers/question/{questionId}/reorder` |
| `CreateMultipleAnswersAsync` | POST | `/api/answers/question/{questionId}/bulk` |
| `ValidateAnswerAsync` | GET | `/api/answers/{id}/validate` |
| `GetAnswerStatsAsync` | GET | `/api/answers/{id}/stats` |
| `GetQuestionAnswerAnalyticsAsync` | GET | `/api/answers/question/{questionId}/analytics` |
| `BulkAnswerOperationAsync` | POST | `/api/answers/bulk` |

## Integration

### Prerequisites
- Learnify.ai API running on `http://localhost:5271`
- .NET 8 SDK
- Answer feature registered in `Program.cs`

### Configuration
```csharp
// Program.cs
builder.Services.AddAnswerFeature();
```

### Error Handling
All tools implement comprehensive error handling:
- Input validation
- API communication errors
- Not found scenarios
- Answer validation rules
- Business logic constraints
- Permission/authorization errors

## Answer Management Best Practices

### Answer Creation
- **Clear Text**: Write unambiguous answer options
- **Logical Order**: Arrange answers in logical sequence
- **Balanced Options**: Provide plausible distractors for multiple choice
- **Consistent Format**: Maintain consistent answer formatting

### Validation Rules
- **Single Correct Answer**: Ensure only one correct answer for multiple choice
- **Answer Completeness**: Verify all required answers are provided
- **Text Quality**: Check for spelling and grammar
- **Order Logic**: Ensure logical answer ordering

### Analytics Usage
- **Selection Rates**: Monitor which answers are most/least selected
- **Performance Patterns**: Identify problematic answer options
- **Question Difficulty**: Use analytics to adjust question difficulty
- **Improvement Insights**: Use data to improve answer quality

### Bulk Operations
- **Batch Updates**: Use bulk operations for efficiency
- **Validation First**: Validate before bulk operations
- **Rollback Planning**: Plan for operation failures
- **Audit Trail**: Track bulk changes for accountability

## Testing Commands

Test the answer MCP tools with Claude:

### **Basic Operations:**
1. **"What answer management tools are available?"**
2. **"Get all answers for question 1"**
3. **"Create a new correct answer for question 2"**
4. **"Update answer 5 to mark it as incorrect"**

### **Question Management:**
1. **"Create multiple answers for question 3"**
2. **"Reorder answers for question 1"**
3. **"Validate all answers for question 2"**

### **Analytics Operations:**
1. **"Get statistics for answer 10"**
2. **"Get comprehensive analytics for question 1 answers"**
3. **"Find the most selected answer for question 5"**

### **Bulk Operations:**
1. **"Delete answers 1, 2, and 3"**
2. **"Mark answers 4 and 5 as correct"**
3. **"Validate answers for all questions in quiz 1"**

### **Advanced Operations:**
1. **"Create a complete set of 4 answers for a multiple choice question"**
2. **"Find all incorrect answers across multiple questions"**
3. **"Analyze answer performance across all quiz questions"**

## Answer Workflow Examples

### **Multiple Choice Question Setup:**
1. **Create Question**: Start with a multiple choice question
2. **Add Options**: Use `CreateMultipleAnswersAsync` with 4 options
3. **Set Correct**: Mark one answer as correct, others as incorrect
4. **Order Logically**: Use `ReorderQuestionAnswersAsync`
5. **Validate**: Use `ValidateQuestionAnswersAsync`

### **True/False Question Setup:**
1. **Create Question**: Start with a true/false question
2. **Add Answers**: Create "True" and "False" options
3. **Set Correct**: Mark appropriate answer as correct
4. **Validate**: Ensure only one correct answer

### **Performance Analysis Workflow:**
1. **Get Analytics**: Use `GetQuestionAnswerAnalyticsAsync`
2. **Identify Issues**: Find poorly performing answers
3. **Review Options**: Analyze selection patterns
4. **Improve Quality**: Update problematic answers
5. **Re-validate**: Confirm improvements

## Integration with Other Features

### **Quiz Integration:**
- **Quiz Questions**: Answers belong to quiz questions
- **Quiz Analytics**: Answer performance affects quiz statistics
- **Assessment Quality**: Answer quality impacts quiz effectiveness

### **Question Integration:**
- **Question Types**: Different question types require different answer patterns
- **Question Validation**: Answer validation is part of question validation
- **Question Analytics**: Answer analytics contribute to question performance

### **Performance Tracking:**
- **Student Responses**: Track which answers students select
- **Learning Insights**: Use answer analytics for learning improvement
- **Content Optimization**: Improve questions based on answer performance

## Performance Metrics

### **Key Answer Metrics:**
- **Selection Rate**: How often an answer is chosen
- **Correct Rate**: Success rate when answer is selected
- **Distractor Effectiveness**: How well incorrect answers function
- **Question Clarity**: Measured through answer selection patterns

### **Quality Indicators:**
- **Balanced Selection**: Good distractors show reasonable selection rates
- **Clear Correct Answer**: Correct answers should have high selection by knowledgeable students
- **Appropriate Difficulty**: Answer patterns should match intended difficulty
- **Learning Value**: Incorrect answers should provide learning opportunities

The Answer MCP feature provides comprehensive answer option management capabilities while maintaining consistency with the quiz, question, and other assessment features! 💡