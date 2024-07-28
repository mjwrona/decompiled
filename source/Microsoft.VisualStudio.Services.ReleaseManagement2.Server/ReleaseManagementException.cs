// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.ReleaseManagementException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  [Serializable]
  public class ReleaseManagementException : Exception
  {
    public ReleaseManagementException()
    {
    }

    public ReleaseManagementException(string message)
      : base(message)
    {
    }

    public ReleaseManagementException(string message, params object[] arguments)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, message, arguments))
    {
    }

    public ReleaseManagementException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ReleaseManagementException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
