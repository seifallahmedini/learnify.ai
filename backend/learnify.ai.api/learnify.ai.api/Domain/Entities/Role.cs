using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Domain.Entities;

public class Role : IdentityRole<int>
{
    public Role() : base() { }
    public Role(string roleName) : base(roleName) { }
}

