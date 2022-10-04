namespace Assignment3.Infrastructure.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class WorkItemRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;

    public WorkItemnamespace Assignment3.Entities.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class TaskRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tasks.AddRange(new Task("taskTitle1"), new Task("taskTitle2"));
        context.Tags.AddRange(new Tag("tag1"), new Tag("tag2"));
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public void Create_task_returns_response_and_id() 
    {
        //Arrange
        var listOfTagIds = new[]{"1", "2"};

        //Act
        var (response, created) = _repository.Create(new Core.TaskCreateDTO("taskTitle3", null, null, listOfTagIds));

        //Assert
        response.Should().Be(Response.Created);
        created.Should().Be(new TaskDTO(3, "taskTitle3", null, listOfTagIds, State.New).Id);
    }


    [Fact]
    public void Delete_task_returns_response() 
    {
        //Arrange

        //Act
        var response = _repository.Delete(2);

        //Assert
        response.Should().Be(Response.Deleted);
        
    }

    [Fact]
    public void Read_given_TaskId_returns_TaskDTO() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Read(2);
        var tagList = new List<string>();

        //Assert
        response.Should().BeEquivalentTo(new TaskDetailsDTO(2, "taskTitle2",null, new DateTime(), null, tagList, State.New, new DateTime()), options => options.Excluding(x => x.StateUpdated));
        // In the above we choose to ignore the StateUpdated field, as it is not relavant to this test, and impossible to pinpoint its exact value

    }

    [Fact]
    public void Update_given_TagUpdateDTO_returns_Response() 
    {
        //Arrange
        //Happens in database creation
        var tagList = new List<string>();

        //Act
        var response = _repository.Update(new TaskUpdateDTO(2, "nytNavn", null, null, tagList, State.Active));

        //Assert
        response.Should().Be(Core.Response.Updated);

    }

    [Fact]
    public void ReadAll_returns_all_TaskDTOs() 
    {
        //Arrange
        //Happens in database creation
        var tagList = new List<string>();

        //Act
        var response = _repository.ReadAll();
        var output = new[] {new TaskDTO(1, "taskTitle1", null, tagList, State.New), new TaskDTO(2, "taskTitle2", null, tagList, State.New)};

        //Assert
        response.Should().BeEquivalentTo(output);
    }

    [Fact]
    public void Updating_tags_is_allowed_and_returns_new_tags_correctly() {
        //Arrange
        //Happens in database creation
        var tagList = new[]{"2"};

        //Act
        _repository.Update(new WorkItemUpdateDTO(2, "nytNavnIgen", null, null, tagList, State.Active));

        var response = _repository.Read(2);

        //Assert
        response.Should().BeEquivalentTo(new WorkItemDetailsDTO(2, "nytNavnIgen", null, new DateTime(), null, new[]{"tag2"}, State.Active, new DateTime()), options => options.Excluding(x => x.StateUpdated));
        // In the above we choose to ignore the StateUpdated field, as it is not relavant to this test, and impossible to pinpoint its exact value
    }

    [Fact]
    public void Updating_state_changes_StateUpdated_correctly() {
        //Arrange
        //Happens in database creation
        var tagList = new[]{"2"};
        var expected = DateTime.UtcNow;

        //Act
        _repository.Update(new WorkItemUpdateDTO(2, "nytNavnIgen", null, null, tagList, State.Active));
        var response = _repository.Read(2);

        var stateUpdated = response.StateUpdated;

        bool updateIsAfterExpected = expected < stateUpdated;
        //Assert
        Assert.Equal(expected, stateUpdated, precision: TimeSpan.FromSeconds(1)); // If new updated time was within 1 second.
        Assert.Equal(true, updateIsAfterExpected); // Since expected is made before the update happens, we expect its value to be less (meaning the actual was actually updated).
    }
}
RepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.WorkItems.AddRange(new WorkItem("taskTitle1"), new WorkItem("taskTitle2"));
        context.Tags.AddRange(new Tag("tag1"), new Tag("tag2"));
        context.SaveChanges();

        _context = context;
        _repository = new WorkItemRepository(_context);
    }

    [Fact]
    public void Create_workItem_returns_response_and_id() 
    {
        //Arrange
        var listOfTagIds = new[]{"1", "2"};

        //Act
        var (response, created) = _repository.Create(new Core.WorkItemCreateDTO("taskTitle3", null, null, listOfTagIds));

        //Assert
        response.Should().Be(Response.Created);
        created.Should().Be(new WorkItemDTO(3, "taskTitle3", null, listOfTagIds, State.New).Id);
    }


    [Fact]
    public void Delete_workItem_returns_response() 
    {
        //Arrange

        //Act
        var response = _repository.Delete(2);

        //Assert
        response.Should().Be(Response.Deleted);
        
    }

    [Fact]
    public void Read_given_WorkItemId_returns_WorkItemDTO() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Read(2);
        var tagList = new List<string>();

        //Assert
        response.Should().BeEquivalentTo(new WorkItemDetailsDTO(2, "taskTitle2",null, new DateTime(), null, tagList, State.New, new DateTime()), options => options.Excluding(x => x.StateUpdated));
        // In the above we choose to ignore the StateUpdated field, as it is not relavant to this test, and impossible to pinpoint its exact value

    }

    [Fact]
    public void Update_given_TagUpdateDTO_returns_Response() 
    {
        //Arrange
        //Happens in database creation
        var tagList = new List<string>();

        //Act
        var response = _repository.Update(new WorkItemUpdateDTO(2, "nytNavn", null, null, tagList, State.Active));

        //Assert
        response.Should().Be(Core.Response.Updated);

    }

    [Fact]
    public void ReadAll_returns_all_WorkItemDTOs() 
    {
        //Arrange
        //Happens in database creation
        var tagList = new List<string>();

        //Act
        var response = _repository.ReadAll();
        var output = new[] {new WorkItemDTO(1, "taskTitle1", null, tagList, State.New), new TaskDTO(2, "taskTitle2", null, tagList, State.New)};

        //Assert
        response.Should().BeEquivalentTo(output);
    }

    [Fact]
    public void Updating_tags_is_allowed_and_returns_new_tags_correctly() {
        //Arrange
        //Happens in database creation
        var tagList = new[]{"2"};

        //Act
        _repository.Update(new WorkItemUpdateDTO(2, "nytNavnIgen", null, null, tagList, State.Active));

        var response = _repository.Read(2);

        //Assert
        response.Should().BeEquivalentTo(new WorkItemDetailsDTO(2, "nytNavnIgen", null, new DateTime(), null, new[]{"tag2"}, State.Active, new DateTime()), options => options.Excluding(x => x.StateUpdated));
        // In the above we choose to ignore the StateUpdated field, as it is not relavant to this test, and impossible to pinpoint its exact value
    }

    [Fact]
    public void Updating_state_changes_StateUpdated_correctly() {
        //Arrange
        //Happens in database creation
        var tagList = new[]{"2"};
        var expected = DateTime.UtcNow;

        //Act
        _repository.Update(new WorkItemUpdateDTO(2, "nytNavnIgen", null, null, tagList, State.Active));
        var response = _repository.Read(2);

        var stateUpdated = response.StateUpdated;

        bool updateIsAfterExpected = expected < stateUpdated;
        //Assert
        Assert.Equal(expected, stateUpdated, precision: TimeSpan.FromSeconds(1)); // If new updated time was within 1 second.
        Assert.Equal(true, updateIsAfterExpected); // Since expected is made before the update happens, we expect its value to be less (meaning the actual was actually updated).
    }
}
