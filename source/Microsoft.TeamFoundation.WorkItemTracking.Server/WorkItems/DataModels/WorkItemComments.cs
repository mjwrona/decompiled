// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemComments : WorkItemSecuredObject
  {
    public int TotalCount { get; private set; }

    public int FromRevisionCount { get; private set; }

    public IEnumerable<WorkItemComment> Comments { get; private set; }

    public Guid? ProjectId { get; set; }

    public WorkItemComments(
      IEnumerable<WorkItemComment> comments,
      int totalCount,
      int fromRevisionCount)
    {
      this.Comments = comments;
      this.TotalCount = totalCount;
      this.FromRevisionCount = fromRevisionCount;
    }
  }
}
