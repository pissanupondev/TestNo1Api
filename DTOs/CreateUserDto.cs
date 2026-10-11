public record CreateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly BirthDay,
    string Sex,
    string Occupation,
    string Profile,
    string ContentType
);