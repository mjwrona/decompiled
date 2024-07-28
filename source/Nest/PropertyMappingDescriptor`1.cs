// Decompiled with JetBrains decompiler
// Type: Nest.PropertyMappingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class PropertyMappingDescriptor<TDocument> : 
    DescriptorBase<PropertyMappingDescriptor<TDocument>, IDescriptor>
    where TDocument : class
  {
    internal IList<IClrPropertyMapping<TDocument>> Mappings { get; } = (IList<IClrPropertyMapping<TDocument>>) new List<IClrPropertyMapping<TDocument>>();

    public PropertyMappingDescriptor<TDocument> PropertyName(
      Expression<Func<TDocument, object>> property,
      string field)
    {
      property.ThrowIfNull<Expression<Func<TDocument, object>>>(nameof (property));
      field.ThrowIfNullOrEmpty(nameof (field));
      this.Mappings.Add((IClrPropertyMapping<TDocument>) new RenameClrPropertyMapping<TDocument>(property, field));
      return this;
    }

    public PropertyMappingDescriptor<TDocument> Ignore(Expression<Func<TDocument, object>> property)
    {
      property.ThrowIfNull<Expression<Func<TDocument, object>>>(nameof (property));
      this.Mappings.Add((IClrPropertyMapping<TDocument>) new IgnoreClrPropertyMapping<TDocument>(property));
      return this;
    }
  }
}
