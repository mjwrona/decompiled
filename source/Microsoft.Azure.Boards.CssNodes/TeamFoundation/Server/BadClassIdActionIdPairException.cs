// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.BadClassIdActionIdPairException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using System;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class BadClassIdActionIdPairException : AuthorizationSubsystemServiceException
  {
    public BadClassIdActionIdPairException(string classId, string actionId)
      : base(ServerResources.GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION((object) classId, (object) actionId))
    {
    }
  }
}
