// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SyncAccessDeniedServiceException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class SyncAccessDeniedServiceException : SyncSubsystemServiceException
  {
    public SyncAccessDeniedServiceException()
      : this((Exception) null)
    {
    }

    public SyncAccessDeniedServiceException(Exception innerException)
      : base(ServerResources.SYNC_ITEM_DOES_NOT_EXIST_OR_ACCESS_DENIED(), innerException)
    {
    }

    protected SyncAccessDeniedServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
