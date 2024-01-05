using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
namespace MTCG.Interface
{
    public interface IHttpService
    {
        string Route(HttpRequest? httpRequest);
        HttpRequest? Parse(string requestString);
        public HttpMethod GetMethod(string method);
    }
}
