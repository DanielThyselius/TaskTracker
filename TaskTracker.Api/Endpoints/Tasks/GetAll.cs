using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Api.Endpoints.Task;
public class GetAllTasks : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapGet("/posts", Handle)
        .WithSummary("Gets all tasks");

    public record Request(
       [FromQuery] Status? WithStatus = null,
       [FromQuery] DateTime? DueBefore = null,
       [FromQuery] OrderByField? OrderBy = null,
       bool Descending = false
    );
    public record Response(
        int Id,
        string Title,
        string Description,
        Status Status,
        DateTime DueDate
    );

    private static List<Response> Handle([AsParameters] Request request, ITaskService taskService)
    {
        TaskItem[] tasks = Array.Empty<TaskItem>();

        if (request.WithStatus is null && request.DueBefore is null)
            tasks = taskService.GetAll();
        else
        {
            var filterCriteria = new FilterCriteria
            {
                WithStatus = request.WithStatus,
                DueBefore = request.DueBefore
            };
            tasks = taskService.GetFilteredTasks(filterCriteria);
        }

        if (request.OrderBy is not null)
        {
            var orderCriteria = new OrderCriteria
            {
                OrderBy = request.OrderBy,
                Descending = request.Descending
            };
            tasks = taskService.OrderTasks(tasks, orderCriteria);
        }

        return tasks.Select(task => new Response(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate
          )).ToList();
    }
}