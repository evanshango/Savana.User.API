using Treasures.Common.Helpers;

namespace Savana.User.API.Requests.Params;

public class GroupParams : Pagination {
    public string? Name { get; set; }
    public string? OrderBy { get; set; }
}