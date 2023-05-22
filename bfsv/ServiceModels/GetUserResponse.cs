using bfsv.Dtos;
using System.Collections.Generic;

namespace bfsv.ServiceModels
{
    public class GetUserResponse
    {
        public Dictionary<string, UserResponse> Users { get; set; }
    }
}