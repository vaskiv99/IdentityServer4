namespace IS.Models.UserDto
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = default(string);
        public string LastName { get; set; } = default(string);
        public string UserName { get; set; } = default(string);
    }
}
