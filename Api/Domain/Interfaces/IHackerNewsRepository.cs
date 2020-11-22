using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Model;

namespace Api.Domain.Interfaces
{
    public interface IHackerNewsRepository
    {
        public Task<List<Story>> GetBestStories();
    }
}
