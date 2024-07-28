// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.InvalidRepositoryException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  [Serializable]
  public class InvalidRepositoryException : Exception
  {
    public InvalidRepositoryException()
    {
    }

    public InvalidRepositoryException(string message)
      : base(message)
    {
    }

    public InvalidRepositoryException(string message, params object[] arguments)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, message, arguments))
    {
    }

    public InvalidRepositoryException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvalidRepositoryException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
