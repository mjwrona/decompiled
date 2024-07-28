// Decompiled with JetBrains decompiler
// Type: Nest.DisMaxQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DisMaxQuery : QueryBase, IDisMaxQuery, IQuery
  {
    public IEnumerable<QueryContainer> Queries { get; set; }

    public double? TieBreaker { get; set; }

    protected override bool Conditionless => DisMaxQuery.IsConditionless((IDisMaxQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.DisMax = (IDisMaxQuery) this;

    internal static bool IsConditionless(IDisMaxQuery q) => q.Queries.NotWritable();
  }
}
