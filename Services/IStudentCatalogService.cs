using CampusRouterLab.Models;

namespace CampusRouterLab.Services;

// Хранит и управляет данными о студентах и группах (каталог)
public interface IStudentCatalogService
{
    // возвращает все учебные группы
    IEnumerable<Group> GetAllGroups();

    // возвращает одну группу по её названию
    Group GetGroup(String groupName);
    
    // возвращает конкретного студента из указанной группы по его ID
    Student GetStudent(string groupName, Guid studentId);
}