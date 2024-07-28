// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.OptionValueFilter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine.Validation
{
  public class OptionValueFilter : OptionValidationFilter
  {
    private Collection<IComparable> valueConstraints;

    public OptionValueFilter(
      string dependentOption,
      OptionValidation dependency,
      IEnumerable<IComparable> values)
      : base(dependentOption, dependency)
    {
      this.SetValueConstraints(values);
    }

    public override bool ShouldValidate(
      IEnumerable<Option> options,
      IEnumerable<Argument> arguments)
    {
      bool flag = false;
      if (this.Dependency != null && !string.IsNullOrEmpty(this.DependentOption))
      {
        Option option = options.Get(this.DependentOption);
        if (option != null)
        {
          IEnumerable<Argument> all = arguments.GetAll(option);
          if (all != null && this.valueConstraints != null && this.valueConstraints.Any<IComparable>())
          {
            int num = 0;
            foreach (Argument obj in all)
            {
              foreach (IComparable valueConstraint in this.valueConstraints)
              {
                if (valueConstraint.CompareTo(obj.Value) == 0)
                {
                  ++num;
                  break;
                }
              }
            }
            if (num == all.Count<Argument>())
              flag = true;
          }
        }
      }
      return flag;
    }

    private void SetValueConstraints(IEnumerable<IComparable> values)
    {
      if (values == null || !values.Any<IComparable>())
        return;
      this.valueConstraints = new Collection<IComparable>();
      foreach (IComparable comparable in values)
      {
        if (comparable != null)
          this.valueConstraints.Add(comparable);
      }
    }
  }
}
