using Assignment.Core;

namespace Assignment.Infrastructure;

public class WorkItemRepository : Assignment.Core.IWorkItemRepository
{
    private readonly KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int ItemId) Create(WorkItemCreateDTO workItem)
    {
        var entity = _context.Items.FirstOrDefault(t => t.Title == workItem.Title);
        Response response;
        
        if (entity is null)
        {
            entity = new WorkItem(workItem.Title);

            _context.Items.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else 
        {
            response =  Response.Conflict;
        }

        return (response, entity.Id);
    }

    public Response Delete(int ItemId)
    {
        var workItem = _context.Items.FirstOrDefault(t => t.Id == ItemId);
        Response response;

        if (workItem is null) 
        {
            response = Response.NotFound;
        }
        else if (workItem.State == State.Active)
        {
            workItem.State = State.Removed;
            _context.SaveChanges();
            response = Response.Deleted; //Updated?
        }
        else if (workItem.State == State.New)
        {
            _context.Items.Remove(workItem);
            _context.SaveChanges();

            response = Response.Deleted;
            
        }
        else //Only land here if State is one of the conflicting types
        {
            response = Response.Conflict;
        }

        return response;
    }

    public WorkItemDetailsDTO Find(int ItemId)
    {
        var workItems = from t in _context.Items
                    where t.Id == ItemId
                    select new WorkItemDetailsDTO(t.Id, t.Title, null, DateTime.MinValue, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State, DateTime.MaxValue);
       
        if (workItems is null) return null;
        else return workItems.FirstOrDefault(); //This will never be null here
    }

    public IReadOnlyCollection<WorkItemDTO> Read()
    {
        var workItems = from t in _context.Items
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByState(Core.State state)
    {
        var workItems = from t in _context.Items
                    where t.State == state
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByTag(string tag)
    {
        var specificTag = _context.Tags.Find(tag);
        var workItems = from t in _context.Items
                    where t.Tags == specificTag
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByUser(int userId)
    {
        var workItems = from t in _context.Items
                    where t.AssignedTo!.Id == userId
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadRemoved()
    {
        var workItems = from t in _context.Items
                    where t.State == State.Removed
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public Response Update(WorkItemUpdateDTO workItem)
    {
        var entity = _context.Items.Find(workItem.Id);

        // Get all tags from workItem
        var allTags = new List<Tag> {};
        foreach(string s in workItem.Tags) {
            int sInt = int.Parse(s);
            var tag = _context.Tags.Find(sInt);
            if (tag != null) allTags.Add(tag);
        }

        Response response;

        if (entity is null) 
        {
            response = Response.NotFound;
        }
        else if (_context.Items.FirstOrDefault(t => t.Id != workItem.Id && t.Title == workItem.Title) != null)
        {
            response = Response.Conflict;
        }
        else 
        {
            entity.Title = workItem.Title;
            entity.Tags = allTags;
            entity.State = workItem.State;
            _context.SaveChanges();
            response = Response.Updated;
        }
        return response;
    }
}

