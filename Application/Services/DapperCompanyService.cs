using System.Data;
using Application.IServices;
using Infrastructure.Common;
using Infrastructure.InputModels;
using Infrastructure.ViewModels;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Core.Entities.Base;
using Infrastructure.Utility;
using Infrastructure.Resources;
using System.Diagnostics;

namespace Application.Services;

public class DapperCompanyService : ICompanyService
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration configuraiton;
    private readonly FileUtility fileUtility;

    public DapperCompanyService(Microsoft.Extensions.Configuration.IConfiguration configuraiton, FileUtility fileUtility)
    {
        this.configuraiton = configuraiton;
        this.fileUtility = fileUtility;
    }

    public Task<MellatActionResult<bool>> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<MellatActionResult<CompanyViewModel>> GetById(int id)
    {
        var result = new MellatActionResult<CompanyViewModel>();

        var connetionString = configuraiton.GetConnectionString("SampleConnectionString");
        using (IDbConnection connection = new SqlConnection(connetionString))
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var model = await connection.QuerySingleOrDefaultAsync<Company>("select * from Companies Where Id=@id", new { id });
            stopWatch.Stop();
            var exexuteTime = stopWatch.ElapsedMilliseconds;
            
            if (model == null)
            {
                result.IsSuccess = false;
                result.Message = Messages.InvalidFileFormat;
                return result;
            }

            result.Data = new CompanyViewModel
            {
                Id = model.Id,
                CompanyName = model.CompanyName,
                LogoBase64 = fileUtility.ConvertToBase64(model.Logo),
                LogoUrl = fileUtility.GenerateFileUrl(model.LogoFileName, nameof(Company))
            };
        }

        result.IsSuccess = true;

        return result;
    }

    public Task<MellatActionResult<List<CompanyViewModel>>> GetWithPaging(int page, int itemCount)
    {
        throw new NotImplementedException();
    }

    public Task<MellatActionResult<Guid>> Insert(ComapnyInputModel model)
    {
        throw new NotImplementedException();
    }

    public Task<MellatActionResult<bool>> Update(ComapnyInputModel model)
    {
        throw new NotImplementedException();
    }
}