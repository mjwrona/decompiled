// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingTreeNodeNotFoundException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [Serializable]
  public class WorkItemTrackingTreeNodeNotFoundException : WorkItemTrackingServiceException
  {
    public WorkItemTrackingTreeNodeNotFoundException(int nodeId)
      : base(ServerResources.InvalidNodeIdException((object) nodeId))
    {
      this.IntId = new int?(nodeId);
    }

    public WorkItemTrackingTreeNodeNotFoundException(Guid nodeId)
      : base(ServerResources.InvalidNodeGuidException((object) nodeId))
    {
      this.GuidId = new Guid?(nodeId);
    }

    public WorkItemTrackingTreeNodeNotFoundException(string name)
      : base(ServerResources.InvalidNodeNameException((object) name))
    {
      this.Name = name;
    }

    public string Name { get; private set; }

    public int? IntId { get; private set; }

    public Guid? GuidId { get; private set; }
  }
}
