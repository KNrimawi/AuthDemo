using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Helpers
{
    public class TaskHelper
    {
        public void Execute()
        {
            var jwtAuth = new JwtAuthorization();
            var processGuid = Guid.NewGuid();
            var jwt = jwtAuth.GenerateJwtToken(processGuid);

            var webHelper = new WebAPIHelper();
            var x = webHelper.Get<bool>("http://localhost:44367/", "api/items", null, null, null, jwt);
            
        }
    }
}
