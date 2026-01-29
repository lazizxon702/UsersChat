using Microsoft.AspNetCore.Mvc;
using UsersChat.Interface;

namespace UsersChat.Controller;

[ApiController]
[Route("api/[controller]")]
public class DialogController(IDialogService dialogService) : ControllerBase
{
    
}