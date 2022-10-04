using Assignment3.Core;

namespace Assignment.Infrastructure;

public class WorkItemRepository : Assignment3.Core.IWorkItemRepository
{
    private readonly KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int WorkItemId) Create(WorkItemCreateDTO workItem)
    {
        var entity = _context.WorkItems.FirstOrDefault(t => t.Title == workItem.Title);
        Response response;
        
        if (entity is null)
        {
            entity = new WorkItem(workItem.Title);

            _context.WorkItems.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else 
        {
            response =  Response.Conflict;
        }

        return (response, entity.Id);
    }

    public Response Delete(int WorkItemId)
    {
        var workItem = _context.WorkItems.FirstOrDefault(t => t.Id == workItemId);
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
            _context.WorkItems.Remove(workItem);
            _context.SaveChanges();

            response = Response.Deleted;
            
        }
        else //Only land here if State is one of the conflicting types
        {
            response = Response.Conflict;
        }

        return response;
    }

    public WorkItemDetailsDTO Read(int workItemId)
    {
        var workItems = from t in _context.WorkItems
                    where t.Id == workItemId
                    select new WorkItemDetailsDTO(t.Id, t.Title, t.Description, t.Created, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State, t.Updated);
       
        if (workItems is null) return null;
        else return workItems.FirstOrDefault(); //This will never be null here
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAll()
    {
        var workItems = from t in _context.WorkItems
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllByState(Core.State state)
    {
        var workItems = from t in _context.WorkItems
                    where t.State == state
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return WorkItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tag)
    {
        var specificTag = _context.Tags.Find(tag);
        var workItems = from t in _context.WorkItems
                    where t.Tags == specificTag
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId)
    {
        var workItems = from t in _context.WorkItems
                    where t.AssignedTo!.Id == userId
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllRemoved()
    {
        var workItems = from t in _context.WorkItems
                    where t.State == State.Removed
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return workItems.ToArray();
    }

    public Response Update(WorkItemUpdateDTO workItem)
    {
        var entity = _context.WorkItems.Find(workItem.Id);

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
        else if (_context.WorkItems.FirstOrDefault(t => t.Id != workItem.Id && t.Title == workItem.Title) != null)
        {
            response = Response.Conflict;
        }
        else 
        {
            entity.Title = task.Title;
            entity.Tags = allTags;
            if (task.State != entity.State) {   //Only update "updated" if state is changed
                entity.Updated = DateTime.UtcNow;
            }
            entity.State = task.State;
            _context.SaveChanges();
            response = Response.Updated;
        }
        return response;
    }
}

