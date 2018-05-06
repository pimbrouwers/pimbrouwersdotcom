using System;
using System.Collections.Generic;
using System.Text;

namespace Pimbrouwersdotcom.Domain
{
  public class Account
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
  }
}