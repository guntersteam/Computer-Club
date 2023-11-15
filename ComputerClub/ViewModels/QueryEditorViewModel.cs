using System.Data;

namespace ComputerClub.ViewModels;

public class QueryEditorViewModel
{
    public string? Query { get; set; }
    public DataTable? Result { get; set; }
}
