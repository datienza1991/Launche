using Microsoft.Extensions.DependencyInjection;

namespace UI.Group
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddGroupServiceCollection(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAdd, Add>()
                .AddSingleton<IDelete, Delete>()
                .AddSingleton<IGetAll, GetAll>()
                .AddSingleton<IGetOne, GetOne>()
                .AddSingleton<IEdit, Edit>();
        }
    }
}
