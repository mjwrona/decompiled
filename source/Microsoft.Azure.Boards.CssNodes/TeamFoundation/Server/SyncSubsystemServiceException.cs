// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SyncSubsystemServiceException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class SyncSubsystemServiceException : TeamFoundationServiceException
  {
    public SyncSubsystemServiceException()
    {
    }

    public SyncSubsystemServiceException(string message)
      : base(message)
    {
    }

    public SyncSubsystemServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected SyncSubsystemServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
