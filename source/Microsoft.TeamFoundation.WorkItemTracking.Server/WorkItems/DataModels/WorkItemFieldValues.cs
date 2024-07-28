// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemFieldValues
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemFieldValues
  {
    private int? m_manualLatestAreaId;
    private bool? m_manualLatestIsDeleted;

    public WorkItemFieldValues()
    {
      this.Fields = new Dictionary<int, object>();
      this.IdentityFields = new Dictionary<int, Guid>();
    }

    public Guid ProjectId { get; set; }

    public int Id => this.Fields.GetValueOrDefault<int>(-3);

    public int Rev => this.Fields.GetValueOrDefault<int>(8);

    public int AreaId => this.Fields.GetValueOrDefault<int>(-2);

    public DateTime AuthorizedDate
    {
      get => this.Fields.GetValueOrDefault<DateTime>(3, DateTime.MinValue);
      protected set => this.Fields[3] = (object) value;
    }

    public DateTime RevisedDate => this.Fields.GetValueOrDefault<DateTime>(-5, SharedVariables.FutureDateTimeValue);

    public int LatestAreaId
    {
      get => !this.m_manualLatestAreaId.HasValue ? this.AreaId : this.m_manualLatestAreaId.Value;
      set => this.m_manualLatestAreaId = new int?(value);
    }

    public bool LatestIsDeleted
    {
      get => !this.m_manualLatestIsDeleted.HasValue ? this.Fields.GetValueOrDefault<bool>(-404) : this.m_manualLatestIsDeleted.Value;
      set => this.m_manualLatestIsDeleted = new bool?(value);
    }

    public Dictionary<int, object> Fields { get; private set; }

    public Dictionary<int, Guid> IdentityFields { get; private set; }

    public IEnumerable<TagDefinition> TagDefinitions { get; internal set; }

    public WorkItemCommentVersionRecord WorkItemCommentVersion { get; internal set; }

    private int GetStringSize(object o)
    {
      if (o == null)
        return 0;
      try
      {
        return o.ToString().Length * 2;
      }
      catch
      {
        return o is DateTime ? 38 : 0;
      }
    }
  }
}
