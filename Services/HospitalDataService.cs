using HospitalManagementSystem.Models;
using TaskModel = HospitalManagementSystem.Models.Task;
using TaskStatusModel = HospitalManagementSystem.Models.TaskStatus;

namespace HospitalManagementSystem.Services;

public class HospitalDataService
{
    private readonly List<Patient> _patients = new();
    private readonly List<Doctor> _doctors = new();
    private readonly List<Appointment> _appointments = new();
    private readonly List<User> _users = new();
    private readonly List<TaskModel> _tasks = new();
    private readonly List<Notification> _notifications = new();
    private readonly List<AuditLog> _auditLogs = new();
    private readonly List<Report> _reports = new();

    private int _patientId = 1;
    private int _doctorId = 1;
    private int _appointmentId = 1;
    private int _userId = 1;
    private int _taskId = 1;
    private int _notificationId = 1;
    private int _auditLogId = 1;
    private int _reportId = 1;

    public HospitalDataService()
    {
        // Seed with some demo data
        var admin = AddUser(new User 
        { 
            UserName = "admin", 
            Password = "admin123", 
            FullName = "System Administrator",
            Email = "admin@hospital.com",
            Role = UserRole.Administrator 
        });

        var manager = AddUser(new User 
        { 
            UserName = "manager", 
            Password = "manager123", 
            FullName = "John Manager",
            Email = "manager@hospital.com",
            Role = UserRole.Manager 
        });

        var staff = AddUser(new User 
        { 
            UserName = "staff", 
            Password = "staff123", 
            FullName = "Jane Staff",
            Email = "staff@hospital.com",
            Role = UserRole.Staff 
        });

        // Bangladeshi Doctors
        var doc1 = AddDoctor(new Doctor { FullName = "Dr. Mohammad Rahman", Specialty = "Cardiology", Phone = "01712345678" });
        var doc2 = AddDoctor(new Doctor { FullName = "Dr. Fatima Begum", Specialty = "Pediatrics", Phone = "01712345679" });
        var doc3 = AddDoctor(new Doctor { FullName = "Dr. Hasan Ali", Specialty = "Orthopedics", Phone = "01712345680" });
        var doc4 = AddDoctor(new Doctor { FullName = "Dr. Ayesha Khatun", Specialty = "Gynecology", Phone = "01712345681" });
        var doc5 = AddDoctor(new Doctor { FullName = "Dr. Kamal Hossain", Specialty = "Neurology", Phone = "01712345682" });
        var doc6 = AddDoctor(new Doctor { FullName = "Dr. Nasreen Akter", Specialty = "Dermatology", Phone = "01712345683" });
        var doc7 = AddDoctor(new Doctor { FullName = "Dr. Rashed Ahmed", Specialty = "General Surgery", Phone = "01712345684" });
        var doc8 = AddDoctor(new Doctor { FullName = "Dr. Sharmin Islam", Specialty = "Internal Medicine", Phone = "01712345685" });
        var doc9 = AddDoctor(new Doctor { FullName = "Dr. Tanvir Chowdhury", Specialty = "Urology", Phone = "01712345686" });
        var doc10 = AddDoctor(new Doctor { FullName = "Dr. Nusrat Jahan", Specialty = "Ophthalmology", Phone = "01712345687" });
        var doc11 = AddDoctor(new Doctor { FullName = "Dr. Imran Hossain", Specialty = "ENT (Ear, Nose, Throat)", Phone = "01712345688" });
        var doc12 = AddDoctor(new Doctor { FullName = "Dr. Rukhsana Begum", Specialty = "Psychiatry", Phone = "01712345689" });
        var doc13 = AddDoctor(new Doctor { FullName = "Dr. Saiful Islam", Specialty = "Gastroenterology", Phone = "01712345690" });
        var doc14 = AddDoctor(new Doctor { FullName = "Dr. Tahmina Rahman", Specialty = "Endocrinology", Phone = "01712345691" });
        var doc15 = AddDoctor(new Doctor { FullName = "Dr. Arif Mahmud", Specialty = "Pulmonology", Phone = "01712345692" });
        var doc16 = AddDoctor(new Doctor { FullName = "Dr. Farzana Ahmed", Specialty = "Oncology", Phone = "01712345693" });
        var doc17 = AddDoctor(new Doctor { FullName = "Dr. Masud Karim", Specialty = "Cardiac Surgery", Phone = "01712345694" });
        var doc18 = AddDoctor(new Doctor { FullName = "Dr. Salma Akter", Specialty = "Nephrology", Phone = "01712345695" });
        var doc19 = AddDoctor(new Doctor { FullName = "Dr. Rafiqul Islam", Specialty = "Hematology", Phone = "01712345696" });
        var doc20 = AddDoctor(new Doctor { FullName = "Dr. Jahanara Begum", Specialty = "Rheumatology", Phone = "01712345697" });
        var doc21 = AddDoctor(new Doctor { FullName = "Dr. Anwar Hossain", Specialty = "Emergency Medicine", Phone = "01712345698" });
        var doc22 = AddDoctor(new Doctor { FullName = "Dr. Khadija Khatun", Specialty = "Family Medicine", Phone = "01712345699" });
        var doc23 = AddDoctor(new Doctor { FullName = "Dr. Zakir Hussain", Specialty = "Plastic Surgery", Phone = "01712345700" });
        var doc24 = AddDoctor(new Doctor { FullName = "Dr. Rehana Parvin", Specialty = "Anesthesiology", Phone = "01712345701" });
        var doc25 = AddDoctor(new Doctor { FullName = "Dr. Sajjad Ahmed", Specialty = "Radiology", Phone = "01712345702" });

        var pat1 = AddPatient(new Patient { FullName = "John Doe", Age = 45, Gender = "Male", Phone = "555-2001", Address = "123 Main St" });
        var pat2 = AddPatient(new Patient { FullName = "Emily Brown", Age = 30, Gender = "Female", Phone = "555-2002", Address = "456 Oak Ave" });

        AddAppointment(new Appointment
        {
            PatientId = pat1.Id,
            DoctorId = doc1.Id,
            Date = DateTime.Today.AddHours(10),
            Reason = "Routine check-up"
        });

        AddAppointment(new Appointment
        {
            PatientId = pat2.Id,
            DoctorId = doc2.Id,
            Date = DateTime.Today.AddDays(1).AddHours(14),
            Reason = "Fever and cough"
        });
    }

