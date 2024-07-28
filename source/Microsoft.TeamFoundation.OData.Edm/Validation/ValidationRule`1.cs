// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationRule`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Validation
{
  public sealed class ValidationRule<TItem> : ValidationRule where TItem : IEdmElement
  {
    private readonly Action<ValidationContext, TItem> validate;

    public ValidationRule(Action<ValidationContext, TItem> validate) => this.validate = validate;

    internal override Type ValidatedType => typeof (TItem);

    internal override void Evaluate(ValidationContext context, object item)
    {
      TItem obj = (TItem) item;
      this.validate(context, obj);
    }
  }
}
