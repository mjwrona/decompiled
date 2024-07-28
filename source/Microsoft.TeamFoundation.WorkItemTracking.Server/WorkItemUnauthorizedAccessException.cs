// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemUnauthorizedAccessException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemUnauthorizedAccessException : WorkItemTrackingUnauthorizedAccessException
  {
    public WorkItemUnauthorizedAccessException(int id, AccessType accessType)
      : base(WorkItemUnauthorizedAccessException.FormatMessage(id, accessType), accessType)
    {
      this.Id = id;
    }

    public int Id { get; private set; }

    private static string FormatMessage(int workItemId, AccessType accessType)
    {
      switch (accessType)
      {
        case AccessType.Read:
          return ServerResources.WorkItemNotFoundOrAccessDenied((object) workItemId);
        case AccessType.SoftDelete:
          return ServerResources.WorkItemUnauthorizedDelete((object) workItemId);
        case AccessType.Move:
          return ServerResources.WorkItemUnauthorizedMove((object) workItemId);
        case AccessType.Restore:
          return ServerResources.WorkItemUnauthorizedRestore((object) workItemId);
        default:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, InternalsResourceStrings.Get("ErrorNoAreaWriteAccess"), (object) workItemId);
      }
    }
  }
}
