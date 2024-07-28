// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.WorkItemTrackingQueryUnauthorizedAccessException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  [Serializable]
  public class WorkItemTrackingQueryUnauthorizedAccessException : 
    WorkItemTrackingUnauthorizedAccessException
  {
    public WorkItemTrackingQueryUnauthorizedAccessException(Guid id, AccessType accessType)
      : base(ServerResources.QueryAccessDenied((object) id, (object) accessType), accessType)
    {
      this.ErrorCode = 600288;
      this.Id = id;
    }

    public WorkItemTrackingQueryUnauthorizedAccessException(string path, AccessType accessType)
      : base(ServerResources.QueryAccessDenied((object) path, (object) accessType), accessType)
    {
      this.ErrorCode = 600288;
      this.Path = path;
    }

    public Guid Id { get; private set; }

    public string Path { get; private set; }
  }
}
