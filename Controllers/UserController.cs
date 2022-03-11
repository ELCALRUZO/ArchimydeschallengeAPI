using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchimydeschallengeAPI.Models;
using ArchimydesWeb.Helpers;
using ArchimydesWeb.Models;
using ArchimydesWeb.Repositories;
using ArchimydesWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ArchimydeschallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
       // RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly UnitOfWork _concreteUnitOfWork;
        private ILogger<dynamic> _log;

        public UserController(
            UserManager<ApplicationUser> userManager,
           // SignInManager<ApplicationUser> signInManager,
            UnitOfWork unitOfWork,
            ILogger<dynamic> log
            //RoleManager<ApplicationRole> roleManager
            )
        {
            _userManager = userManager;
            //_concreteUnitOfWork = unitOfWork;
            //_signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _log = log;
            //_roleManager = roleManager;
        }



        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost, Route("user/Create")]
        public async Task<IActionResult> Create([FromBody] UserSignUpApiModel setdata)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var systemUserViewModel = new CreateUserVM();
                var role = _unitOfWork.Role.Get(setdata.RoleID);

                var usernameExists = await _unitOfWork.User.GetUserByEmail(setdata.Email.Trim());

                if (usernameExists == null)
                {
                    var sysUser = new SysUsers
                    {
                        FirstName = setdata.FirstName,
                        LastName = setdata.LastName,
                        MiddleName = setdata.MiddleName,
                        PhoneNumber = setdata.PhoneNumber,
                        Email = setdata.Email,
                        CompanyName = setdata.CompanyName,
                        DateCreated = DateTime.Now,
                        RoleID = setdata.RoleID
                    };

                    await _unitOfWork.CreateUser.AddAsync(sysUser);
                    _unitOfWork.Complete();

                    var systemUser = new ApplicationUser { UserName = sysUser.Email, Email = sysUser.Email, UserID = sysUser.UserID };
                    var password = "A" + Guid.NewGuid().ToString("N") + "@01";
                    var result = await _userManager.CreateAsync(systemUser, password);

                    if (result.Succeeded)
                    {
                        var roleselected = role.RoleName;
                        var userAddToRoleResult = await _userManager.AddToRoleAsync(systemUser, roleselected);

                        if (userAddToRoleResult.Succeeded)
                        {
                            return Ok(new SuccessfulSignupModel
                            {
                                Status = 00,
                                Message = "User Created up succesfully",
                            });
                        }
                    }
                }
                //_unitOfWork.CreateUser.UpdateUser(sysUser);

                //_unitOfWork.Complete(); 
                return Ok(new SuccessfulSignupModel
                {
                    Status = 99,
                    Message = "User Creation Failed",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error has occured! Please try again");
            }

        }


        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpGet, Route("UpdateUser/{ID}")]
        public async Task<IActionResult> UpdateUser(string ID)
        {
            try
            {
                var model = new UserRModel();
                if (ID != null)
                {
                    var getid = await _unitOfWork.User.GetUserByID(Convert.ToInt32(ID));
                    model = new UserRModel
                    {
                        Status=00,
                        Message="User Details retrieved successfully",
                        UserID = getid.UserID,
                        FirstName = getid.FirstName,
                        MiddleName = getid.MiddleName,
                        LastName = getid.LastName,
                        Email = getid.Email,
                        PhoneNumber = getid.PhoneNumber,
                        CompanyName = getid.CompanyName,
                        DateCreated = getid.DateCreated.ToString(),

                    };
                    return Ok(model);
                }
                return Ok(new UserRModel
                {
                    Status = 99,
                    Message = "User Retrieval Failed",
                });
            }
            catch (Exception ex)
            {
                General.LogToFile(ex.InnerException); 
                return StatusCode(100, new { message = " Error, Please try again" });
            }
        }


        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost, Route("user/update")]
        public async Task<IActionResult> Update([FromBody] UserSignUpApiModel setdata)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _unitOfWork.User.UpdateUser(new SysUsers
                {
                    UserID = setdata.UserID,
                    FirstName = setdata.FirstName,
                    MiddleName = setdata.MiddleName,
                    LastName = setdata.LastName,
                    PhoneNumber = setdata.PhoneNumber,
                    CompanyName = setdata.CompanyName,
                    Email = setdata.Email,
                    DateCreated = DateTime.Now,
                });
                return Ok(new SuccessfulSignupModel
                {
                    Status = 00,
                    Message = "User has been updated successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error has occured! Please try again");
            }

        }



        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpDelete, Route("DeleteUser/{ID}")]
        public async Task<IActionResult> Delete(string ID)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var submis = await _unitOfWork.User.GetUserByID(Convert.ToInt32(ID));

                if (submis != null)
                {
                    _unitOfWork.CreateUser.Remove(submis);
                    return Ok(new SuccessfulSignupModel
                    {
                        Status = 00,
                        Message = "User has been deleted successfully",
                    });
                }
                return Ok(new SuccessfulSignupModel
                {
                    Status = 00,
                    Message = "User does not exist",
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error has occured! Please try again");
            }

        }











        //// GET: api/User
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/User/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/User
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/User/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
