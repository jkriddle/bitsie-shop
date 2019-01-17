namespace Bitsie.Shop.Common
{
	public class SignInResponse : BaseResponse
	{
		public string Token { get; set; }
		public string Expires { get; set; }
		public UserViewModel User { get; set; }
	}
}

