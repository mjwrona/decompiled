// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteLinkE2EData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class RemoteLinkE2EData
  {
    public int SourceId { get; set; }

    public int TargetId { get; set; }

    public int LinkType { get; set; }

    public Guid AuthorizedByTfid { get; set; }

    public Guid? RemoteHostId { get; set; }

    public Guid? RemoteProjectId { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? RemoteStatus { get; set; }

    public string RemoteStatusMessage { get; set; }

    public Guid LocalProjectId { get; set; }

    public int E2EElapsedTime { get; set; }
  }
}
