// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingDictionaryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class WorkItemTrackingDictionaryService : 
    CacheServiceBase,
    ITeamFoundationReplicaAwareService
  {
    void ITeamFoundationReplicaAwareService.ServiceEnd(IVssRequestContext requestContext) => this.ServiceEnd(requestContext);

    void ITeamFoundationReplicaAwareService.ServiceStart(
      IVssRequestContext requestContext,
      bool isMaster)
    {
      if (!isMaster)
        return;
      this.ServiceStart(requestContext);
    }

    string ITeamFoundationReplicaAwareService.DatabaseCategory => "WorkItem";
  }
}
