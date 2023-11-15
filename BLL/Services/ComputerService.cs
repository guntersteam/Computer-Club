using BLL.Interfaces;
using DAL.Interfaces;
using DL.Entities;

namespace BLL.Services;

public class ComputerService : GenericService<Computer>,IComputerService
{
    public ComputerService(IRepository<Computer> repository) : base(repository)
    {
    }

    public int FindByModelName(string modelName)
    {
       
       var computer = _repository.GetByPredicate(computer => computer.ModelName == modelName).FirstOrDefault();
        if (computer != null)
        {
            computer.IsReserved = true;
            return computer.ComputerId;
            
        }
        return 0;
    }
    
    public bool GetComputerState(int id)
    {
        var computer = _repository.FindById(id);
        return computer.IsReserved;
    }
}
