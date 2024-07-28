// Decompiled with JetBrains decompiler
// Type: Nest.CombinedFieldsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CombinedFieldsQuery : QueryBase, ICombinedFieldsQuery, IQuery
  {
    public string Query { get; set; }

    public Fields Fields { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    public Nest.Operator? Operator { get; set; }

    public Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }

    protected override bool Conditionless => CombinedFieldsQuery.IsConditionless((ICombinedFieldsQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.CombinedFields = (ICombinedFieldsQuery) this;

    internal static bool IsConditionless(ICombinedFieldsQuery q) => q.Fields.IsConditionless() || q.Query.IsNullOrEmpty();
  }
}
