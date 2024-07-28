// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils.Validator`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils
{
  public abstract class Validator<T> : IValidator<T>
  {
    private readonly Func<T, bool> isValid;
    private readonly Action<T> actionIfNotValid;

    protected Validator(Func<T, bool> isValid, Action<T> actionIfNotValid)
    {
      this.isValid = isValid;
      this.actionIfNotValid = actionIfNotValid;
    }

    public void Validate(T valueToValidate)
    {
      if ((object) valueToValidate == null || this.isValid(valueToValidate))
        return;
      Action<T> actionIfNotValid = this.actionIfNotValid;
      if (actionIfNotValid == null)
        return;
      actionIfNotValid(valueToValidate);
    }
  }
}
