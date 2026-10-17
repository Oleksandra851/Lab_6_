using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab_6.Data;
using Lab_6.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Lab_6.Hubs
{
    public class TeacherHub : Hub
    {
        private readonly UniversityDbContext _context;

        public TeacherHub(UniversityDbContext context)
        {
            _context = context;
        }

        // 1. READ: Отримання списку всіх викладачів
        public async Task GetTeachers()
        {
            var teachers = await FetchTeacherDtosAsync();
            await Clients.Caller.SendAsync("ReceiveTeachers", teachers);
        }

        // 2. READ: Отримання одного викладача за ID
        public async Task GetTeacherById(int id)
        {
            var teacher = await FetchTeacherDtoByIdAsync(id);
            await Clients.Caller.SendAsync("ReceiveTeacherDetails", teacher);
        }

        // 3. CREATE: Створення викладача з кількома предметами
        public async Task CreateTeacher(Teacher teacher, List<SubjectAssignment> selectedSubjects)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            if (selectedSubjects != null && selectedSubjects.Any())
            {
                foreach (var sub in selectedSubjects)
                {
                    if (sub.HoursRead > 0)
                    {
                        _context.TeacherSubjects.Add(new TeacherSubject
                        {
                            TeacherID = teacher.TeacherID,
                            SubjectID = sub.SubjectID,
                            HoursRead = sub.HoursRead
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            await NotifyAllClientsUpdate();
        }

        // 4. UPDATE: Редагування викладача та його предметів
        public async Task UpdateTeacher(Teacher teacher, List<SubjectAssignment> selectedSubjects)
        {
            var existingTeacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherID == teacher.TeacherID);

            if (existingTeacher != null)
            {
                existingTeacher.LastName = teacher.LastName;
                existingTeacher.FirstName = teacher.FirstName;
                existingTeacher.MiddleName = teacher.MiddleName;
                existingTeacher.Phone = teacher.Phone;
                existingTeacher.Workplace = teacher.Workplace;
                existingTeacher.PositionID = teacher.PositionID;
                existingTeacher.HomeAddress = teacher.HomeAddress;
                existingTeacher.Characteristic = teacher.Characteristic;

                _context.TeacherSubjects.RemoveRange(existingTeacher.TeacherSubjects);

                if (selectedSubjects != null && selectedSubjects.Any())
                {
                    foreach (var sub in selectedSubjects)
                    {
                        if (sub.HoursRead > 0)
                        {
                            _context.TeacherSubjects.Add(new TeacherSubject
                            {
                                TeacherID = teacher.TeacherID,
                                SubjectID = sub.SubjectID,
                                HoursRead = sub.HoursRead
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await NotifyAllClientsUpdate();
            }
        }

        // 5. DELETE: Видалення викладача
        public async Task DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (teacher != null)
            {
                if (teacher.TeacherSubjects != null && teacher.TeacherSubjects.Any())
                {
                    _context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
                }
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();

                await NotifyAllClientsUpdate();
            }
        }

        // Допоміжні методи
        private async Task NotifyAllClientsUpdate()
        {
            var teachers = await FetchTeacherDtosAsync();
            await Clients.All.SendAsync("ReceiveTeachers", teachers);
        }

        private async Task<List<TeacherDto>> FetchTeacherDtosAsync()
        {
            var rawTeachers = await _context.Teachers
                .Include(t => t.Position)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .ToListAsync();

            return rawTeachers.Select(t => new TeacherDto
            {
                TeacherID = t.TeacherID,
                LastName = t.LastName,
                FirstName = t.FirstName,
                MiddleName = t.MiddleName,
                Phone = t.Phone,
                Workplace = t.Workplace,
                PositionID = t.PositionID,
                PositionName = t.Position != null ? t.Position.PositionName : string.Empty,
                HomeAddress = t.HomeAddress,
                Characteristic = t.Characteristic,
                Subjects = t.TeacherSubjects.Select(ts => new SubjectDto
                {
                    SubjectID = ts.SubjectID,
                    SubjectName = ts.Subject != null ? ts.Subject.SubjectName : string.Empty,
                    HoursRead = ts.HoursRead
                }).ToList()
            }).ToList();
        }

        private async Task<TeacherDto?> FetchTeacherDtoByIdAsync(int id)
        {
            var t = await _context.Teachers
                .Include(t => t.Position)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (t == null) return null;

            return new TeacherDto
            {
                TeacherID = t.TeacherID,
                LastName = t.LastName,
                FirstName = t.FirstName,
                MiddleName = t.MiddleName,
                Phone = t.Phone,
                Workplace = t.Workplace,
                PositionID = t.PositionID,
                PositionName = t.Position != null ? t.Position.PositionName : string.Empty,
                HomeAddress = t.HomeAddress,
                Characteristic = t.Characteristic,
                Subjects = t.TeacherSubjects.Select(ts => new SubjectDto
                {
                    SubjectID = ts.SubjectID,
                    SubjectName = ts.Subject != null ? ts.Subject.SubjectName : string.Empty,
                    HoursRead = ts.HoursRead
                }).ToList()
            };
        }
    }
}