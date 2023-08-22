using Infrastructure.Common;
using Infrastructure.InputModels;
using Infrastructure.ViewModels;


namespace Application.IServices;


public interface ICompanyService
{
    public Task<MellatActionResult<Guid>> Insert(ComapnyInputModel model);
    public Task<MellatActionResult<bool>> Update(ComapnyInputModel model);
    public Task<MellatActionResult<bool>> Delete(Guid id);
    public Task<MellatActionResult<CompanyViewModel>> GetById(int id);
    public Task<MellatActionResult<List<CompanyViewModel>>> GetWithPaging(int page, int itemCount);
}