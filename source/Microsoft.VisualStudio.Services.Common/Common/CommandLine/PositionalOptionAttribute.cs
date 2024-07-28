// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.PositionalOptionAttribute
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public class PositionalOptionAttribute : OptionAttributeBase
  {
    public PositionalOptionAttribute()
    {
    }

    public PositionalOptionAttribute(Type converterType) => this.ConverterType = converterType;

    public override Option ToOption(IValueConvertible valueConverter = null)
    {
      IValueConvertible valueConvertible = valueConverter;
      if (valueConverter == null)
        valueConvertible = (IValueConvertible) Activator.CreateInstance(this.ConverterType);
      PositionalOption option = new PositionalOption();
      option.Converter = valueConvertible;
      option.Name = this.Name;
      return (Option) option;
    }
  }
}
