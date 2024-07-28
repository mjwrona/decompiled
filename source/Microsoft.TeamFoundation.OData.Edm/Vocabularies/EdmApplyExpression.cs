// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmApplyExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmApplyExpression : EdmElement, IEdmApplyExpression, IEdmExpression, IEdmElement
  {
    private readonly IEdmFunction appliedFunction;
    private readonly IEnumerable<IEdmExpression> arguments;

    public EdmApplyExpression(IEdmFunction appliedFunction, params IEdmExpression[] arguments)
      : this(appliedFunction, (IEnumerable<IEdmExpression>) arguments)
    {
    }

    public EdmApplyExpression(IEdmFunction appliedFunction, IEnumerable<IEdmExpression> arguments)
    {
      EdmUtil.CheckArgumentNull<IEdmFunction>(appliedFunction, nameof (appliedFunction));
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmExpression>>(arguments, nameof (arguments));
      this.appliedFunction = appliedFunction;
      this.arguments = arguments;
    }

    public IEdmFunction AppliedFunction => this.appliedFunction;

    public IEnumerable<IEdmExpression> Arguments => this.arguments;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.FunctionApplication;
  }
}
