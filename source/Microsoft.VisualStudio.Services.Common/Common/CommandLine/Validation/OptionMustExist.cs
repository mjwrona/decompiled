// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.OptionMustExist
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common.CommandLine.Validation
{
  public class OptionMustExist : OptionValidation
  {
    public OptionMustExist(string dependentOption)
      : base(dependentOption)
    {
      this.DependencyFailedAction = (Action<OptionValidation>) (dependency =>
      {
        throw new OptionValidationException(CommonResources.ErrorOptionMustExist((object) this.DependentOption));
      });
    }

    public OptionMustExist(string dependentOption, Action<OptionValidation> dependencyFailedAction)
      : base(dependentOption, dependencyFailedAction)
    {
    }

    public override void Validate(IEnumerable<Option> options, IEnumerable<Argument> arguments)
    {
      if (string.IsNullOrWhiteSpace(this.DependentOption) || options.Get(this.DependentOption) != null)
        return;
      this.DependencyFailedAction((OptionValidation) this);
    }
  }
}
