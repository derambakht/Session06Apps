using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Common;
using Infrastructure.InputModels;
using Infrastructure.ViewModels;
using Core;
using Infrastructure.Utility;
using Core.Entities.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace SampleAPI.Controllers;

//[Authorize]
public class CompanyController : BaseController
{
    private readonly SampleDbContext dbContext;
    private readonly FileUtility fileUtility;
    private readonly IMemoryCache memoryCache;

    public CompanyController(SampleDbContext dbContext, FileUtility fileUtility, IMemoryCache memoryCache)
    {
        this.dbContext = dbContext;
        this.fileUtility = fileUtility;
        this.memoryCache = memoryCache;
    }

    [HttpPost]
    //[AllowAnonymous]
    public async Task<IActionResult> Create([FromForm] ComapnyInputModel model)
    {
        //CheckPermission ????


        var fileSaveResult = await fileUtility.SaveInDisk(model.Logo, nameof(Company));
        if (!fileSaveResult.IsSuccess) return BadRequest(fileSaveResult);

        memoryCache.Remove(CacheKeys.CompanyList);

        var company = new Company
        {
            Address = model.Address,
            CompanyName = model.CompanyName,
            Phone = model.Phone,
            Logo = await fileUtility.ConvertToByteArray(model.Logo),
            LogoFileName = fileSaveResult.Data,
        };

        await dbContext.Companies.AddAsync(company);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    // [HttpPost]
    // public async Task<IActionResult> Create(Company model)
    // {
    //     await dbContext.Companies.AddAsync(model);
    //     await dbContext.SaveChangesAsync();
    //     return Ok();
    // }   

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] ComapnyInputModel model)
    {
        var result = new MellatActionResult<bool>();
        var company = await dbContext.Companies.FindAsync(model.Id);
        if (company == null)
        {
            result.IsSuccess = false;
            result.Message = "شناسه ارسالی صحیح نمی باشد";
            return BadRequest(result);
        }

        memoryCache.Remove(CacheKeys.CompanyList);

        company.CompanyName = model.CompanyName;
        company.Address = model.Address;
        if (model.Logo is not null && model.Logo.Length > 0)
        {
            company.Logo = await fileUtility.ConvertToByteArray(model.Logo);
            var fileSaveResult = await fileUtility.SaveInDisk(model.Logo, nameof(Company), FileType.Image);
            if (!fileSaveResult.IsSuccess) return BadRequest(fileSaveResult);
            company.LogoFileName = fileSaveResult.Data;
        }

        await dbContext.SaveChangesAsync();

        result.IsSuccess = true;
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        //1
        // var company = await dbContext.Companies.FindAsync(id);
        // dbContext.Companies.Remove(company);
        // await dbContext.SaveChangesAsync();

        // //2
        // var company2 = new Company {Id = id};
        // dbContext.Companies.Remove(company2);
        // await dbContext.SaveChangesAsync();


        memoryCache.Remove(CacheKeys.CompanyList);


        //3
        await dbContext.Companies.Where(q => q.Id == id).ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();

        return Ok();

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await dbContext.Companies.FindAsync(id);
        // var result2 = await dbContext.Companies.SingleOrDefaultAsync(q => q.Id == id);
        // var result3 = await dbContext.Companies.OrderBy(q => q.CompanyName).FirstOrDefaultAsync(q => q.Id == id);

        var model = new CompanyViewModel
        {
            Id = result.Id,
            CompanyName = result.CompanyName,
            LogoBase64 = fileUtility.ConvertToBase64(result.Logo),
            LogoUrl = fileUtility.GenerateFileUrl(result.LogoFileName, nameof(Company))
        };



        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyList = new List<Company>();
        if (!memoryCache.TryGetValue(CacheKeys.CompanyList, out companyList))
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(120));

            companyList = await dbContext.Companies.ToListAsync();

            memoryCache.Set(CacheKeys.CompanyList, companyList, cacheEntryOptions);

        }
        return Ok(companyList);
    }

    [HttpGet("{page}/itemcount")]
    public async Task<IActionResult> GetWithPaging(int page = 1, int itemCount = 2)
    {
        var skipCount = (page - 1) * itemCount;

        var result = await dbContext.Companies.Skip(skipCount).Take(itemCount)
        .Select(q => new { q.Id, q.CompanyName }).ToListAsync();

        return Ok(result);
    }


    [HttpGet("WithPaging")]
    public async Task<IActionResult> GetWithPagingWithQueryString(int page = 1, int itemCount = 2)
    {
        var skipCount = (page - 1) * itemCount;

        var result = await dbContext.Companies.Skip(skipCount).Take(itemCount)
        .Select(q => new { q.Id, q.CompanyName }).ToListAsync();

        return Ok(result);
    }
}