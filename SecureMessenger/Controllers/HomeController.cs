using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SecureMessenger.Models;
using SecureMessenger.Repositories;
using SecureMessenger.Services.Util;

namespace SecureMessenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MessageRepository _messageRepository;
        private readonly UserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, MessageRepository messageRepository,
            UserRepository userRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public IActionResult Index(int page = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Welcome");
            }

            String privateKey = null;
            
            if (TempData.ContainsKey("PrivateKey"))
            {
                ViewData["PrivateKey"] = TempData["PrivateKey"];
                privateKey = (String)TempData["PrivateKey"];
            }
            if (TempData.ContainsKey("SignUpSuccess"))
            {
                ViewData["SignUpSuccess"] = TempData["SignUpSuccess"];
            }
            if (TempData.ContainsKey("SendMessageSuccess"))
            {
                ViewData["SendMessageSuccess"] = TempData["SendMessageSuccess"];
            }
            if (TempData.ContainsKey("SendMessageError"))
            {
                ViewData["SendMessageError"] = TempData["SendMessageError"];
            }
            if (TempData.ContainsKey("CurrentPage"))
            {
                page = (int) TempData["CurrentPage"];
            }

            ViewData["CurrentPage"] = page;

            const int PageSize = 10;

            var messages = _messageRepository.GetMessagesByUsername(username);
            var totalMessages = messages.Count();

            messages = messages
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            if (privateKey != null)
            {
                try
                {
                    var DN = RSA.DecodeKey(privateKey);
                    foreach (var message in messages)
                    {
                        message.Content = RSA.Decode(message.Content, DN[0], DN[1]);
                    }
            
                    ViewData["Messages"] = messages;
                    ViewData["CurrentPage"] = page;
                    ViewData["TotalPages"] = (int)Math.Ceiling(totalMessages / (double)PageSize);
                    ViewData["PrivateKey"] = privateKey;
                }
                catch (Exception)
                {
                    ViewData["GetMessageError"] = "Invalid private key format. Please ensure your private key is valid.";
                }
            }

            ViewData["Messages"] = messages;
            ViewData["TotalPages"] = (int)System.Math.Ceiling(totalMessages / (double)PageSize);

            return View();
        }

        [HttpPost]
        public IActionResult DecodeMessages(string privateKey, int page = 1)
        {
            TempData["PrivateKey"] = privateKey;
            TempData["CurrentPage"] = page;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SendMessage(string recipientUsername, string messageContent)
        {
            var senderUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(senderUsername)) // Session expired
            {
                return RedirectToAction("Welcome");
            }
            var sender = _userRepository.GetUserByName(senderUsername);
            if (sender == null)
            {
                return RedirectToAction("Welcome");
            }

            var recipient = _userRepository.GetUserByName(recipientUsername);
            if (recipient?.PublicKey == null)
            {
                TempData["SendMessageError"] = "No user found with that username.";
                return RedirectToAction("Index");
            }

            try
            {
                var EN = RSA.DecodeKey(recipient.PublicKey);
                var cipher = RSA.Encode(messageContent, EN[0], EN[1]);

                var message = new Message
                {
                    User = recipient,
                    Sender = sender,
                    Content = cipher
                };

                _messageRepository.CreateMessage(message);

                TempData["SendMessageSuccess"] = "Message sent successfully!";
            }
            catch (Exception)
            {
                TempData["SendMessageError"] = "Invalid public key format. Please ensure the recipient's public key is valid.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Welcome()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
