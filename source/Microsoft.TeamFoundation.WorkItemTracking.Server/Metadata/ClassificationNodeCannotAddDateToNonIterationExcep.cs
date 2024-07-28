// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeCannotAddDateToNonIterationException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ClassificationNodeCannotAddDateToNonIterationException : 
    WorkItemTrackingServiceException
  {
    public ClassificationNodeCannotAddDateToNonIterationException(Guid nodeId)
      : base(ServerResources.ClassificationNodeCannotAddDateToNonIteration((object) nodeId))
    {
      this.NodeGuidId = new Guid?(nodeId);
    }

    public ClassificationNodeCannotAddDateToNonIterationException(int nodeId)
      : base(ServerResources.ClassificationNodeCannotAddDateToNonIteration((object) nodeId))
    {
      this.NodeIntegerId = new int?(nodeId);
    }

    public int? NodeIntegerId { get; private set; }

    public Guid? NodeGuidId { get; private set; }
  }
}
