namespace Assignment.Infrastructure.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class UserRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Users.AddRange(new User("Bob", "mail@mail.dk"), new User("Bobbeline", "test@mail.com"));
        context.SaveChanges();

        _context = context;
        _repository = new UserRepository(_context);
    }

    [Fact]
    public void Create_given_User_returns_Created_with_User()
    {
        //Arrange
        //Happens in database creation

        //Act
        var (response, created) = _repository.Create(new UserCreateDTO("Bobbelademad", "sej@mail.tv"));

        //Assert
        response.Should().Be(Response.Created);
        created.Should().Be(new UserDTO(3, "Bobbelademad", "sej@mail.tv").Id); //When creating the database, we add 2 tags (tagName and tagName2) therefor the one created in this test must have id 3.
    }

    [Fact]
    public void Delete_given_User_returns_Deleted() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Delete(2);

        //Assert
        response.Should().Be(Response.Deleted);

    }

    [Fact]
    public void Read_given_UserId_returns_UserDTO() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Find(2);

        //Assert
        response.Should().Be(new UserDTO(2, "Bobbeline", "test@mail.com"));

    }

    [Fact]
    public void Update_given_TagUpdateDTO_returns_Response() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Update(new UserUpdateDTO(2, "Toddesine", "Todd@esine.dk"));

        //Assert
        response.Should().Be(Response.Updated);

    }

    [Fact]
    public void ReadAll_returns_all_UserDTOs() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Read();
        var output = new[] {new UserDTO(1,"Bob", "mail@mail.dk"), new UserDTO(2, "Bobbeline", "test@mail.com")};

        //Assert
        response.Should().BeEquivalentTo(output);
    }
    

}
