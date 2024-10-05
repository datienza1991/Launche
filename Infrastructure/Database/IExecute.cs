namespace Infrastructure.Database
{
    public interface IExecute<T>
    {
        T Execute();
    }
    public interface IExecuteAsync<T>
    {
        Task<T> ExecuteAsync();
    }

    public interface IExecuteAsync<P, T>
    {
        Task<T> ExecuteAsync(P param);
    }

    public interface IExecute<T, P>
    {
        T Execute(P param);
    }

    public interface IExecute
    {
        void Execute();
    }
}
