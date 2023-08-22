
using System.Diagnostics;
using System.Net.Http.Headers;
using Application.IServices;
using Core;
using Core.Entities.Base;
using Infrastructure.Common;
using Infrastructure.InputModels;
using Infrastructure.Utility;
using Infrastructure.ViewModels;

namespace Application.Services;

public class EFCompanyService : ICompanyService
{
    private readonly SampleDbContext dbContext;
    private readonly FileUtility fileUtility;

    public EFCompanyService(SampleDbContext dbContext, FileUtility fileUtility)
    {
        this.dbContext = dbContext;
        this.fileUtility = fileUtility;
    }

    public Task<MellatActionResult<bool>> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<MellatActionResult<CompanyViewModel>> GetById(int id)
    {
        var result = new MellatActionResult<CompanyViewModel>();
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var data = await dbContext.Companies.FindAsync(id);
        stopWatch.Stop();
        var exexuteTime = stopWatch.ElapsedMilliseconds;

        result.IsSuccess = true;
        result.Data = new CompanyViewModel
        {
            Id = data.Id,
            CompanyName = data.CompanyName,
            LogoBase64 = fileUtility.ConvertToBase64(data.Logo),
            LogoUrl = fileUtility.GenerateFileUrl(data.LogoFileName, nameof(Company))
        };



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