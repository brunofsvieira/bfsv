using bfsv.Dtos;
using System.Collections.Generic;

namespace bfsv.ServiceModels
{
    public class GetUserResponse
    {
        public Dictionary<int, UserResponse> Users { get; set; }
    }
}