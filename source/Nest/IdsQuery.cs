// Decompiled with JetBrains decompiler
// Type: Nest.IdsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class IdsQuery : QueryBase, IIdsQuery, IQuery
  {
    public IEnumerable<Id> Values { get; set; }

    protected override bool Conditionless => IdsQuery.IsConditionless((IIdsQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Ids = (IIdsQuery) this;

    internal static bool IsConditionless(IIdsQuery q) => !q.Values.HasAny<Id>();
  }
}
