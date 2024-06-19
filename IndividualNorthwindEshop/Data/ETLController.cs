using ETL.Load;
using Microsoft.AspNetCore.Mvc;

public class ETLController : Controller
{
    private readonly ETLProcess _etlProcess;

    public ETLController(ETLProcess etlProcess)
    {
        _etlProcess = etlProcess;
    }

    public async Task<IActionResult> ExecuteETL()
    {
        await _etlProcess.RunETLAsync();
        return View();
    }
}

