// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.OptionValidation
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common.CommandLine.Validation
{
  public abstract class OptionValidation : IOptionValidation
  {
    protected OptionValidation()
    {
    }

    protected OptionValidation(string dependentOption) => this.DependentOption = dependentOption;

    protected OptionValidation(
      string dependentOption,
      Action<OptionValidation> dependencyFailedAction)
    {
      if (dependencyFailedAction == null)
        throw new ArgumentNullException(nameof (dependencyFailedAction));
      this.DependentOption = dependentOption;
      this.DependencyFailedAction = dependencyFailedAction;
    }

    public string DependentOption { get; set; }

    public Action<OptionValidation> DependencyFailedAction { get; set; }

    public static OptionValidation Default => (OptionValidation) DefaultValidation.Instance;

    public abstract void Validate(IEnumerable<Option> options, IEnumerable<Argument> arguments);
  }
}
