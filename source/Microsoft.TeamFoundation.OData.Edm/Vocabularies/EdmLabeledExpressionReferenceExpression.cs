// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmLabeledExpressionReferenceExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmLabeledExpressionReferenceExpression : 
    EdmElement,
    IEdmLabeledExpressionReferenceExpression,
    IEdmExpression,
    IEdmElement
  {
    private IEdmLabeledExpression referencedLabeledExpression;

    public EdmLabeledExpressionReferenceExpression()
    {
    }

    public EdmLabeledExpressionReferenceExpression(IEdmLabeledExpression referencedLabeledExpression)
    {
      EdmUtil.CheckArgumentNull<IEdmLabeledExpression>(referencedLabeledExpression, nameof (referencedLabeledExpression));
      this.referencedLabeledExpression = referencedLabeledExpression;
    }

    public IEdmLabeledExpression ReferencedLabeledExpression
    {
      get => this.referencedLabeledExpression;
      set
      {
        EdmUtil.CheckArgumentNull<IEdmLabeledExpression>(value, nameof (value));
        this.referencedLabeledExpression = this.referencedLabeledExpression == null ? value : throw new InvalidOperationException(Strings.ValueHasAlreadyBeenSet);
      }
    }

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.LabeledExpressionReference;
  }
}
