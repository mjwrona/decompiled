// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadLabeledExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class BadLabeledExpression : 
    BadElement,
    IEdmLabeledExpression,
    IEdmNamedElement,
    IEdmElement,
    IEdmExpression
  {
    private readonly string name;
    private readonly Cache<BadLabeledExpression, IEdmExpression> expressionCache = new Cache<BadLabeledExpression, IEdmExpression>();
    private static readonly Func<BadLabeledExpression, IEdmExpression> ComputeExpressionFunc = (Func<BadLabeledExpression, IEdmExpression>) (me => BadLabeledExpression.ComputeExpression());

    public BadLabeledExpression(string name, IEnumerable<EdmError> errors)
      : base(errors)
    {
      this.name = name ?? string.Empty;
    }

    public string Name => this.name;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Labeled;

    public IEdmExpression Expression => this.expressionCache.GetValue(this, BadLabeledExpression.ComputeExpressionFunc, (Func<BadLabeledExpression, IEdmExpression>) null);

    private static IEdmExpression ComputeExpression() => (IEdmExpression) EdmNullExpression.Instance;
  }
}
