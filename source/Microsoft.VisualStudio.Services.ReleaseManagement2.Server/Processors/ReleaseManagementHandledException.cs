// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseManagementHandledException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  [Serializable]
  public class ReleaseManagementHandledException : VssServiceException
  {
    public ReleaseManagementHandledException()
    {
    }

    public ReleaseManagementHandledException(Exception ex)
      : base(ex != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception: {0}", (object) ex.GetType()) : string.Empty, ex)
    {
    }

    public ReleaseManagementHandledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ReleaseManagementHandledException(string message)
      : base(message)
    {
    }

    protected ReleaseManagementHandledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
