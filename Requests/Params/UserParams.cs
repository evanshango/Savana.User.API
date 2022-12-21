using Treasures.Common.Helpers;

namespace Savana.User.API.Requests.Params;

public class UserParams : Pagination {
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
    public bool? Enabled { get; set; }
}