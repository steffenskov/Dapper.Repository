﻿using Dapper.DDD.Repository.MySql;
using Dapper.DDD.Repository.UnitTests.Aggregates;
using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.QueryGenerators;

public class MySqlQueryGeneratorTests
{
	#region Constructor

	[Fact]
	public void Constructor_TableNameIsNull_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate> { TableName = null! };

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(
			() => new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));

		Assert.Equal("Value cannot be null. (Parameter 'readConfiguration.EntityName')", ex.Message);
	}

	[Fact]
	public void Constructor_TableNameIsWhiteSpace_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate> { TableName = " " };
		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() => new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));

		Assert.Equal("Entity name cannot be null or whitespace. (Parameter 'configuration')", ex.Message);
	}

	[Fact]
	public void Constructor_SchemaIsNotNull_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate> { TableName = "Some name", Schema = " " };
		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() => new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));

		Assert.Equal("MySql doesn't support Schema. (Parameter 'configuration')", ex.Message);
	}

	#endregion

	#region Delete

	[Fact]
	public void GenerateDeleteQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"SELECT Users.Id, Users.FirstLevel_SecondLevel_Name FROM Users WHERE Users.Id = @Id;DELETE FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			@"SELECT Users.Age, Users.Id_Password, Users.Id_Username FROM Users WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;DELETE FROM Users WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			@"SELECT Users.Id, Users.DeliveryAddress_City, Users.DeliveryAddress_Street, Users.InvoiceAddress_City, Users.InvoiceAddress_Street FROM Users WHERE Users.Id = @Id;DELETE FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_OnePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var deleteQuery = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			@"SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;DELETE FROM Users WHERE Users.Id = @Id;",
			deleteQuery);
	}

	[Fact]
	public void GenerateDeleteQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var deleteQuery = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			@"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;DELETE FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;",
			deleteQuery);
	}

	#endregion

	#region GetAll

	[Fact]
	public void GenerateGetAllQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal("SELECT Users.Id, Users.FirstLevel_SecondLevel_Name FROM Users;", query);
	}

	[Fact]
	public void GenerateGetAllQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal("SELECT Users.Age, Users.Id_Password, Users.Id_Username FROM Users;", query);
	}

	[Fact]
	public void GenerateGetAllQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal(
			"SELECT Users.Id, Users.DeliveryAddress_City, Users.DeliveryAddress_Street, Users.InvoiceAddress_City, Users.InvoiceAddress_Street FROM Users;",
			query);
	}

	[Fact]
	public void GenerateGetAllQuery_ProperTableName_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var selectQuery = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal("SELECT Users.Id, Users.Username, Users.Password FROM Users;", selectQuery);
	}

	#endregion

	#region Get

	[Fact]
	public void GenerateGetQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal("SELECT Users.Id, Users.FirstLevel_SecondLevel_Name FROM Users WHERE Users.Id = @Id;", query);
	}

	[Fact]
	public void GenerateGetQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT Users.Age, Users.Id_Password, Users.Id_Username FROM Users WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT Users.Id, Users.DeliveryAddress_City, Users.DeliveryAddress_Street, Users.InvoiceAddress_City, Users.InvoiceAddress_Street FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_SinglePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var selectQuery = generator.GenerateGetQuery();

		// Assert
		Assert.Equal("SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;",
			selectQuery);
	}

	[Fact]
	public void GenerateGetQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var selectQuery = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;",
			selectQuery);
	}

	#endregion

	#region Insert

	[Fact]
	public void GenerateInsertQuery_HasMultipleIdentityProperties_Throws()
	{
		// Arrange
		var defaultConfig = new DefaultConfiguration();
		var config = new TableAggregateConfiguration<AggregateWithNestedValueObject> { TableName = "Users" };
		config.HasKey(x => x.Id);
		config.HasIdentity(x => x.Id);
		config.HasIdentity(x => x.FirstLevel);
		config.SetDefaults(defaultConfig);
		var generator = new MySqlQueryGenerator<AggregateWithNestedValueObject>(config);

		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => generator.GenerateInsertQuery(new AggregateWithNestedValueObject(
			Guid.NewGuid(),
			new FirstLevelValueObject(new SecondLevelValueObject("Hello world")))));

		Assert.Equal("Cannot generate INSERT query for table with multiple identity properties", ex.Message);
	}

	[Fact]
	public void GenerateInsertQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new AggregateWithNestedValueObject(Guid.NewGuid(),
			new FirstLevelValueObject(new SecondLevelValueObject("Hello world"))));

		// Assert
		Assert.Equal(
			"INSERT INTO Users (Id, FirstLevel_SecondLevel_Name) VALUES (@Id, @FirstLevel_SecondLevel_Name);SELECT Users.Id, Users.FirstLevel_SecondLevel_Name FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new AggregateWithValueObjectId());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Age, Id_Password, Id_Username) VALUES (@Age, @Id_Password, @Id_Username);SELECT Users.Age, Users.Id_Password, Users.Id_Username FROM Users WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new UserAggregate());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Id, DeliveryAddress_City, DeliveryAddress_Street, InvoiceAddress_City, InvoiceAddress_Street) VALUES (@Id, @DeliveryAddress_City, @DeliveryAddress_Street, @InvoiceAddress_City, @InvoiceAddress_Street);SELECT Users.Id, Users.DeliveryAddress_City, Users.DeliveryAddress_Street, Users.InvoiceAddress_City, Users.InvoiceAddress_Street FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_AggregateHasMultipleIdentities_Invalid()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<HasMultipleIdentiesAggregate> { TableName = "Users" };
		configuration.HasKey(x => x.Id);
		configuration.HasIdentity(x => x.Id);
		configuration.HasIdentity(x => x.Counter);
		var generator = new MySqlQueryGenerator<HasMultipleIdentiesAggregate>(configuration);

		var aggregate = new HasMultipleIdentiesAggregate();

		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => generator.GenerateInsertQuery(aggregate));
		Assert.Equal("Cannot generate INSERT query for table with multiple identity properties", ex.Message);
	}

	[Fact]
	public void GenerateInsertQuery_PropertyHasDefaultConstraintAndDefaultValue_Valid()
	{
		// Arrange
		var generator = CreateHasDefaultConstraintAggregateQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new HasDefaultConstraintAggregate());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Id) VALUES (@Id);SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_PropertyHasDefaultConstraintAndNonDefaultValue_Valid()
	{
		// Arrange
		var generator = CreateHasDefaultConstraintAggregateQueryGenerator();
		var record = new HasDefaultConstraintAggregate { Id = 42, DateCreated = DateTime.Now };

		// Act
		var query = generator.GenerateInsertQuery(record);

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Id, DateCreated) VALUES (@Id, @DateCreated);SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_IdentityValuePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Username, Password) VALUES (@Username, @Password);SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = LAST_INSERT_ID();",
			insertQuery);
	}

	[Fact]
	public void GenerateInsertQuery_MissingPropertyValue_ContainsProperty()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;",
			insertQuery);
	}

	[Fact]
	public void GenerateInsertQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;",
			insertQuery);
	}

	#endregion

	#region Update

	[Fact]
	public void GenerateUpdateQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithNestedValueObject(Guid.NewGuid(),
			new FirstLevelValueObject(new SecondLevelValueObject("Hello world"))));

		// Assert
		Assert.Equal(
			"UPDATE Users SET FirstLevel_SecondLevel_Name = @FirstLevel_SecondLevel_Name WHERE Users.Id = @Id;SELECT Users.Id, Users.FirstLevel_SecondLevel_Name FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithValueObjectId());

		// Assert
		Assert.Equal(
			@"UPDATE Users SET Age = @Age WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;SELECT Users.Age, Users.Id_Password, Users.Id_Username FROM Users WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new UserAggregate());

		// Assert
		Assert.Equal(
			@"UPDATE Users SET DeliveryAddress_City = @DeliveryAddress_City, DeliveryAddress_Street = @DeliveryAddress_Street, InvoiceAddress_City = @InvoiceAddress_City, InvoiceAddress_Street = @InvoiceAddress_Street WHERE Users.Id = @Id;SELECT Users.Id, Users.DeliveryAddress_City, Users.DeliveryAddress_Street, Users.InvoiceAddress_City, Users.InvoiceAddress_Street FROM Users WHERE Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_SinglePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act 
		var updateQuery = generator.GenerateUpdateQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			@"UPDATE Users SET Username = @Username, Password = @Password WHERE Users.Id = @Id;SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;",
			updateQuery);
	}

	[Fact]
	public void GenerateUpdateQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act 
		var updateQuery = generator.GenerateUpdateQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			@"UPDATE Users SET DateCreated = @DateCreated WHERE Users.Username = @Username AND Users.Password = @Password;SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;",
			updateQuery);
	}

	[Fact]
	public void GenerateUpdateQuery_AllPropertiesHasNoSetter_Throws()
	{
		// Arrange
		var configuration =
			new TableAggregateConfiguration<AllPropertiesHasMissingSetterAggregate> { TableName = "Users" };
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new MySqlQueryGenerator<AllPropertiesHasMissingSetterAggregate>(configuration);

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() =>
			generator.GenerateUpdateQuery(new AllPropertiesHasMissingSetterAggregate()));
	}

	[Fact]
	public void GenerateUpdateQuery_PropertyHasNoSetter_PropertyIsExcluded()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<AggregateWithDefaultConstraint> { TableName = "Users" };
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new MySqlQueryGenerator<AggregateWithDefaultConstraint>(configuration);

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithDefaultConstraint());

		// Assert
		Assert.Equal(
			@"UPDATE Users SET Age = @Age WHERE Users.Id = @Id;SELECT Users.Id, Users.Age, Users.DateCreated FROM Users WHERE Users.Id = @Id;",
			query);
	}

	#endregion

	#region Upsert

	[Fact]
	public void GenerateUpsertQuery_HasIdentity_ReturnsPureInsertQuery()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var query = generator.GenerateUpsertQuery(new SinglePrimaryKeyAggregate());
		var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(insertQuery, query);
	}

	[Fact]
	public void GenerateUpsertQuery_HasIdentityWithExistingValue_ReturnsPureUpdateQuery()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var query = generator.GenerateUpsertQuery(new SinglePrimaryKeyAggregate { Id = 42 });
		var updateQuery = generator.GenerateUpdateQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(updateQuery, query);
	}

	[Fact]
	public void GenerateUpsertQuery_HasNoIdentity_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateUpsertQuery(new AggregateWithValueObjectId());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Age, Id_Password, Id_Username) VALUES (@Age, @Id_Password, @Id_Username) ON DUPLICATE KEY UPDATE Age = @Age;SELECT Users.Age, Users.Id_Password, Users.Id_Username FROM Users WHERE Users.Id_Password = @Id_Password AND Users.Id_Username = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateUpsertQuery_HasNoUpdatableColumns_Valid()
	{
		// Arrange
		var configuration =
			new TableAggregateConfiguration<AllPropertiesHasMissingSetterAggregate> { TableName = "Users" };
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new MySqlQueryGenerator<AllPropertiesHasMissingSetterAggregate>(configuration);

		// Act
		var query = generator.GenerateUpsertQuery(new AllPropertiesHasMissingSetterAggregate());

		// Assert
		Assert.Equal(
			@"INSERT INTO Users (Id) VALUES (@Id);SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;",
			query);
	}

	#endregion

	#region Constructors

	private static MySqlQueryGenerator<HasDefaultConstraintAggregate>
		CreateHasDefaultConstraintAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<HasDefaultConstraintAggregate> { TableName = "Users" };
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new MySqlQueryGenerator<HasDefaultConstraintAggregate>(configuration);
		return generator;
	}

	private static MySqlQueryGenerator<SinglePrimaryKeyAggregate> CreateSinglePrimaryKeyAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate> { TableName = "Users" };
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasIdentity(aggregate => aggregate.Id);
		var generator = new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static MySqlQueryGenerator<CompositePrimaryKeyAggregate>
		CreateCompositePrimaryKeyAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<CompositePrimaryKeyAggregate> { TableName = "Users" };
		configuration.HasKey(aggregate => new { aggregate.Username, aggregate.Password });
		var generator = new MySqlQueryGenerator<CompositePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static MySqlQueryGenerator<UserAggregate> CreateUserAggregateQueryGenerator()
	{
		var config = new TableAggregateConfiguration<UserAggregate> { TableName = "Users" };
		config.HasKey(x => x.Id);
		var generator = new MySqlQueryGenerator<UserAggregate>(config);
		return generator;
	}

	private static MySqlQueryGenerator<AggregateWithValueObjectId> CreateAggregateWithValueObjectIdQueryGenerator()
	{
		var config = new TableAggregateConfiguration<AggregateWithValueObjectId> { TableName = "Users" };
		config.HasKey(x => x.Id);
		var generator = new MySqlQueryGenerator<AggregateWithValueObjectId>(config);
		return generator;
	}

	private static MySqlQueryGenerator<AggregateWithNestedValueObject>
		CreateAggregateWithNestedValueObjectGenerator()
	{
		var defaultConfig = new DefaultConfiguration();
		var config = new TableAggregateConfiguration<AggregateWithNestedValueObject> { TableName = "Users" };
		config.HasKey(x => x.Id);
		config.SetDefaults(defaultConfig);
		var generator = new MySqlQueryGenerator<AggregateWithNestedValueObject>(config);
		return generator;
	}

	#endregion
}