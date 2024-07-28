// Decompiled with JetBrains decompiler
// Type: Nest.MatchNoneQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MatchNoneQuery : QueryBase, IMatchNoneQuery, IQuery
  {
    protected override bool Conditionless => false;

    internal override void InternalWrapInContainer(IQueryContainer container) => container.MatchNone = (IMatchNoneQuery) this;
  }
}
