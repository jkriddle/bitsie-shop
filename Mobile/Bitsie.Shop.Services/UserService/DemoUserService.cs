using Bitsie.Shop.Common;

namespace Bitsie.Shop.Services
{
	public class DemoUserService : IUserService
	{
		public AuthenticateResponse Authenticate(string token) {
			// Always show login screen
			return new AuthenticateResponse {
				Success = true,
				User = new UserViewModel {
					BackupAddress = "1BitcoinAddress",
					EnableGratuity = true,
					PaymentMethod = PaymentMethod.Bitcoin
				}
			};
		}

		public SignInResponse SignIn(string email, string password) {
			return new SignInResponse {
				Success = true,
				User = new UserViewModel {
					BackupAddress = "1BitcoinAddress",
					EnableGratuity = true,
					PaymentMethod = PaymentMethod.Bitcoin
				}
			};
		}

	}
}

