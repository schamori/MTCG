using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using MTCG.Services;
using MTCG.Models;
using Newtonsoft.Json;
using MTCG.Interface;

namespace MTCG.Controller
{
    public abstract class ControllerBase
    {
        public abstract HttpResponse HandleRequest(HttpRequest httpRequest);


        protected object ParseContent(string? content, Type desiredObject)
        {
            if (content == null)
            {
                throw new InvalidDataException();
            }
            object? parsedContent = JsonConvert.DeserializeObject(content, desiredObject);
            if (parsedContent is null)
            {
                throw new InvalidDataException();
            }
            return parsedContent;

        }

    }
}
