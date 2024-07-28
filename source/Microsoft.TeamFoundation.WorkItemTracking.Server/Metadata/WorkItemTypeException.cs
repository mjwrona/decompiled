// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [Serializable]
  public abstract class WorkItemTypeException : WorkItemTrackingServiceException
  {
    protected WorkItemTypeException(string message)
      : base(message)
    {
    }

    protected WorkItemTypeException(string message, Guid projectId, string workItemTypeReference)
      : base(message)
    {
      this.ProjectId = projectId;
      this.WorkItemTypeReference = workItemTypeReference;
    }

    public Guid ProjectId { get; private set; }

    public string WorkItemTypeReference { get; private set; }
  }
}
