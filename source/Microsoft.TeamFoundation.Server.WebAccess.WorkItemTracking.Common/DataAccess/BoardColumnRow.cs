// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardColumnRow
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardColumnRow
  {
    public Guid? Id { get; set; }

    public Guid? BoardId { get; set; }

    public string Name { get; set; }

    public int Order { get; set; }

    public int ItemLimit { get; set; }

    public int ColumnType { get; set; }

    public DateTime? RevisedDate { get; set; }

    public bool Deleted { get; set; }

    public bool IsSplit { get; set; }

    public string Description { get; set; }

    public int Watermark { get; set; }
  }
}
