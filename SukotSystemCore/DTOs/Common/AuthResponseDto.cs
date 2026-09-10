using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukotSystemCore.DTOs.Common
{

    public class AuthResponseDto<TProfile>
    {
        public string ?Token { get; set; } 
        public TProfile Profile { get; set; } 
    }

}
