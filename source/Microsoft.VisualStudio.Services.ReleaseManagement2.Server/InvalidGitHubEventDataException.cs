// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.InvalidGitHubEventDataException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  [Serializable]
  public class InvalidGitHubEventDataException : Exception
  {
    public InvalidGitHubEventDataException()
    {
    }

    public InvalidGitHubEventDataException(string message)
      : base(message)
    {
    }

    public InvalidGitHubEventDataException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected InvalidGitHubEventDataException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
