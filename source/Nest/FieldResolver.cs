// Decompiled with JetBrains decompiler
// Type: Nest.FieldResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  public class FieldResolver
  {
    protected readonly ConcurrentDictionary<Field, string> Fields = new ConcurrentDictionary<Field, string>();
    protected readonly ConcurrentDictionary<PropertyName, string> Properties = new ConcurrentDictionary<PropertyName, string>();
    private readonly IConnectionSettingsValues _settings;

    public FieldResolver(IConnectionSettingsValues settings)
    {
      settings.ThrowIfNull<IConnectionSettingsValues>(nameof (settings));
      this._settings = settings;
    }

    public string Resolve(Field field)
    {
      string str = this.ResolveFieldName(field);
      if (field.Boost.HasValue)
        str = str + "^" + field.Boost.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return str;
    }

    private string ResolveFieldName(Field field)
    {
      if (field.IsConditionless())
        return (string) null;
      if (!field.Name.IsNullOrEmpty())
        return field.Name;
      if (field.Expression != null && !field.CachableExpression)
        return this.Resolve(field.Expression, (MemberInfo) field.Property);
      string str1;
      if (this.Fields.TryGetValue(field, out str1))
        return str1;
      string str2 = this.Resolve(field.Expression, (MemberInfo) field.Property);
      this.Fields.TryAdd(field, str2);
      return str2;
    }

    public string Resolve(PropertyName property)
    {
      if (property.IsConditionless())
        return (string) null;
      if (!property.Name.IsNullOrEmpty())
        return property.Name;
      if (property.Expression != null && !property.CacheableExpression)
        return this.Resolve(property.Expression, (MemberInfo) property.Property);
      string str1;
      if (this.Properties.TryGetValue(property, out str1))
        return str1;
      string str2 = this.Resolve(property.Expression, (MemberInfo) property.Property, true);
      this.Properties.TryAdd(property, str2);
      return str2;
    }

    private string Resolve(Expression expression, MemberInfo member, bool toLastToken = false)
    {
      FieldExpressionVisitor expressionVisitor = new FieldExpressionVisitor(this._settings);
      return (expression != null ? expressionVisitor.Resolve(expression, toLastToken) : (member != (MemberInfo) null ? expressionVisitor.Resolve(member) : (string) null)) ?? throw new ArgumentException("Name resolved to null for the given Expression or MemberInfo.");
    }
  }
}
