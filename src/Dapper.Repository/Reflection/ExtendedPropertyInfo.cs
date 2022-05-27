using System.Reflection;

namespace Dapper.Repository.Reflection;

public class ExtendedPropertyInfo
{
	public PropertyInfo Property { get; }
	public string Name => Property.Name;
	public Type Type => Property.PropertyType;

	public bool HasSetter { get; }

	private readonly object? _defaultValue;
	private readonly MemberAccessor _accessor;

	public ExtendedPropertyInfo(PropertyInfo property)
	{
		Property = property;
		var type = property.PropertyType;

		_accessor = new MemberAccessor(property);
		_defaultValue = TypeDefaultValueCache.GetDefaultValue(type);
		HasSetter = _accessor.HasSetter;
	}

	public bool HasDefaultValue<T>(T aggregate)
	where T : notnull
	{
		var value = GetValue(aggregate);

		return value == _defaultValue || value?.Equals(_defaultValue) == true;
	}

	public object GetValue<T>(T aggregate)
	where T : notnull
	{
		return _accessor.getter(aggregate);
	}
}