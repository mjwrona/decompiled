// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions.ReleaseNotActiveException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions
{
  [Serializable]
  public class ReleaseNotActiveException : VssServiceException
  {
    public ReleaseNotActiveException()
    {
    }

    public ReleaseNotActiveException(string message)
      : base(message)
    {
    }

    public ReleaseNotActiveException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ReleaseNotActiveException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
