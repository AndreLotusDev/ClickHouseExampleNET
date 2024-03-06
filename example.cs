using System.ComponentModel.Design;
using ClickHouse.Ado;
using CyberEnergy.Domain.Classes;
using CyberEnergy.Domain.Interfaces.Repository.Common;
using System.Data;
using Microsoft.Extensions.Options;
using Polly;
 
namespace CyberEnergy.Infrastructure.ClickHouse.Repositories
{
    public class ClickHouseAsyncRepository : IClickHouseAsyncRepository
    {
        //private readonly IOptions<ConnectionStringClickHouse> _options;
        private readonly ClickHouseConnection _connection;
 
        //public ClickHouseAsyncRepository(IOptions<ConnectionStringClickHouse> options)
        //{
        //    _options = options;
        //    _connection = new ClickHouseConnection(_options.Value.Connection);
        //    _connection.Open();
        //}
 
        public ClickHouseAsyncRepository()
        {
            _connection = new ClickHouseConnection("");
            _connection.Open();
        }
 
        public async Task ExecuteNonQueryWithParametersAsync(string query, IEnumerable<QueryParameters> data)
        {
            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => InternalExecuteNonQueryWithParametersAsync(query, data));
        }
 
 
        public async Task InternalExecuteNonQueryWithParametersAsync(string query, IEnumerable<QueryParameters> data)
        {
            var command = _connection.CreateCommand(query);
 
            List<ClickHouseParameter> parameters = new();
            foreach (var parameter in data)
            {
                command.Parameters.Add(new ClickHouseParameter()
                {
                    ParameterName = parameter.QueryParameter,
                    Value = parameter.Value
                });
            }
 
            await Task.Run(() => command.ExecuteReader());
        }
 
        public async Task ExecuteQueryAsync(string query, Action<IDataReader> mappingAction)
        {
            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => InternalExecuteQueryAsync(query, mappingAction));
        }
 
        public async Task InternalExecuteQueryAsync(string query, Action<IDataReader> mappingAction)
        {
            await Task.Run(() =>
            {
                using var cmd = _connection.CreateCommand(query);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.ReadAll(r =>
                    {
                        mappingAction.Invoke(r);
                    });
                }
            });
        }
 
        public async Task ExecuteNonQueryAsync(string query)
        {
            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => InternalExecuteNonQueryAsync(query));
        }
 
        public async Task InternalExecuteNonQueryAsync(string query)
        {
            using var cmd = _connection.CreateCommand(query);
            await Task.Run(() => cmd.ExecuteNonQuery());
        }
 
        public async Task ExecuteQueryWithParametersAsync(
            string query, 
            IEnumerable<QueryParameters> data, 
            Action<IDataReader> mappingAction)
        {
            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => InternalExecuteQueryWithParametersAsync(query, data, mappingAction));
        }
 
        public async Task InternalExecuteQueryWithParametersAsync(
            string query, 
            IEnumerable<QueryParameters> data,
            Action<IDataReader> mappingAction
            )
        {
            var command = _connection.CreateCommand(query);
 
            List<ClickHouseParameter> parameters = new();
            foreach (var parameter in data)
            {
                command.Parameters.Add(new ClickHouseParameter()
                {
                    ParameterName = parameter.QueryParameter,
                    Value = parameter.Value
                });
            }
 
            var reader = await Task.Run(() => command.ExecuteReader());
            while (reader.Read())
            {
                mappingAction.Invoke(reader);
            }
 
        }
    }
}
