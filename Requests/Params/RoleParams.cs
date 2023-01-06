using Treasures.Common.Helpers;

namespace Savana.User.API.Requests.Params;

public class RoleParams : Pagination {
    public string? Name { get; set; }
}