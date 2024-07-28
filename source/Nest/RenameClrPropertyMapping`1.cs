// Decompiled with JetBrains decompiler
// Type: Nest.RenameClrPropertyMapping`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RenameClrPropertyMapping<TDocument> : ClrPropertyMappingBase<TDocument> where TDocument : class
  {
    public RenameClrPropertyMapping(Expression<Func<TDocument, object>> property, string newName)
      : base(property)
    {
      newName.ThrowIfNull<string>(nameof (newName));
      this.Self.NewName = newName;
    }
  }
}