    // Patients
    public IEnumerable<Patient> GetPatients() => _patients;

    public Patient? GetPatient(int id) => _patients.FirstOrDefault(p => p.Id == id);

    public Patient AddPatient(Patient patient)
    {
        patient.Id = _patientId++;
        _patients.Add(patient);
        return patient;
    }

    // Doctors
    public IEnumerable<Doctor> GetDoctors() => _doctors;

    public Doctor? GetDoctor(int id) => _doctors.FirstOrDefault(d => d.Id == id);

    public Doctor AddDoctor(Doctor doctor)
    {
        doctor.Id = _doctorId++;
        _doctors.Add(doctor);
        return doctor;
    }

    // Appointments
    public IEnumerable<Appointment> GetAppointments() => _appointments;

    public Appointment? GetAppointment(int id) => _appointments.FirstOrDefault(a => a.Id == id);

    public Appointment AddAppointment(Appointment appointment)
    {
        // simple double-booking prevention: same doctor + exact same time
        if (_appointments.Any(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date && a.Status != AppointmentStatus.Cancelled))
        {
            throw new InvalidOperationException("This doctor already has an appointment at the selected time.");
        }

        appointment.Id = _appointmentId++;
        _appointments.Add(appointment);
        return appointment;
    }

    public Appointment? UpdateAppointment(int id, Appointment appointment)
    {
        var existing = GetAppointment(id);
        if (existing == null) return null;

        existing.PatientId = appointment.PatientId;
        existing.DoctorId = appointment.DoctorId;
        existing.Date = appointment.Date;
        existing.Reason = appointment.Reason;
        existing.Status = appointment.Status;
        return existing;
    }

    public bool DeleteAppointment(int id)
    {
        var appointment = GetAppointment(id);
        if (appointment == null) return false;
        _appointments.Remove(appointment);
        return true;
    }

    public Patient? UpdatePatient(int id, Patient patient)
    {
        var existing = GetPatient(id);
        if (existing == null) return null;

        existing.FullName = patient.FullName;
        existing.Age = patient.Age;
        existing.Gender = patient.Gender;
        existing.Phone = patient.Phone;
        existing.Address = patient.Address;
        return existing;
    }

    public bool DeletePatient(int id)
    {
        var patient = GetPatient(id);
        if (patient == null) return false;
        _patients.Remove(patient);
        return true;
    }

    public Doctor? UpdateDoctor(int id, Doctor doctor)
    {
        var existing = GetDoctor(id);
        if (existing == null) return null;

        existing.FullName = doctor.FullName;
        existing.Specialty = doctor.Specialty;
        existing.Phone = doctor.Phone;
        return existing;
    }

    public bool DeleteDoctor(int id)
    {
        var doctor = GetDoctor(id);
        if (doctor == null) return false;
        _doctors.Remove(doctor);
        return true;
    }

    // Search functionality
    public IEnumerable<Patient> SearchPatients(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return GetPatients();

        var term = searchTerm.ToLowerInvariant();
        return _patients.Where(p =>
            p.FullName.ToLowerInvariant().Contains(term) ||
            p.Phone.Contains(term) ||
            p.Address.ToLowerInvariant().Contains(term));
    }

    public IEnumerable<Doctor> SearchDoctors(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return GetDoctors();

        var term = searchTerm.ToLowerInvariant();
        return _doctors.Where(d =>
            d.FullName.ToLowerInvariant().Contains(term) ||
            d.Specialty.ToLowerInvariant().Contains(term) ||
            d.Phone.Contains(term));
    }

