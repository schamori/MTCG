using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public record class HttpResponse (StatusCode StatusCode, string? Payload = null);
}
