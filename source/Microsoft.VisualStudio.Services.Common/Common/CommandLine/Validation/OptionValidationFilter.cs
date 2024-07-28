// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.OptionValidationFilter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common.CommandLine.Validation
{
  public abstract class OptionValidationFilter : OptionValidation
  {
    protected OptionValidationFilter(OptionValidation dependency) => this.Dependency = (IOptionValidation) dependency;

    protected OptionValidationFilter(string dependentOption, OptionValidation dependency)
      : base(dependentOption)
    {
      this.Dependency = (IOptionValidation) dependency;
    }

    protected IOptionValidation Dependency { get; private set; }

    public abstract bool ShouldValidate(
      IEnumerable<Option> options,
      IEnumerable<Argument> arguments);

    public override void Validate(IEnumerable<Option> options, IEnumerable<Argument> arguments)
    {
      if (!this.ShouldValidate(options, arguments))
        return;
      this.Dependency.Validate(options, arguments);
    }
  }
}
