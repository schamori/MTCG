using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public record class HttpRequest(HttpMethod method, string route, string? subRouteParameter = null, string? authentication = null, string? content = null, bool plain = false);

}
