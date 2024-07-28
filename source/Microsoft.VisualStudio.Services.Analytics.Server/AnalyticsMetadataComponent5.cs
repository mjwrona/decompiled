// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent5
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent5 : AnalyticsMetadataComponent4
  {
    public override List<Process> GetProcesses(IList<Guid> projectIds)
    {
      this.PrepareStoredProcedure("AnalyticsModel.prc_GetProcesses");
      this.BindGuidTable("@projectIds", (IEnumerable<Guid>) projectIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Process>((ObjectBinder<Process>) new AnalyticsMetadataComponent5.ProcessColumns());
        return resultCollection.GetCurrent<Process>().Items;
      }
    }

    internal class ProcessColumns : ObjectBinder<Process>
    {
      private SqlColumnBinder m_projectSK = new SqlColumnBinder("ProjectSK");
      private SqlColumnBinder m_teamSK = new SqlColumnBinder("TeamSK");
      private SqlColumnBinder m_backlogCategoryReferenceName = new SqlColumnBinder("BacklogCategoryReferenceName");
      private SqlColumnBinder m_backlogName = new SqlColumnBinder("BacklogName");
      private SqlColumnBinder m_backlogType = new SqlColumnBinder("BacklogType");
      private SqlColumnBinder m_backlogLevel = new SqlColumnBinder("BacklogLevel");
      private SqlColumnBinder m_workItemType = new SqlColumnBinder("WorkItemType");
      private SqlColumnBinder m_hasBacklog = new SqlColumnBinder("HasBacklog");
      private SqlColumnBinder m_isHiddenType = new SqlColumnBinder("IsHiddenType");
      private SqlColumnBinder m_isBugType = new SqlColumnBinder("IsBugType");

      protected override Process Bind() => new Process()
      {
        ProjectSK = new Guid?(this.m_projectSK.GetGuid((IDataReader) this.Reader, false)),
        TeamSK = this.m_teamSK.GetGuid((IDataReader) this.Reader, false),
        BacklogCategoryReferenceName = this.m_backlogCategoryReferenceName.GetString((IDataReader) this.Reader, true),
        BacklogName = this.m_backlogName.GetString((IDataReader) this.Reader, true),
        BacklogType = this.m_backlogType.GetString((IDataReader) this.Reader, true),
        BacklogLevel = this.m_backlogLevel.GetNullableInt32((IDataReader) this.Reader),
        WorkItemType = this.m_workItemType.GetString((IDataReader) this.Reader, false),
        HasBacklog = this.m_hasBacklog.GetBoolean((IDataReader) this.Reader),
        IsHiddenType = this.m_isHiddenType.GetBoolean((IDataReader) this.Reader),
        IsBugType = this.m_isBugType.GetBoolean((IDataReader) this.Reader)
      };
    }
  }
}
