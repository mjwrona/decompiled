// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic.DeleteBoardsForDeletedBacklogLevelsResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic
{
  public class DeleteBoardsForDeletedBacklogLevelsResult
  {
    public string LastRunOldValue { get; set; }

    public string LastRunNewValue { get; set; }

    public DeletedBehaviors ReferenceNamesFound { get; set; }

    public IDictionary<Guid, string> BoardsDeleted { get; set; }

    public string ExceptionMsg { get; set; }
  }
}
