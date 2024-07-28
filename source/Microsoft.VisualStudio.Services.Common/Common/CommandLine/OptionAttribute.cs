// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.OptionAttribute
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [DebuggerDisplay("{Name,nq}")]
  public sealed class OptionAttribute : OptionAttributeBase
  {
    public OptionAttribute()
    {
      this.AllowMultiple = false;
      this.ArgumentType = OptionArgumentType.Required;
      this.CaseSensitivity = StringComparison.OrdinalIgnoreCase;
      this.OptionType = OptionType.Required;
    }

    public bool AllowMultiple { get; set; }

    public OptionArgumentType ArgumentType { get; set; }

    public StringComparison CaseSensitivity { get; set; }

    public object DefaultValue { get; set; }

    public string Description { get; set; }

    public OptionType OptionType { get; set; }

    public char ShortName { get; set; }

    public override Option ToOption(IValueConvertible valueConverter = null)
    {
      IValueConvertible valueConvertible = valueConverter;
      if (valueConverter == null)
        valueConvertible = (IValueConvertible) Activator.CreateInstance(this.ConverterType);
      return new Option()
      {
        AllowMultiple = this.AllowMultiple,
        ArgumentType = this.ArgumentType,
        CaseSensitivity = this.CaseSensitivity,
        Converter = valueConvertible,
        DefaultValue = this.DefaultValue,
        Description = this.Description,
        Name = this.Name,
        OptionType = this.OptionType,
        ShortName = this.ShortName
      };
    }
  }
}
