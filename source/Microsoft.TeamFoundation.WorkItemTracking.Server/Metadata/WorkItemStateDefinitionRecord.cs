// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemStateDefinitionRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemStateDefinitionRecord
  {
    public Guid ProcessId { get; set; }

    public Guid WorkItemTypeId { get; set; }

    public Guid StateId { get; set; }

    public string Name { get; set; }

    public int StateOrder { get; set; }

    public string Color { get; set; }

    public int StateCategory { get; set; }

    public bool Hidden { get; set; }

    public DateTime AuthorizedDate { get; set; }
  }
}
