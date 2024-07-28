// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.OptionValueConversionException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [ExceptionMapping("0.0", "3.0", "OptionValueConversionException", "Microsoft.VisualStudio.Services.Common.CommandLine.OptionValueConversionException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class OptionValueConversionException : OptionValidationException
  {
    public OptionValueConversionException()
    {
    }

    public OptionValueConversionException(string message)
      : base(message)
    {
    }

    public OptionValueConversionException(string value, Type valueType)
      : base(OptionValueConversionException.CreateMessage(value, valueType))
    {
    }

    public OptionValueConversionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public OptionValueConversionException(string value, Type valueType, Exception innerException)
      : base(OptionValueConversionException.CreateMessage(value, valueType), innerException)
    {
    }

    protected OptionValueConversionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    private static string CreateMessage(string value, Type valueType) => CommonResources.ErrorInvalidValueTypeConversion((object) value, (object) valueType.FullName);
  }
}
