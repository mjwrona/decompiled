// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemUnauthorizedAttachmentException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemUnauthorizedAttachmentException : WorkItemTrackingUnauthorizedAccessException
  {
    public WorkItemUnauthorizedAttachmentException(
      Guid projectGuid,
      string areaNodeUri,
      AccessType accessType)
      : base(ServerResources.WorkItemUnauthorizedProjectAttachment((object) accessType, (object) areaNodeUri, (object) projectGuid.ToString()), accessType)
    {
    }

    public WorkItemUnauthorizedAttachmentException(string areaNodeUri, AccessType accessType)
      : base(ServerResources.WorkItemUnauthorizedAttachment((object) accessType, (object) areaNodeUri), accessType)
    {
    }

    public WorkItemUnauthorizedAttachmentException(
      IEnumerable<string> areaNodeUris,
      AccessType accessType)
      : base(ServerResources.WorkItemUnauthorizedAttachment((object) accessType, (object) string.Join(", ", areaNodeUris)), accessType)
    {
    }
  }
}
