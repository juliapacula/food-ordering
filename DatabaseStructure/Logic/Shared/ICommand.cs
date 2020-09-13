using System.Threading.Tasks;

namespace DatabaseStructure.Logic.Shared
{
    public interface ICommand
    {
        public Task HandleAsync();
    }
}