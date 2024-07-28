// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.WorkspaceVersionTableException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class WorkspaceVersionTableException : VersionControlException
  {
    public WorkspaceVersionTableException(string message)
      : this(message, (Exception) null)
    {
    }

    public WorkspaceVersionTableException(Exception innerException)
      : base((string) null, innerException)
    {
    }

    public WorkspaceVersionTableException(string message, Exception exception)
      : base(message, exception)
    {
    }

    public WorkspaceVersionTableException()
      : this((Exception) null)
    {
    }

    protected WorkspaceVersionTableException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
