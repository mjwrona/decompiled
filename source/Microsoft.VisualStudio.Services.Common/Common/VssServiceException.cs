// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssServiceException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Common
{
  [ExceptionMapping("0.0", "3.0", "VssServiceException", "Microsoft.VisualStudio.Services.Common.VssServiceException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class VssServiceException : VssException
  {
    public VssServiceException()
    {
    }

    public VssServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public VssServiceException(string message)
      : base(message)
    {
    }

    protected VssServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public virtual void GetTypeNameAndKey(
      Version restApiVersion,
      out string typeName,
      out string typeKey)
    {
      VssException.GetTypeNameAndKeyForExceptionType(this.GetType(), restApiVersion, out typeName, out typeKey);
    }
  }
}
