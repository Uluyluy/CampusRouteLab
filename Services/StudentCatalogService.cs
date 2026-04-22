using CampusRouterLab.Models;

namespace CampusRouterLab.Services;

public class StudentCatalogService : IStudentCatalogService
{
    private readonly List<Group> _groups;
    public StudentCatalogService()
    {
        _groups = CreateTestGroups();
    }

    public IEnumerable<Group> GetAllGroups()
    {
        return _groups;
    }

    public Group GetGroup(String groupName)
    {
        var group = _groups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа '{groupName}' не найдена!");
        }


        return group;
    }
    
    public Student GetStudent(string groupName, Guid studentId)
    {
        var group = GetGroup(groupName);
        var student = group.Students.FirstOrDefault(s => s.Id == studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Студент '{student}' не найден!");
        }
        return student;
    }

    private List<Group> CreateTestGroups()
    {
        return new List<Group>
        {
            new Group("КВБО-22-24", new List<Student>
            {
                new Student(Guid.NewGuid(), "Иванов Иван", "ivanov@campus.ru", "КВБО-22-24"),
                new Student(Guid.NewGuid(), "Петров Петр", "petrov@campus.ru", "КВБО-22-24"),
                new Student(Guid.NewGuid(), "Сидорова Анна", "sidorova@campus.ru", "КВБО-22-24")
            }),
            new Group("КРБО-13-25", new List<Student>
            {
                new Student(Guid.NewGuid(), "Козлов Дмитрий", "kozlov@campus.ru", "КРБО-13-25"),
                new Student(Guid.NewGuid(), "Новикова Мария", "novikova@campus.ru", "КРБО-13-25"),
            }),
            new Group("КТСО-02-23", new List<Student>
            {
                new Student(Guid.NewGuid(), "Волков Алексей", "volkov@campus.ru", "КТСО-02-23"),
                new Student(Guid.NewGuid(), "Лебедева Елена", "lebedeva@campus.ru", "КТСО-02-23"),
                new Student(Guid.NewGuid(), "Кузнецов Сергей", "kuznetsov@campus.ru", "КТСО-02-23"),
                new Student(Guid.NewGuid(), "Попова Ольга", "popova@campus.ru", "КТСО-02-23")
            })
        };
    }
   
}