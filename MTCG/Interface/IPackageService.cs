using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
namespace MTCG.Interface
{
    public interface IPackageService
    {
        List<Card> CreateNewPackage(List<RawRequestCard> Cards);
        bool SavePackage(List<Card> Cards);

        bool isPackageAvaliabe();
        string AssignUserToPackage(int userId);



    }
}
