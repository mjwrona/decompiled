// Decompiled with JetBrains decompiler
// Type: Nest.WildcardQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class WildcardQuery : 
    FieldNameQueryBase,
    IWildcardQuery,
    ITermQuery,
    IFieldNameQuery,
    IQuery
  {
    public MultiTermQueryRewrite Rewrite { get; set; }

    public object Value { get; set; }

    public bool? CaseInsensitive { get; set; }

    public string Wildcard { get; set; }

    protected override bool Conditionless => WildcardQuery.IsConditionless((IWildcardQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Wildcard = (IWildcardQuery) this;

    internal static bool IsConditionless(IWildcardQuery q)
    {
      if (q.Value != null || q.Wildcard != null)
      {
        object obj = q.Value;
        if ((obj != null ? (obj.ToString().IsNullOrEmpty() ? 1 : 0) : 1) != 0)
        {
          string wildcard = q.Wildcard;
          if ((wildcard != null ? (wildcard.ToString().IsNullOrEmpty() ? 1 : 0) : 1) != 0)
            goto label_4;
        }
        return q.Field.IsConditionless();
      }
label_4:
      return true;
    }
  }
}
