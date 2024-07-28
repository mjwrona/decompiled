// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.ComputeTransformationNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class ComputeTransformationNode : TransformationNode
  {
    private readonly IEnumerable<ComputeExpression> expressions;

    public ComputeTransformationNode(IEnumerable<ComputeExpression> expressions)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<ComputeExpression>>(expressions, nameof (expressions));
      this.expressions = expressions;
    }

    public IEnumerable<ComputeExpression> Expressions => this.expressions;

    public override TransformationNodeKind Kind => TransformationNodeKind.Compute;
  }
}
