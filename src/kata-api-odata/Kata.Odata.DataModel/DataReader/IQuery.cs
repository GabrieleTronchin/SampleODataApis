namespace Kata.Odata.DataModel.DataReader
{
    public interface IQuery
    {
        Task<IEnumerable<dynamic>> QueryAsync(string sqlCommand, object parameter);

        Task<int> CountAsync(string sqlCommand, object parameters);
    }
}
