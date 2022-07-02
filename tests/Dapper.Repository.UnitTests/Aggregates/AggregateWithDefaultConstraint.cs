using System;

namespace Dapper.Repository.UnitTests.Aggregates
{
	public record AggregateWithDefaultConstraint
	{
		public int Id { get; set; }

		public int Age { get; set; }

		public DateTime DateCreated { get; private init; }
	}
}
