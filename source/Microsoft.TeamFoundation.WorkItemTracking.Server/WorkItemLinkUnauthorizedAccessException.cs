// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkUnauthorizedAccessException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemLinkUnauthorizedAccessException : WorkItemTrackingUnauthorizedAccessException
  {
    public WorkItemLinkUnauthorizedAccessException(
      int linkType,
      AccessType accessType,
      int sourceId,
      int targetId)
      : base(InternalsResourceStrings.Get("ErrorNotANamespaceAdmin"), accessType)
    {
      this.LinkType = linkType;
      this.SourceId = sourceId;
      this.TargetId = targetId;
    }

    public int LinkType { get; private set; }

    public int SourceId { get; private set; }

    public int TargetId { get; private set; }
  }
}
