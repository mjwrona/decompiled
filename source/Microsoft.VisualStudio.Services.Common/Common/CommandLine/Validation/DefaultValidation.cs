// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Validation.DefaultValidation
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine.Validation
{
  public sealed class DefaultValidation : OptionValidation
  {
    private static DefaultValidation singletonInstance = new DefaultValidation();

    private DefaultValidation()
    {
    }

    public static DefaultValidation Instance => DefaultValidation.singletonInstance;

    public override void Validate(IEnumerable<Option> options, IEnumerable<Argument> arguments)
    {
      DefaultValidation.ValidateMultiplesValidIfDefined(options, arguments);
      DefaultValidation.ValidateRequiredOptionsExist(options, arguments);
      DefaultValidation.ValidateOptionsRequiringValuesHaveValues(options, arguments);
      DefaultValidation.ValidateOptionsThatDoNotAllowValues(options, arguments);
    }

    private static void ValidateMultiplesValidIfDefined(
      IEnumerable<Option> options,
      IEnumerable<Argument> arguments)
    {
      foreach (Option option in options)
      {
        if (!option.AllowMultiple && arguments != null)
        {
          IEnumerable<Argument> all = arguments.GetAll(option);
          if (all != null && all.Count<Argument>() > 1)
            throw new OptionValidationException(CommonResources.ErrorOptionMultiplesNotAllowed((object) option.Name));
        }
      }
    }

    private static void ValidateOptionsRequiringValuesHaveValues(
      IEnumerable<Option> options,
      IEnumerable<Argument> arguments)
    {
      foreach (Option option in options)
      {
        if (option.ArgumentType == OptionArgumentType.Required && arguments != null)
        {
          IEnumerable<Argument> all = arguments.GetAll(option);
          if (all != null)
          {
            foreach (Argument obj in all)
            {
              if (obj.Value == null)
                throw new OptionValidationException(CommonResources.ErrorOptionRequiresValue((object) option.Name));
            }
          }
        }
      }
    }

    private static void ValidateOptionsThatDoNotAllowValues(
      IEnumerable<Option> options,
      IEnumerable<Argument> arguments)
    {
      foreach (Option option in options)
      {
        if (option.ArgumentType == OptionArgumentType.None && arguments != null)
        {
          IEnumerable<Argument> all = arguments.GetAll(option);
          if (all != null)
          {
            foreach (Argument obj in all)
            {
              if (obj.Value != null)
                throw new OptionValidationException(CommonResources.ErrorOptionValueNotAllowed((object) option.Name));
            }
          }
        }
      }
    }

    private static void ValidateRequiredOptionsExist(
      IEnumerable<Option> options,
      IEnumerable<Argument> arguments)
    {
      foreach (Option option in options)
      {
        if (option.OptionType == OptionType.Required && arguments.Get(option) == null)
          throw new OptionValidationException(CommonResources.ErrorOptionRequired((object) option.Name));
      }
    }
  }
}
