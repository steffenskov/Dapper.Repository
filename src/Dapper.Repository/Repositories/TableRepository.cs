namespace Dapper.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : ITableRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	private readonly IDapperInjectionFactory _dapperInjectionFactory;
	private readonly IDapperInjection<TAggregate> _dapperInjection;
	private readonly AggregateConfiguration<TAggregate> _configuration;
	private readonly IConnectionFactory _connectionFactory;
	private readonly IQueryGenerator<TAggregate> _queryGenerator;

	public TableRepository(IOptions<AggregateConfiguration<TAggregate>> options) // TODO: Cannot rely on AggregateConfiguration as it's abstract, propbably better to just use a single config class
	{
		ArgumentNullException.ThrowIfNull(options);
		ArgumentNullException.ThrowIfNull(options.Value);
		ArgumentNullException.ThrowIfNull(options.Value.QueryGeneratorFactory);
		ArgumentNullException.ThrowIfNull(options.Value.ConnectionFactory);
		ArgumentNullException.ThrowIfNull(options.Value.DapperInjectionFactory);

		_configuration = options.Value;
		_dapperInjectionFactory = _configuration.DapperInjectionFactory;
		_dapperInjection = _dapperInjectionFactory.Create<TAggregate>();
		_connectionFactory = _configuration.ConnectionFactory!;
		_queryGenerator = _configuration.QueryGeneratorFactory.Create<TAggregate>(_configuration);
	}

	#region ITableRepository
	public async Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

		return await QuerySingleOrDefaultAsync(query, new { id }, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateGetQuery();

		return await QuerySingleOrDefaultAsync(query, new { id }, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateGetAllQuery();
		return await QueryAsync(query, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateInsertQuery(aggregate);
		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateUpdateQuery();
		return await QuerySingleOrDefaultAsync(query, aggregate, cancellationToken: cancellationToken).ConfigureAwait(false);
	}
	#endregion

	#region Dapper methods
	protected async Task<IEnumerable<TAggregate>> QueryAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QueryAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TAggregate?> QuerySingleOrDefaultAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QuerySingleOrDefaultAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TAggregate> QuerySingleAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QuerySingleAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<IEnumerable<TResult>> ScalarMultipleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QueryAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TResult?> ScalarSingleOrDefaultAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QuerySingleOrDefaultAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TResult> ScalarSingleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QuerySingleAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<int> ExecuteAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.ExecuteAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}
	#endregion
}
