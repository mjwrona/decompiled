// Decompiled with JetBrains decompiler
// Type: Nest.ClrPropertyMappingBase`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class ClrPropertyMappingBase<TDocument> : IClrPropertyMapping<TDocument> where TDocument : class
  {
    protected ClrPropertyMappingBase(Expression<Func<TDocument, object>> property) => this.Self.Property = property;

    protected IClrPropertyMapping<TDocument> Self => (IClrPropertyMapping<TDocument>) this;

    bool IClrPropertyMapping<TDocument>.Ignore { get; set; }

    string IClrPropertyMapping<TDocument>.NewName { get; set; }

    Expression<Func<TDocument, object>> IClrPropertyMapping<TDocument>.Property { get; set; }

    IPropertyMapping IClrPropertyMapping<TDocument>.ToPropertyMapping()
    {
      if (this.Self.Ignore)
        return (IPropertyMapping) PropertyMapping.Ignored;
      return (IPropertyMapping) new PropertyMapping()
      {
        Name = this.Self.NewName
      };
    }
  }
}
