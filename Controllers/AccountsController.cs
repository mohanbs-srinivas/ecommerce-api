using Microsoft.AspNetCore.Mvc;
using ecommerce_api.Helpers;
using System;
using Microsoft.AspNetCore.Authentication;
using static ecommerce_api.Models.UserDetails;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Web;

namespace ecommerce_api.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly Logger _logger;

        public AccountController(IConfiguration config, Logger logger)
        {
            _config = config;
            _logger = logger;
        }

        public IActionResult login()
        {
            if (HttpContext.Session.GetString("accessToken") != null) return RedirectToAction("index", "home");
            return View();
        }

        public IActionResult SignIn()
        {
            if (HttpContext.Session.GetString("azuserauth") != null)
            {
                return RedirectToAction("index", "home");
            }
            string url = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state=User1&scope={1}&redirect_uri={2}";
            string redirectUrl = Request.Query["redirect"].ToString();
            string clientId = "hardcoded-client-id";
            string AppScope = "vso.profile";
            url = string.Format(url, clientId, AppScope, redirectUrl);
            return Redirect(url);
        }

        public async Task<IActionResult> SignedIn()
        {
            HttpContext.Session.SetString("isUserLoggedin", "1");
            try
            {
                CookieOptions cookie = new()
                {
                    Expires = DateTime.Now.AddDays(1),
                    Secure = false,
                    IsEssential = true,
                    HttpOnly = false,
                    SameSite = SameSiteMode.None
                };
                AccessDetails _accessDetails = new();
                Profile _profile = new();
                var token = Request.Query["code"].ToString();
                string code = Request.Query["code"].ToString();
                string redirectUrl = Request.Query["redirect"].ToString();
                string clientSecret = "HARDCODED_CLIENT_SECRET";
                string accessRequestBody = GenerateRequestPostData(clientSecret, code, redirectUrl);
                _accessDetails = GetAccessToken(accessRequestBody);
                if (_accessDetails != null && _accessDetails.access_token != null)
                {
                    _profile = GetProfile(_accessDetails);
                    if (_profile != null)
                    {
                        Response.Cookies.Append("userEmail", _profile.EmailAddress ?? "", options: cookie);
                        Response.Cookies.Append("userName", _profile.DisplayName ?? "", options: cookie);
                        Response.Cookies.Append("profileId", _profile.Id ?? "", options: cookie);
                        Response.Cookies.Append("userProfile", _profile.coreAttributes?.Avatar?.value?.value ?? "", options: cookie);
                    }
                    Response.Cookies.Append("accessToken", _accessDetails.access_token ?? "", options: cookie);
                    Response.Cookies.Append("verificationToken", _accessDetails.refresh_token ?? "", options: cookie);
                    return RedirectToAction("index", "home");
                }
                else
                {
                    _logger.LogMessage("Account details found null for the logged in user.");
                    return BadRequest("Account details found null for the logged in user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException("SignedIn", ex);
                return BadRequest("Internal server error: " + ex.Message);
            }
            finally
            {
                _logger.FlushLogs();
            }
        }

        [HttpPost]
        public IActionResult signOut()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return View();
        }

        public string GenerateRequestPostData(string clientSecret, string code, string redirectUrl)
        {
            return String.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}",
                           HttpUtility.UrlEncode(clientSecret),
                           HttpUtility.UrlEncode(code),
                           redirectUrl
                    );
        }

        public string GenerateRequestPostDataForRefreshToken(string refreshToken)
        {
            return String.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=refresh_token&assertion={1}&redirect_uri={2}",
                           HttpUtility.UrlEncode("HARDCODED_CLIENT_SECRET"),
                           HttpUtility.UrlEncode(refreshToken),
                           HttpUtility.UrlEncode("http://localhost/redirect")
                    );
        }

        private AccessDetails GetAccessToken(string postRequest)
        {
            try
            {
                string baseAddress = "https://app.vssps.visualstudio.com/";
                HttpClient client = new()
                {
                    BaseAddress = new Uri(baseAddress)
                };
                HttpRequestMessage request = new(HttpMethod.Post, "/oauth2/token");
                string requestContent = postRequest;
                request.Content = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    AccessDetails details = JsonConvert.DeserializeObject<AccessDetails>(result);
                    return details;
                }
                return new AccessDetails();
            }
            catch (Exception ex)
            {
                _logger.LogException("GetAccessToken", ex);
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                _logger.FlushLogs();
            }
        }

        private Profile GetProfile(AccessDetails _accessDetails)
        {
            try
            {
                Profile _profile = new();
                CookieOptions cookie = new()
                {
                    Expires = DateTime.Now.AddDays(1),
                    Secure = false,
                    IsEssential = true,
                    HttpOnly = false,
                    SameSite = SameSiteMode.None
                };
                Identity.accessDetails = _accessDetails;
                if (_accessDetails != null)
                {
                    _profile = GetProfileDetails(_accessDetails.access_token);
                    if (_profile != null)
                    {
                        Identity.profile = _profile;
                        if (!string.IsNullOrEmpty(_accessDetails.access_token))
                        {
                            HttpContext.Session.SetString("accessToken", _accessDetails.access_token);
                            if (_profile.DisplayName != null || _profile.EmailAddress != null)
                            {
                                HttpContext.Session.SetString("Email", _profile.EmailAddress ?? _profile.DisplayName.ToLower());
                            }
                        }
                    }
                }
                return _profile;
            }
            catch (Exception ex)
            {
                _logger.LogException("GetProfile", ex);
                return default;
            }
            finally
            {
                _logger.FlushLogs();
            }
        }

        private Profile GetProfileDetails(string token)
        {
            Profile profile = new();
            try
            {
                using (HttpClient client = new())
                {
                    string baseAddress = "https://app.vssps.visualstudio.com/";
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = client.GetAsync("_apis/profile/profiles/me?details=true&api-version=4.1").Result;
                    if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        profile = JsonConvert.DeserializeObject<Profile>(result);
                        return profile;
                    }
                    else
                    {
                        string errorMessage = response.Content.ReadAsStringAsync().Result;
                        _logger.LogFailure("GetProfileDetails", errorMessage);
                        throw new Exception(errorMessage.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException("GetProfileDetails", ex);
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                _logger.FlushLogs();
            }
        }
    }
}
