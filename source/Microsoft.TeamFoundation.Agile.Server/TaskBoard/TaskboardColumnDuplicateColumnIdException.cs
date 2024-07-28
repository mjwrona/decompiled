// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumnDuplicateColumnIdException
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using System;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  [Serializable]
  public class TaskboardColumnDuplicateColumnIdException : ArgumentException
  {
    public TaskboardColumnDuplicateColumnIdException(Guid columnId)
      : base(string.Format(AgileResources.TaskboardColumnDuplicateColumnIdException, (object) columnId))
    {
    }
  }
}
