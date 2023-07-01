using UCABPagaloTodoMS.Infrastructure.Settings;

namespace UCABPagaloTodoMS.Providers.Interface
{
    public interface IProviders
    {
        IServiceCollection AddDatabaseService(IServiceCollection services, IConfiguration configuration,
            string environment, bool isRequired);

        public IServiceCollection AddRabbitMqService(IServiceCollection services, IConfiguration configuration);
        
        IServiceCollection AddSendGridService(IServiceCollection services, IConfiguration configuration);
        
        IServiceCollection AddControllers(IServiceCollection services, IConfiguration configuration,
            AppSettings appSettings);

        IServiceCollection AddSwagger(IServiceCollection services, string versionNumber, AppSettings appSettings);
        IServiceCollection AddCors(IServiceCollection services);
    }
}
