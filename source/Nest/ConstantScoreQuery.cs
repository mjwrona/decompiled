// Decompiled with JetBrains decompiler
// Type: Nest.ConstantScoreQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ConstantScoreQuery : QueryBase, IConstantScoreQuery, IQuery
  {
    public QueryContainer Filter { get; set; }

    protected override bool Conditionless => ConstantScoreQuery.IsConditionless((IConstantScoreQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.ConstantScore = (IConstantScoreQuery) this;

    internal static bool IsConditionless(IConstantScoreQuery q) => q.Filter.NotWritable();
  }
}
