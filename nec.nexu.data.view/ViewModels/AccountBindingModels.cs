using System;
using System.ComponentModel.DataAnnotations;

namespace nec.nexu.Models
{
	// Models used as parameters to AccountController actions.

	public class AddExternalLoginBindingModel
	{
		[Required]
		[Display(Name = "External access token")]
		public string ExternalAccessToken { get; set; }
	}

	public class ChangePasswordBindingModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}


	public class ChangeAccountInfoBindingModel
	{
		[Required]
		[DataType(DataType.Text)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[Display(Name = "New First Name")]
		public string NewFirstname { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Text)]
		[Display(Name = "New Last Name")]
		public string NewLastname { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "New Nickname")]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		public string NewNickname { get; set; }
	}

	public class RegisterBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[Display(Name = "Firstname")]
		public string Firstname { get; set; }

		[Required]
		[Display(Name = "Lastname")]
		public string Lastname { get; set; }

		[Required]
		[Display(Name = "Nickname")]
		public string Nickname { get; set; }

	}

	public class RegisterExternalBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class RemoveLoginBindingModel
	{
		[Required]
		[Display(Name = "Login provider")]
		public string LoginProvider { get; set; }

		[Required]
		[Display(Name = "Provider key")]
		public string ProviderKey { get; set; }
	}

	public class SetPasswordBindingModel
	{
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}



    public class LocalLoginModel
    {
        [Required]
        [Display(Name = "Email")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
