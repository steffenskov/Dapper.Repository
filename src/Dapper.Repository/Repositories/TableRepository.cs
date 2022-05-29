using Dapper.Repository.Reflection;

namespace Dapper.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>, ITableRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	public TableRepository(IOptions<TableAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
	}

	#region ITableRepository
	public async Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

		return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateGetQuery();

		if (_hasValueObjects)
		{
			return (await QueryWithValueObjectsAsync(query, WrapId(id)).ConfigureAwait(false)).FirstOrDefault();
		}
		else
			return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var invalidIdentityProperties = _configuration.GetIdentityProperties()
											.Where(pk => !pk.HasDefaultValue(aggregate))
											.ToList();

		if (invalidIdentityProperties.Any())
		{
			throw new ArgumentException($"Aggregate has the following identity properties, which have non-default values: {string.Join(", ", invalidIdentityProperties.Select(col => col.Name))}", nameof(aggregate));
		}
		var query = _queryGenerator.GenerateInsertQuery(aggregate);
		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpdateQuery();
		return await QuerySingleOrDefaultAsync(query, aggregate, cancellationToken: cancellationToken).ConfigureAwait(false);
	}
	#endregion
}
