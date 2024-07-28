// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkCircularException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemLinkCircularException : WorkItemLinkInvalidException
  {
    public WorkItemLinkCircularException(
      int workItemId,
      int linkTypeId,
      int sourceId,
      int targetId,
      string linkName = null)
      : base(WorkItemLinkCircularException.ErrorMessage(linkTypeId, sourceId, targetId, linkName), workItemId, linkTypeId, linkName, sourceId, targetId)
    {
      this.ErrorCode = 600270;
    }

    private static string ErrorMessage(
      int linkTypeId,
      int sourceId,
      int targetId,
      string linkName)
    {
      linkName = string.IsNullOrEmpty(linkName) ? linkTypeId.ToString() : linkName;
      return targetId >= 1 ? string.Format(InternalsResourceStrings.Get("AddLinkCircularity"), (object) linkName, (object) sourceId, (object) targetId) : string.Format(InternalsResourceStrings.Get("AddLinkCircularity2"), (object) linkName, (object) sourceId);
    }
  }
}
