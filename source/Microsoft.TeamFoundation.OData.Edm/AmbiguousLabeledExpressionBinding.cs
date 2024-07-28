// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousLabeledExpressionBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousLabeledExpressionBinding : 
    AmbiguousBinding<IEdmLabeledExpression>,
    IEdmLabeledExpression,
    IEdmNamedElement,
    IEdmElement,
    IEdmExpression
  {
    private readonly Cache<AmbiguousLabeledExpressionBinding, IEdmExpression> expressionCache = new Cache<AmbiguousLabeledExpressionBinding, IEdmExpression>();
    private static readonly Func<AmbiguousLabeledExpressionBinding, IEdmExpression> ComputeExpressionFunc = (Func<AmbiguousLabeledExpressionBinding, IEdmExpression>) (me => AmbiguousLabeledExpressionBinding.ComputeExpression());

    public AmbiguousLabeledExpressionBinding(
      IEdmLabeledExpression first,
      IEdmLabeledExpression second)
      : base(first, second)
    {
    }

    public IEdmExpression Expression => this.expressionCache.GetValue(this, AmbiguousLabeledExpressionBinding.ComputeExpressionFunc, (Func<AmbiguousLabeledExpressionBinding, IEdmExpression>) null);

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Labeled;

    private static IEdmExpression ComputeExpression() => (IEdmExpression) EdmNullExpression.Instance;
  }
}
