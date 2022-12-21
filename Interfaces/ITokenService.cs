using Savana.User.API.Entities;

namespace Savana.User.API.Interfaces;

public interface ITokenService {
    string GenerateToken(UserEntity user);
}