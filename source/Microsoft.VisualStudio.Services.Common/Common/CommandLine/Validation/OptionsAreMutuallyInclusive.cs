// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.OptionsAreMutuallyInclusive
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine.Validation
{
  public sealed class OptionsAreMutuallyInclusive : OptionValidation
  {
    private Collection<string> dependencyOptions;

    public OptionsAreMutuallyInclusive(IEnumerable<string> dependencyOptions)
    {
      this.SetDependencyOptions(dependencyOptions);
      this.DependencyFailedAction = (Action<OptionValidation>) (dependency =>
      {
        throw new OptionValidationException(CommonResources.ErrorOptionsAreMutuallyInclusive((object) string.Join(", ", (IEnumerable<string>) this.dependencyOptions)));
      });
    }

    public OptionsAreMutuallyInclusive(
      IEnumerable<string> dependencyOptions,
      Action<OptionValidation> dependencyFailedAction)
      : base((string) null, dependencyFailedAction)
    {
      this.SetDependencyOptions(dependencyOptions);
    }

    public override void Validate(IEnumerable<Option> options, IEnumerable<Argument> arguments)
    {
      if (options == null || arguments == null || this.dependencyOptions == null || !this.dependencyOptions.Any<string>())
        return;
      HashSet<string> stringSet = new HashSet<string>();
      try
      {
        int num = 0;
        foreach (string dependencyOption in this.dependencyOptions)
        {
          Option option = options.Get(dependencyOption);
          if (option != null)
          {
            stringSet.Add(option.Name);
            if (OptionsAreMutuallyInclusive.IsOptionDefined(option, arguments))
              ++num;
          }
        }
        if (num <= 0 || num == stringSet.Count)
          return;
        this.DependencyFailedAction((OptionValidation) this);
      }
      finally
      {
        stringSet.Clear();
      }
    }

    private void SetDependencyOptions(IEnumerable<string> options)
    {
      if (options == null)
        return;
      this.dependencyOptions = new Collection<string>();
      foreach (string option in options)
        this.dependencyOptions.Add(option);
    }

    private static bool IsOptionDefined(Option option, IEnumerable<Argument> arguments)
    {
      bool flag = false;
      if (arguments.Get(option) != null)
        flag = true;
      return flag;
    }
  }
}
