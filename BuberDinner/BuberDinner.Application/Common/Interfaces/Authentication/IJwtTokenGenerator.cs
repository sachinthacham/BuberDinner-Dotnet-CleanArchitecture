using BuberDinner.Domain.Entities;

namespace BuberDinner.Application.Common.interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}