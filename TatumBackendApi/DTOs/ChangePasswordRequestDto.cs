using System;
using System.ComponentModel.DataAnnotations;

namespace TatumBackendApi.DTOs
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; } = string.Empty;



        [Required(ErrorMessage = "Password is required.")]

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]

        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]

        //[RegularExpression(

        //    @"^(?=.{8, 100}$)(?=.*[a - z])(?=.*[A - Z])(?=.*\d)(?=.*[^A - Za - z\d]).*$",

        //    //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]){8,100}$",

        //    ErrorMessage = 

        //        "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."

        //)]

        [RegularExpression(

           @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",

           ErrorMessage = "Password must contain at least one uppercase letter," +

           " one lowercase letter, one number, and one special character."

       )]

        public string NewPassword { get; set; } = string.Empty;



        [Required(ErrorMessage = "Password is required.")]

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]

        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]

        //[RegularExpression(

        //    @"^(?=.{8, 100}$)(?=.*[a - z])(?=.*[A - Z])(?=.*\d)(?=.*[^A - Za - z\d]).*$",

        //    //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]){8,100}$",

        //    ErrorMessage = 

        //        "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."

        //)]

        [RegularExpression(

           @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",

           ErrorMessage = "Password must contain at least one uppercase letter," +

           " one lowercase letter, one number, and one special character."

       )]

        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}