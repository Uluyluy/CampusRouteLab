namespace CampusRouterLab.Models;

public record Student(Guid Id, string Name, string Email, string GroupName);

public record Group(string Name, List<Student> Students);
