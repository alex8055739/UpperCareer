using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "{0}不可以少于{2}个字", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "密码输入不一致")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "请输入电子邮箱")]
        [Display(Name = "电子邮箱")]
        [EmailAddress(ErrorMessage = "输入不符合电子邮箱格式")]
        public string Email { get; set; }

        [Required(ErrorMessage = "没有密码，谁知道你是不是敌人派来的奸细！")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class ForgetPasswordModel
    {
        [Required(ErrorMessage="电子邮箱不能为空")]
        [EmailAddress(ErrorMessage="输入不符合电子邮箱格式")]
        public string Email { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0}不可以少于{2}个字", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码输入不一致")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "邀请码")]
        public string InviteCode { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "电子邮箱")]
        [EmailAddress(ErrorMessage = "电子邮箱格式不正确")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "称谓")]
        [StringLength(10, ErrorMessage = "称谓不要超过10个字")]
        public string Name { get; set; }

        public User RestoreRegisterModelToUserObject()
        {
            return new User()
            {
                Password = Password,
                Name = Name,
                Email = Email,
                Roles = new List<Role>() { new Role() { RoleName = "User" } }
            };
        }
    }

    public class LoginRegiModel
    {
        public LoginRegiModel()
        {
            Login = new LoginModel();
            Register = new RegisterModel();
        }

        public LoginModel Login { get; set; }

        public RegisterModel Register { get; set; }
    }

    public class UserManageModel
    {
        public UserManageModel() { }

        public UserManageModel(UserDetail ud)
        {
            ConvertUserManageObjectToModel(ud);
        }

        public string UserDetailID { get; set; }

        public string UserID { get; set; }

        //public bool HasPasswordChangeRequest { get; set; }

        //public LocalPasswordModel NewPassword { get; set; }

        [Required]
        [Display(Name = "称谓")]
        [StringLength(10, ErrorMessage = "称谓不要超过10个字")]
        public string Name { get; set; }

        [Display(Name = "头衔")]
        [StringLength(10, ErrorMessage = "头衔不要超过10个字")]
        public string Title { get; set; }

        [Display(Name = "性别")]
        public int Gender { get; set; }

        [Display(Name = "公司")]
        [StringLength(20, ErrorMessage = "公司名称不要超过20个字")]
        public string Company { get; set; }

        [Display(Name = "个人简介")]
        public string SelfDescription { get; set; }

        [Display(Name = "行业")]
        public int FieldIndex { get; set; }

        private void ConvertUserManageObjectToModel(UserDetail ud)
        {
            if (ud != null)
            {
                UserDetailID = ud.ObjectId;
                UserID = ud.ForUser.ObjectID;
                Name = ud.ForUser.Name;
                Gender = ud.ForUser.Gender;
                Title = ud.ForUser.Title;
                Company = ud.ForUser.Company;
                SelfDescription = ud.SelfDescription;
                FieldIndex = ud.ForUser.FieldIndex;
            }
        }

        public UserDetail RestoreUserManageModelToUserDetailObject()
        {
            return new UserDetail()
            {
                ObjectId = UserDetailID,
                ForUser = new User()
                {
                    ObjectID = UserID,
                    Name = Name,
                    Title = Title,
                    Gender = Gender,
                    Company = Company,
                    FieldIndex = FieldIndex
                },
                SelfDescription = SelfDescription
            };
        }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