    public IEnumerable<Appointment> SearchAppointments(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return GetAppointments();

        var term = searchTerm.ToLowerInvariant();
        var patientIds = _patients.Where(p => p.FullName.ToLowerInvariant().Contains(term)).Select(p => p.Id).ToList();
        var doctorIds = _doctors.Where(d => d.FullName.ToLowerInvariant().Contains(term)).Select(d => d.Id).ToList();

        return _appointments.Where(a =>
            patientIds.Contains(a.PatientId) ||
            doctorIds.Contains(a.DoctorId) ||
            a.Reason.ToLowerInvariant().Contains(term));
    }

    // Users
    public IEnumerable<User> GetUsers() => _users;
    public User? GetUser(int id) => _users.FirstOrDefault(u => u.Id == id);
    public User? GetUserByUsername(string username) => _users.FirstOrDefault(u => u.UserName == username);

    public User AddUser(User user)
    {
        user.Id = _userId++;
        _users.Add(user);
        return user;
    }

    public User? UpdateUser(int id, User user)
    {
        var existing = GetUser(id);
        if (existing == null) return null;

        existing.UserName = user.UserName;
        existing.FullName = user.FullName;
        existing.Email = user.Email;
        existing.Role = user.Role;
        existing.IsActive = user.IsActive;
        if (!string.IsNullOrEmpty(user.Password))
            existing.Password = user.Password;
        return existing;
    }

    public bool DeleteUser(int id)
    {
        var user = GetUser(id);
        if (user == null) return false;
        _users.Remove(user);
        return true;
    }

    // Tasks
    public IEnumerable<TaskModel> GetTasks() => _tasks;
    public TaskModel? GetTask(int id) => _tasks.FirstOrDefault(t => t.Id == id);
    public IEnumerable<TaskModel> GetTasksByUser(int userId) => _tasks.Where(t => t.AssignedToUserId == userId);

    public TaskModel AddTask(TaskModel task)
    {
        task.Id = _taskId++;
        _tasks.Add(task);
        return task;
    }

    public TaskModel? UpdateTask(int id, TaskModel task)
    {
        var existing = GetTask(id);
        if (existing == null) return null;

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.AssignedToUserId = task.AssignedToUserId;
        existing.Status = task.Status;
        existing.Priority = task.Priority;
        existing.DueDate = task.DueDate;
        if (task.Status == TaskStatusModel.Completed && existing.Status != TaskStatusModel.Completed)
            existing.CompletedAt = DateTime.Now;
        return existing;
    }

    public bool DeleteTask(int id)
    {
        var task = GetTask(id);
        if (task == null) return false;
        _tasks.Remove(task);
        return true;
    }

    // Notifications
    public IEnumerable<Notification> GetNotifications() => _notifications;
    public Notification? GetNotification(int id) => _notifications.FirstOrDefault(n => n.Id == id);
    public IEnumerable<Notification> GetNotificationsByUser(int userId) => _notifications.Where(n => n.UserId == userId && !n.IsRead).OrderByDescending(n => n.CreatedAt);

    public Notification AddNotification(Notification notification)
    {
        notification.Id = _notificationId++;
        _notifications.Add(notification);
        return notification;
    }

    public bool MarkNotificationAsRead(int id)
    {
        var notification = GetNotification(id);
        if (notification == null) return false;
        notification.IsRead = true;
        return true;
    }

    public bool MarkAllNotificationsAsRead(int userId)
    {
        var userNotifications = _notifications.Where(n => n.UserId == userId && !n.IsRead).ToList();
        foreach (var notification in userNotifications)
        {
            notification.IsRead = true;
        }
        return userNotifications.Any();
    }

    public bool DeleteNotification(int id)
    {
        var notification = GetNotification(id);
        if (notification == null) return false;
        _notifications.Remove(notification);
        return true;
    }

    // Audit Logs
    public IEnumerable<AuditLog> GetAuditLogs() => _auditLogs.OrderByDescending(a => a.Timestamp);
    public AuditLog? GetAuditLog(int id) => _auditLogs.FirstOrDefault(a => a.Id == id);
    public IEnumerable<AuditLog> GetAuditLogsByUser(int userId) => _auditLogs.Where(a => a.UserId == userId).OrderByDescending(a => a.Timestamp);
    public IEnumerable<AuditLog> GetAuditLogsByEntity(string entityType, int? entityId = null)
    {
        var logs = _auditLogs.Where(a => a.EntityType == entityType);
        if (entityId.HasValue)
            logs = logs.Where(a => a.EntityId == entityId);
        return logs.OrderByDescending(a => a.Timestamp);
    }

    public AuditLog AddAuditLog(AuditLog log)
    {
        log.Id = _auditLogId++;
        _auditLogs.Add(log);
        return log;
    }

    // Reports
    public IEnumerable<Report> GetReports() => _reports.OrderByDescending(r => r.GeneratedAt);
    public Report? GetReport(int id) => _reports.FirstOrDefault(r => r.Id == id);

    public Report AddReport(Report report)
    {
        report.Id = _reportId++;
        _reports.Add(report);
        return report;
    }
}


