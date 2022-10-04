namespace Assignment3.Infrastructure.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class TagRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tags.AddRange(new Tag("tagName"), new Tag("tagName2"));
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {
        //Arrange
        //Happens in database creation

        //Act
        var (response, created) = _repository.Create(new Core.TagCreateDTO("Central City"));

        //Assert
        response.Should().Be(Core.Response.Created);
        created.Should().Be(new TagDTO(3, "Central City").Id); //When creating the database, we add 2 tags (tagName and tagName2) therefor the one created in this test must have id 3.
    }

    [Fact]
    public void Delete_given_Tag_returns_Deleted() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Delete(2);

        //Assert
        response.Should().Be(Core.Response.Deleted);

    }

    [Fact]
    public void Read_given_TagId_returns_TagDTO() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Read(2);

        //Assert
        response.Should().Be(new TagDTO(2, "tagName2"));

    }

    [Fact]
    public void Update_given_TagUpdateDTO_returns_Response() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Update(new TagUpdateDTO(2, "nytNavn"));

        //Assert
        response.Should().Be(Core.Response.Updated);

    }

    [Fact]
    public void ReadAll_returns_all_TagDTOs() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.ReadAll();
        var output = new[] {new TagDTO(1, "tagName"), new TagDTO(2, "tagName2")};

        //Assert
        response.Should().BeEquivalentTo(output);
    }
    
    
    
}
