// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.ExpandTransformationNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class ExpandTransformationNode : TransformationNode
  {
    private readonly SelectExpandClause expandClause;

    public ExpandTransformationNode(SelectExpandClause expandClause)
    {
      ExceptionUtils.CheckArgumentNotNull<SelectExpandClause>(expandClause, nameof (expandClause));
      this.expandClause = expandClause;
    }

    public SelectExpandClause ExpandClause => this.expandClause;

    public override TransformationNodeKind Kind => TransformationNodeKind.Expand;
  }
}
