// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.OptionRequiresSpecificValue
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
  public sealed class OptionRequiresSpecificValue : OptionValidation
  {
    private Collection<IComparable> optionValues;

    public OptionRequiresSpecificValue(string dependentOption, IEnumerable<IComparable> values)
      : base(dependentOption)
    {
      this.SetOptionValues(values);
      this.DependencyFailedAction = (Action<OptionValidation>) (dependency =>
      {
        throw new OptionValidationException(CommonResources.ErrorOptionValuesDoNotMatchExpected((object) this.DependentOption, (object) string.Join<IComparable>(", ", (IEnumerable<IComparable>) this.optionValues)));
      });
    }

    public OptionRequiresSpecificValue(
      string dependentOption,
      IEnumerable<IComparable> values,
      Action<OptionValidation> dependencyFailedAction)
      : base(dependentOption, dependencyFailedAction)
    {
      this.SetOptionValues(values);
    }

    public override void Validate(IEnumerable<Option> options, IEnumerable<Argument> arguments)
    {
      if (options == null || arguments == null || string.IsNullOrWhiteSpace(this.DependentOption))
        return;
      if (!options.Contains(this.DependentOption) || options.Get(this.DependentOption).ArgumentType == OptionArgumentType.None)
        return;
      IEnumerable<Argument> all = arguments.GetAll(this.DependentOption);
      if (all == null || this.optionValues == null || !this.optionValues.Any<IComparable>())
        return;
      foreach (Argument obj in all)
      {
        bool flag = false;
        if (obj.Value != null)
        {
          foreach (IComparable optionValue in this.optionValues)
          {
            if (optionValue.CompareTo(obj.Value) == 0)
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag)
          this.DependencyFailedAction((OptionValidation) this);
      }
    }

    private void SetOptionValues(IEnumerable<IComparable> values)
    {
      if (values == null || !values.Any<IComparable>())
        return;
      this.optionValues = new Collection<IComparable>();
      foreach (IComparable comparable in values)
      {
        if (comparable != null)
          this.optionValues.Add(comparable);
      }
    }
  }
}
