// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ClassificationNodeUpdate
  {
    private DateTime? m_startDate;
    private DateTime? m_finishDate;

    public int Id { get; set; }

    public Guid Identifier { get; set; }

    public string Name { get; set; }

    public TreeStructureType StructureType { get; set; }

    public Guid ParentIdentifier { get; set; }

    public int ReclassifyId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? StartDate
    {
      get => this.m_startDate;
      set
      {
        this.m_startDate = value;
        this.AreDatesChanged = true;
      }
    }

    public DateTime? FinishDate
    {
      get => this.m_finishDate;
      set
      {
        this.m_finishDate = value;
        this.AreDatesChanged = true;
      }
    }

    public bool AreDatesChanged { get; private set; }
  }
}
