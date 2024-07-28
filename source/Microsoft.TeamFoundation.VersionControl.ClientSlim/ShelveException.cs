// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.ShelveException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class ShelveException : VersionControlException
  {
    public ShelveException(string message)
      : this(message, (Exception) null)
    {
    }

    public ShelveException(string message, Exception exception)
      : base(message, exception)
    {
    }

    public ShelveException()
      : this((Exception) null)
    {
    }

    public ShelveException(Exception innerException)
      : base((string) null, innerException)
    {
    }

    protected ShelveException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
