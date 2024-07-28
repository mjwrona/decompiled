// Decompiled with JetBrains decompiler
// Type: Nest.ScriptScoreQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ScriptScoreQuery : QueryBase, IScriptScoreQuery, IQuery
  {
    public QueryContainer Query { get; set; }

    public IScript Script { get; set; }

    protected override bool Conditionless => ScriptScoreQuery.IsConditionless((IScriptScoreQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.ScriptScore = (IScriptScoreQuery) this;

    internal static bool IsConditionless(IScriptScoreQuery q)
    {
      if (q.Script == null || q.Query.IsConditionless())
        return true;
      switch (q.Script)
      {
        case IInlineScript inlineScript:
          return inlineScript.Source.IsNullOrEmpty();
        case IIndexedScript indexedScript:
          return indexedScript.Id.IsNullOrEmpty();
        default:
          return false;
      }
    }
  }
}
