// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.GroupByTransformationNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class GroupByTransformationNode : TransformationNode
  {
    private readonly CollectionNode source;
    private readonly TransformationNode childTransformations;
    private readonly IEnumerable<GroupByPropertyNode> groupingProperties;

    public GroupByTransformationNode(
      IList<GroupByPropertyNode> groupingProperties,
      TransformationNode childTransformations,
      CollectionNode source)
    {
      ExceptionUtils.CheckArgumentNotNull<IList<GroupByPropertyNode>>(groupingProperties, nameof (groupingProperties));
      this.groupingProperties = (IEnumerable<GroupByPropertyNode>) groupingProperties;
      this.childTransformations = childTransformations;
      this.source = source;
    }

    public IEnumerable<GroupByPropertyNode> GroupingProperties => this.groupingProperties;

    public TransformationNode ChildTransformations => this.childTransformations;

    public CollectionNode Source => this.source;

    public override TransformationNodeKind Kind => TransformationNodeKind.GroupBy;
  }
}
