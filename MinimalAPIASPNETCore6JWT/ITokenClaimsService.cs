namespace MinimalAPIASPNETCore6JWT;

public interface ITokenClaimsService
{
	Task<string> GetTokenAsync(string userName);
}