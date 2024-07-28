// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkEndUnauthorizedAccessException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemLinkEndUnauthorizedAccessException : 
    WorkItemTrackingUnauthorizedAccessException
  {
    public WorkItemLinkEndUnauthorizedAccessException(
      int linkType,
      int sourceId,
      int targetId,
      WorkItemUnauthorizedAccessException sourceAccessException,
      WorkItemUnauthorizedAccessException targetAccessException)
      : base((sourceAccessException ?? targetAccessException).Message, (sourceAccessException ?? targetAccessException).AccessType, (WorkItemTrackingUnauthorizedAccessException) (sourceAccessException ?? targetAccessException))
    {
      this.LinkType = linkType;
      this.SourceId = sourceId;
      this.TargetId = targetId;
      this.SourceAccessException = sourceAccessException;
      this.TargetAccessException = targetAccessException;
    }

    public WorkItemLinkEndUnauthorizedAccessException(
      int linkType,
      int sourceId,
      int targetId,
      WorkItemUnauthorizedAccessException nodeAccessException,
      bool isSource)
      : base(nodeAccessException == null ? string.Empty : nodeAccessException.Message, nodeAccessException.AccessType, (WorkItemTrackingUnauthorizedAccessException) nodeAccessException)
    {
      this.LinkType = linkType;
      this.SourceId = sourceId;
      this.TargetId = targetId;
      if (isSource)
        this.SourceAccessException = nodeAccessException;
      else
        this.TargetAccessException = nodeAccessException;
    }

    public int LinkType { get; private set; }

    public int SourceId { get; private set; }

    public int TargetId { get; private set; }

    public WorkItemUnauthorizedAccessException SourceAccessException { get; private set; }

    public WorkItemUnauthorizedAccessException TargetAccessException { get; private set; }
  }
}
