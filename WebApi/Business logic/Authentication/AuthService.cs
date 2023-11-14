using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebStore.Business_logic.Files;
using WebStore.Constants;
using WebStore.Data.Entitties.Identity;
using WebStore.Models.Account;
using WebStore.Repository.Security;
using WebStore.Repository.User;

namespace WebStore.Business_logic.Authentication { 

public class AuthService : IAuthService
{
    private readonly IJwtTokenService _tokenService;
    private readonly ITokenRepository _tokenRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IPictureService _pictureService;
    private readonly IMapper _mapper;

    public AuthService(
        IJwtTokenService tokenService,
        ITokenRepository tokenRepository,
        UserManager<UserEntity> userManager,
        IPictureService pictureService,
        IMapper mapper)
    {
        _tokenService = tokenService;
        _tokenRepository = tokenRepository;
        _userManager = userManager;
        _pictureService = pictureService;
        _mapper = mapper;
    }

    private const string WrongEmailOrPwdMsg = "Username or password is incorrect.";

    public async Task<TokensModel?> Login(LoginModel model)
    {
        UserEntity user = await _userManager.FindByNameAsync(model.Username);
        if (user is null)
            throw new InvalidDataException(WrongEmailOrPwdMsg);

        bool isPasswordValid =
            await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
            throw new InvalidDataException(WrongEmailOrPwdMsg);

        var tokens = await _tokenService.GenerateToken(user);
        await _tokenRepository.AddUserRefreshTokens(new UserRefreshTokens
        {
            UserName = user.UserName,
            RefreshToken = tokens!.RefreshToken,
        });
        return tokens;
    }

    public async Task Register(RegisterModel model)
    {
        var userEntity = _mapper.Map<UserEntity>(model);

        // image processing
        var pictureUrl = await _pictureService.Save(model.Image);
        userEntity.Image = pictureUrl;

        var result = await _userManager.CreateAsync(userEntity, model.Password);

        if (result.Errors.Any())
        {
            var errors = result.Errors.Select(e => e.Description);
            var error = new InvalidDataException(string.Join("\n", errors));
            throw error;
        }

        await _userManager.AddToRoleAsync(userEntity, Roles.User);
    }

    public async Task<TokensModel> Refresh(TokensModel model)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
        var user = await _userManager.GetUserAsync(principal);
        var username = user.UserName;

        // retrieve the saved refresh token from database
        if (username != null)
        {
            var savedRefreshToken = _tokenRepository
                .GetSavedRefreshTokens(username, model.RefreshToken);
            if (savedRefreshToken != null &&
                savedRefreshToken.RefreshToken != model.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");
        }

        var newJwtToken = await _tokenService.GenerateToken(user);

        if (newJwtToken == null)
            throw new SecurityTokenException("Invalid attempt!");

        // saving refresh token to the db
        UserRefreshTokens obj = new()
        {
            RefreshToken = newJwtToken.RefreshToken,
            UserName = username!
        };

        await _tokenRepository
            .DeleteUserRefreshTokens(username!, model.RefreshToken);
        await _tokenRepository.AddUserRefreshTokens(obj);

        return newJwtToken;
    } }
}