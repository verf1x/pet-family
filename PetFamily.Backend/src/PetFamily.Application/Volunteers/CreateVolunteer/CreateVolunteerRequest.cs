namespace PetFamily.Application.Volunteers.CreateVolunteer;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber);