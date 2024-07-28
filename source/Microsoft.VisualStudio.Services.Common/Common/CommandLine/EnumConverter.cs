// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.EnumConverter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  internal sealed class EnumConverter : ValueConverter
  {
    private Type enumValueType;

    public EnumConverter(Type enumType)
    {
      if (enumType == (Type) null)
        throw new ArgumentNullException(nameof (enumType));
      this.enumValueType = enumType.IsEnum ? enumType : throw new OptionValueConversionException(CommonResources.ErrorInvalidEnumValueTypeConversion((object) enumType.FullName));
    }

    protected override Type ResultType => this.enumValueType;

    protected override object ConvertValue(string value)
    {
      object obj = (object) null;
      foreach (string name in Enum.GetNames(this.ResultType))
      {
        if (name.Equals(value, StringComparison.OrdinalIgnoreCase))
          obj = Enum.Parse(this.ResultType, name);
      }
      return obj != null ? obj : throw new OptionValueConversionException(CommonResources.ErrorValueCannotBeConvertedToEnum((object) value, (object) this.ResultType.FullName));
    }
  }
}
