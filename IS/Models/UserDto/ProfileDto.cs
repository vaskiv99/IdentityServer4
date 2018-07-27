using System.Collections.Generic;
using IdentityServer4.Models;

namespace IS.Models.UserDto
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public ProfileDto()
        {

        }

        public ProfileDto(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
        }

        public static IEnumerable<ProfileDto> GetUserProfiles(IEnumerable<ApplicationUser> users)
        {
            var profiles = new List<ProfileDto> { };
            foreach (ApplicationUser user in users)
            {
                profiles.Add(new ProfileDto(user));
            }

            return profiles;
        }
    }
}
