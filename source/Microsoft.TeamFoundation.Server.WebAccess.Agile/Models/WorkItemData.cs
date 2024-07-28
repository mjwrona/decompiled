// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal sealed class WorkItemData
  {
    private IDataRecord dataRecord;
    private int parentID;
    private List<WorkItemData> m_children;
    private const int InvalidWorkItemID = -1;
    private string WorkItemTypeFieldID = "System.WorkItemType";

    public WorkItemData(int id, IDataRecord dataRecord, HashSet<string> parentTypes)
    {
      this.ID = id;
      this.dataRecord = dataRecord;
      this.parentID = -1;
      this.realParentId = -1;
      this.m_children = new List<WorkItemData>();
      string str = (string) this.dataRecord[this.WorkItemTypeFieldID];
      this.IsParentType = parentTypes.Contains(str);
    }

    public int ID { get; internal set; }

    public int ParentID
    {
      get => this.parentID;
      set => this.parentID = value;
    }

    public int realParentId { get; set; }

    public bool IsParentType { get; internal set; }

    public IList<WorkItemData> Children => (IList<WorkItemData>) this.m_children.AsReadOnly();

    public void AddChild(WorkItemData workItem)
    {
      ArgumentUtility.CheckForNull<WorkItemData>(workItem, nameof (workItem));
      this.m_children.Add(workItem);
    }

    public object GetFieldValue(string fieldID) => StringComparer.OrdinalIgnoreCase.Equals(fieldID, "ParentId") ? (object) this.ParentID : this.dataRecord[fieldID];

    public bool TryGetFieldValue(string fieldName, out object value)
    {
      value = (object) null;
      bool fieldValue;
      try
      {
        value = this.GetFieldValue(fieldName);
        fieldValue = true;
      }
      catch (IndexOutOfRangeException ex)
      {
        fieldValue = false;
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (WorkItemData), (Exception) ex);
      }
      return fieldValue;
    }
  }
}
