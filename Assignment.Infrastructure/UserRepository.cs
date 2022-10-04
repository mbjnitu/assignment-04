using Assignment4.Core;

namespace Assignment4.Infrastructure;

public class UserRepository : Assignment4.Core.IUserRepository
{

    private readonly KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Name == user.Name);
        Response response;
        
        if (entity is null)
        {
            entity = new User(user.Name, user.Email);

            _context.Users.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else 
        {
            response =  Response.Conflict;
        }

        return (response, entity.Id);

    }

    public Response Delete(int userId, bool force = false)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        Response response;

        if (user is null) 
        {
            response = Response.NotFound;
        }
        else if (user.Tasks.Any() && !force)
        {
            response = Response.Conflict;
        }
        else 
        {
            _context.Users.Remove(user);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;
    }

    public UserDTO Read(int userId)
    {
        var users = from u in _context.Users
                    where u.Id == userId
                    select new UserDTO(u.Id, u.Name, u.Email);
       
        if (users is null) return new UserDTO(-1, "NotFound", "NoEmail");
        else return users.FirstOrDefault(); //This will never be null here
    }

    public IReadOnlyCollection<UserDTO> ReadAll()
    {
        var users = from u in _context.Users
                    select new UserDTO(u.Id, u.Name, u.Email);

        return users.ToArray();
    }

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.Users.Find(user.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.Users.FirstOrDefault(u => u.Id != user.Id && u.Name == user.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Name = user.Name;
            _context.SaveChanges();
            response = Response.Updated;
        }
        return response;
    }
}
