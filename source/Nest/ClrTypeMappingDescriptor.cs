// Decompiled with JetBrains decompiler
// Type: Nest.ClrTypeMappingDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ClrTypeMappingDescriptor : 
    DescriptorBase<ClrTypeMappingDescriptor, IClrTypeMapping>,
    IClrTypeMapping
  {
    private readonly Type _type;

    public ClrTypeMappingDescriptor(Type type) => this._type = type;

    Type IClrTypeMapping.ClrType => this._type;

    string IClrTypeMapping.IdPropertyName { get; set; }

    string IClrTypeMapping.IndexName { get; set; }

    string IClrTypeMapping.RelationName { get; set; }

    bool IClrTypeMapping.DisableIdInference { get; set; }

    public ClrTypeMappingDescriptor IndexName(string indexName) => this.Assign<string>(indexName, (Action<IClrTypeMapping, string>) ((a, v) => a.IndexName = v));

    public ClrTypeMappingDescriptor RelationName(string relationName) => this.Assign<string>(relationName, (Action<IClrTypeMapping, string>) ((a, v) => a.RelationName = v));

    public ClrTypeMappingDescriptor IdProperty(string idProperty) => this.Assign<string>(idProperty, (Action<IClrTypeMapping, string>) ((a, v) => a.IdPropertyName = v));

    public ClrTypeMappingDescriptor DisableIdInference(bool disable = true) => this.Assign<bool>(disable, (Action<IClrTypeMapping, bool>) ((a, v) => a.DisableIdInference = v));
  }
}
