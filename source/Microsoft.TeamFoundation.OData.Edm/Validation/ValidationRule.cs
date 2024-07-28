// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationRule
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Validation
{
  public abstract class ValidationRule
  {
    internal abstract Type ValidatedType { get; }

    internal abstract void Evaluate(ValidationContext context, object item);
  }
}
