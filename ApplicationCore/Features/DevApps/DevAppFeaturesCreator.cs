using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps
{
    public interface IDevAppFeaturesCreator
    {
        IGetAllDevAppService CreateGetAllDevAppService();
        IGetOneDevAppService CreateGetOneDevAppService();
        IAddDevAppService CreateAddDevAppService();
        IEditDevAppService CreateEditDevAppService();
        IDeleteDevAppService CreateDeleteDevAppService();
    }

    public class DevAppFeaturesCreator(IDevAppRepository devAppRepository) : IDevAppFeaturesCreator
    {
        private readonly IDevAppRepository devAppRepository = devAppRepository;

        public IAddDevAppService CreateAddDevAppService()
        {
            return new AddDevAppService(devAppRepository);
        }

        public IDeleteDevAppService CreateDeleteDevAppService()
        {
            return new DeleteDevAppService(devAppRepository);
        }

        public IEditDevAppService CreateEditDevAppService()
        {
            return new EditDevAppService(devAppRepository);
        }

        public IGetAllDevAppService CreateGetAllDevAppService()
        {
            return new GetAllDevAppService(devAppRepository);
        }

        public IGetOneDevAppService CreateGetOneDevAppService()
        {
            return new GetOneDevAppService(devAppRepository);
        }
    }
}
