namespace EngineCore.Services
{
    public interface IServiceProvider<T>
    {
        T GetService();
    }
}
