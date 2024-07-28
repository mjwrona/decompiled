// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeParentNodeDoesNotExistException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ClassificationNodeParentNodeDoesNotExistException : WorkItemTrackingServiceException
  {
    public ClassificationNodeParentNodeDoesNotExistException(int parentNodeId)
      : base(ServerResources.ClassificationNodeParentNodeDoesNotExist((object) parentNodeId))
    {
      this.ParentNodeIntegerId = new int?(parentNodeId);
    }

    public ClassificationNodeParentNodeDoesNotExistException(Guid parentNodeId)
      : base(ServerResources.ClassificationNodeParentNodeDoesNotExist((object) parentNodeId))
    {
      this.ParentNodeGuidId = new Guid?(parentNodeId);
    }

    public int? ParentNodeIntegerId { get; private set; }

    public Guid? ParentNodeGuidId { get; private set; }
  }
}
