// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.OptionValidationException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [ExceptionMapping("0.0", "3.0", "OptionValidationException", "Microsoft.VisualStudio.Services.Common.CommandLine.OptionValidationException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class OptionValidationException : OptionException
  {
    public OptionValidationException()
    {
    }

    public OptionValidationException(string message)
      : base(message)
    {
    }

    public OptionValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected OptionValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
