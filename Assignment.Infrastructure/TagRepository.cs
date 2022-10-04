using Assignment4.Core;

namespace Assignment4.Infrastructure;

public class TagRepository : Assignment4.Core.ITagRepository
{

    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = _context.Tags.FirstOrDefault(t => t.Name == tag.Name);
        Response response;
        
        if (entity is null)
        {
            entity = new Tag(tag.Name);

            _context.Tags.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else 
        {
            response =  Response.Conflict;
        }

        return (response, entity.Id);

    }

    public Response Delete(int tagId, bool force = false)
    {
        var tag = _context.Tags.FirstOrDefault(t => t.Id == tagId);
        Response response;

        if (tag is null) 
        {
            response = Response.NotFound;
        }
        else if (tag.Tasks.Any() && !force)
        {
            response = Response.Conflict;
        }
        else 
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;
    }

    public TagDTO Read(int tagId)
    {
        var tags =  from t in _context.Tags
                    where t.Id == tagId
                    select new TagDTO(t.Id, t.Name);
       
        if (tags is null) return new TagDTO(-1, "NotFound");
        else return tags.FirstOrDefault(); //This will never be null here
    }

    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var tags =  from t in _context.Tags
                    select new TagDTO(t.Id, t.Name);

        return tags.ToArray();
        
    }

    public Response Update(TagUpdateDTO tag)
    {
        var entity = _context.Tags.Find(tag.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.Tags.FirstOrDefault(t => t.Id != tag.Id && t.Name == tag.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Name = tag.Name;
            _context.SaveChanges();
            response = Response.Updated;
        }
        return response;
    }
}
