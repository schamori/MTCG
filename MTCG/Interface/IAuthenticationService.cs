using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Interface
{
    public interface IAuthenticationService
    {
        User AuthenticateUser(string? authtoken, bool isAdmin);
    }
}
