// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexProvisionException
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [Serializable]
  public class IndexProvisionException : SearchServiceException
  {
    public IndexProvisionException()
      : this((string) null)
    {
    }

    public IndexProvisionException(string message)
      : this(message, (Exception) null)
    {
    }

    public IndexProvisionException(string message, Exception innerException)
      : this(message, innerException, (string) null)
    {
    }

    public IndexProvisionException(
      string message,
      Exception innerException,
      string exceptionSource)
      : base(message, innerException, exceptionSource)
    {
    }

    protected IndexProvisionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    protected override string DefaultMessage => ExceptionMessage.IndexProvisionException;
  }
}
