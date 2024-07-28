// Decompiled with JetBrains decompiler
// Type: Nest.RawQueryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RawQueryDescriptor : 
    QueryDescriptorBase<RawQueryDescriptor, IRawQuery>,
    IRawQuery,
    IQuery
  {
    public RawQueryDescriptor()
    {
    }

    public RawQueryDescriptor(string rawQuery) => this.Self.Raw = rawQuery;

    protected override bool Conditionless => this.Self.Raw.IsNullOrEmpty();

    string IRawQuery.Raw { get; set; }

    public RawQueryDescriptor Raw(string rawQuery) => this.Assign<string>(rawQuery, (Action<IRawQuery, string>) ((a, v) => a.Raw = v));
  }
}
