using BuberDinner.Application.Common.Errors;
using BuberDinner.Application.Common.interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Entities;
using OneOf;

namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }
    public OneOf<AuthenticationResult, DuplicateEmailError> Register(string firstName, string lastName, string email, string password)
    {
        //01) validate the user doesn't already exist
        if(_userRepository.GetUserByEmail(email) is not null)
        {
            return new DuplicateEmailError();
        }

        //02) create user(generate unique Id) & Persisi to the database
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };
        _userRepository.Add(user);


        //03) create Jwt token
       
        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthenticationResult(
           user,
            token
        );
    }

    public AuthenticationResult Login(string email, string password)
    {
        //01) validate the user exists
        if(_userRepository.GetUserByEmail(email) is not User user)
        {
            throw new Exception("User with this email does not exist.");
        }

        //02) validate the password is correct
        if(user.Password != password)
        {
            throw new Exception("Invalid credentials.");
        }

        //03) create Jwt token
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(
           user,
            token
        );
    }
}