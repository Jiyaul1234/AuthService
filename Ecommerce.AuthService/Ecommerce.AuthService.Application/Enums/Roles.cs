using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Ecommerce.AuthService.Application.Enums
{
     public  enum Roles
    {
        [Description("Admin")]
        Admin = 1,
        [Description("User")]
        User = 2,
        [Description("Guest")]
        Guest = 3
    }
}



