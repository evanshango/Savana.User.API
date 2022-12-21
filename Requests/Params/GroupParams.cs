using Treasures.Common.Helpers;

namespace Savana.User.API.Requests.Params;

public class GroupParams : Pagination {
    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }
}