// Decompiled with JetBrains decompiler
// Type: Nest.ClrTypeMappingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class ClrTypeMappingDescriptor<TDocument> : 
    DescriptorBase<ClrTypeMappingDescriptor<TDocument>, IClrTypeMapping<TDocument>>,
    IClrTypeMapping<TDocument>,
    IClrTypeMapping
    where TDocument : class
  {
    Type IClrTypeMapping.ClrType { get; } = typeof (TDocument);

    Expression<Func<TDocument, object>> IClrTypeMapping<TDocument>.IdProperty { get; set; }

    string IClrTypeMapping.IdPropertyName { get; set; }

    string IClrTypeMapping.IndexName { get; set; }

    IList<IClrPropertyMapping<TDocument>> IClrTypeMapping<TDocument>.Properties { get; set; } = (IList<IClrPropertyMapping<TDocument>>) new List<IClrPropertyMapping<TDocument>>();

    string IClrTypeMapping.RelationName { get; set; }

    Expression<Func<TDocument, object>> IClrTypeMapping<TDocument>.RoutingProperty { get; set; }

    bool IClrTypeMapping.DisableIdInference { get; set; }

    public ClrTypeMappingDescriptor<TDocument> IndexName(string indexName) => this.Assign<string>(indexName, (Action<IClrTypeMapping<TDocument>, string>) ((a, v) => a.IndexName = v));

    public ClrTypeMappingDescriptor<TDocument> RelationName(string relationName) => this.Assign<string>(relationName, (Action<IClrTypeMapping<TDocument>, string>) ((a, v) => a.RelationName = v));

    public ClrTypeMappingDescriptor<TDocument> IdProperty(
      Expression<Func<TDocument, object>> property)
    {
      return this.Assign<Expression<Func<TDocument, object>>>(property, (Action<IClrTypeMapping<TDocument>, Expression<Func<TDocument, object>>>) ((a, v) => a.IdProperty = v));
    }

    public ClrTypeMappingDescriptor<TDocument> IdProperty(string property) => this.Assign<string>(property, (Action<IClrTypeMapping<TDocument>, string>) ((a, v) => a.IdPropertyName = v));

    public ClrTypeMappingDescriptor<TDocument> RoutingProperty(
      Expression<Func<TDocument, object>> property)
    {
      return this.Assign<Expression<Func<TDocument, object>>>(property, (Action<IClrTypeMapping<TDocument>, Expression<Func<TDocument, object>>>) ((a, v) => a.RoutingProperty = v));
    }

    public ClrTypeMappingDescriptor<TDocument> Ignore(Expression<Func<TDocument, object>> property) => this.Assign<Expression<Func<TDocument, object>>>(property, (Action<IClrTypeMapping<TDocument>, Expression<Func<TDocument, object>>>) ((a, v) => a.Properties.Add((IClrPropertyMapping<TDocument>) new IgnoreClrPropertyMapping<TDocument>(v))));

    public ClrTypeMappingDescriptor<TDocument> PropertyName(
      Expression<Func<TDocument, object>> property,
      string newName)
    {
      return this.Assign<RenameClrPropertyMapping<TDocument>>(new RenameClrPropertyMapping<TDocument>(property, newName), (Action<IClrTypeMapping<TDocument>, RenameClrPropertyMapping<TDocument>>) ((a, v) => a.Properties.Add((IClrPropertyMapping<TDocument>) v)));
    }

    public ClrTypeMappingDescriptor<TDocument> DisableIdInference(bool disable = true) => this.Assign<bool>(disable, (Action<IClrTypeMapping<TDocument>, bool>) ((a, v) => a.DisableIdInference = v));
  }
}
