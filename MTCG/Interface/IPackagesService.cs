using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
namespace MTCG.Interface
{
    public interface IPackagesService
    {
        List<Card> CreateNewPackage(List<Dictionary<string, string>> Cards);

    }
}
