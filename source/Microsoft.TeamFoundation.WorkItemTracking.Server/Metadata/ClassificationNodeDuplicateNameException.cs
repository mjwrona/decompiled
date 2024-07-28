// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeDuplicateNameException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ClassificationNodeDuplicateNameException : WorkItemTrackingServiceException
  {
    public ClassificationNodeDuplicateNameException(Guid parentNodeId, string nodeName)
      : base(ServerResources.ClassificationNodeDuplicateName((object) nodeName, (object) parentNodeId))
    {
      this.ParentGuidId = new Guid?(parentNodeId);
      this.NodeName = nodeName;
    }

    public ClassificationNodeDuplicateNameException(int parentNodeId, string nodeName)
      : base(ServerResources.ClassificationNodeDuplicateName((object) nodeName, (object) parentNodeId))
    {
      this.ParentIntegerId = new int?(parentNodeId);
      this.NodeName = nodeName;
    }

    public int? ParentIntegerId { get; private set; }

    public Guid? ParentGuidId { get; private set; }

    public string NodeName { get; private set; }
  }
}
