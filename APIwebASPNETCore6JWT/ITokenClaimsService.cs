namespace APIwebASPNETCore6JWT;

public interface ITokenClaimsService
{
	Task<string> GetTokenAsync(string userName);
}