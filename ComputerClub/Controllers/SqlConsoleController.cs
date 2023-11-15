using ComputerClub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ComputerClub.Controllers;

public class SqlConsoleController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var query = new QueryEditorViewModel
        {
            Query = "",
            Result = null,
        };
        return View(query);
    }


    [HttpPost]
    public ActionResult RunQuery(string sqlQuery)
    {
        var model = new QueryEditorViewModel
        {
            Query = sqlQuery
        };
        try
        {
            const string connectionString = "Data Source=DESKTOP-5L9TMA8;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Database=ComputerClubDb;";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(sqlQuery, connection);
            connection.Open();
            var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            model.Result = table;
        }
        catch (Exception)
        {
            model.Result = null;
        }
        return View("Index", model);
    }

    [HttpPost]
    public ActionResult ClearTable()
    {
        var model = new QueryEditorViewModel
        {
            Query = "",
            Result = null
        };
        return View("Index", model);
    }


}
