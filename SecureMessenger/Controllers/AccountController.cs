using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SecureMessenger.Models;
using SecureMessenger.Repositories;
using SecureMessenger.Services.Util;
namespace SecureMessenger.Controllers;

public class AccountController : Controller
{
    private readonly UserRepository _userRepository;

    public AccountController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(User user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = _userRepository.GetUserByName(user.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return View(user);
            }
            
            // Generate the public key for the user
            user.Id = ObjectId.GenerateNewId().ToString();
            var (publicKey, privateKey) = KeyGenClass.GenerateKeys(2048);

            user.PublicKey = publicKey;
            user.Id = ObjectId.GenerateNewId().ToString();

            var newUser = _userRepository.CreateUser(user);
            HttpContext.Session.SetString("Username", newUser.Username);
            TempData["PrivateKey"] = privateKey;
            TempData["SignUpSuccess"] = "User has been created successfully. Please copy and save the private key.";

            return RedirectToAction("Index", "Home");
        }
        return View(user);
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(User user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = _userRepository.GetUserByName(user.Username);
            if (existingUser == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username.");
                return View(user);
            }

            HttpContext.Session.SetString("Username", existingUser.Username);

            return RedirectToAction("Index", "Home");
        }
        return View(user);
    }
    
    [HttpGet]
    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Welcome", "Home");
    }
}
