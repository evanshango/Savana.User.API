using Savana.User.API.Requests;
using Savana.User.API.Responses;

namespace Savana.User.API.Interfaces;

public interface IAuthService {
    Task<SignupRes> Signup(SignupReq signupReq);
    Task<SigninRes> Signin(SigninReq signinReq);
}