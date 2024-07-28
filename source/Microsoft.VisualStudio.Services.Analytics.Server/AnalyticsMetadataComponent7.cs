// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent7
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
  internal class AnalyticsMetadataComponent7 : AnalyticsMetadataComponent6
  {
    public override List<Area> GetAreas(ICollection<Guid> areaIds)
    {
      if (areaIds.Count == 0)
        return new List<Area>();
      this.PrepareStoredProcedure("AnalyticsModel.prc_GetAreas");
      this.BindGuidTable("@areaIds", (IEnumerable<Guid>) areaIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Area>((ObjectBinder<Area>) new AnalyticsMetadataComponent7.AreaColumns());
        return resultCollection.GetCurrent<Area>().Items;
      }
    }

    internal class AreaColumns : ObjectBinder<Area>
    {
      private SqlColumnBinder m_projectSK = new SqlColumnBinder("ProjectSK");
      private SqlColumnBinder m_areaSK = new SqlColumnBinder("AreaSK");
      private SqlColumnBinder m_areaId = new SqlColumnBinder("AreaId");
      private SqlColumnBinder m_areaName = new SqlColumnBinder("AreaName");
      private SqlColumnBinder m_number = new SqlColumnBinder("Number");
      private SqlColumnBinder m_areaPath = new SqlColumnBinder("AreaPath");
      private SqlColumnBinder m_depth = new SqlColumnBinder("Depth");

      protected override Area Bind() => new Area()
      {
        ProjectSK = new Guid?(this.m_projectSK.GetGuid((IDataReader) this.Reader, false)),
        AreaSK = this.m_areaSK.GetGuid((IDataReader) this.Reader, false),
        AreaId = this.m_areaId.GetGuid((IDataReader) this.Reader, false),
        AreaName = this.m_areaName.GetString((IDataReader) this.Reader, false),
        Number = this.m_number.GetNullableInt32((IDataReader) this.Reader),
        AreaPath = this.m_areaPath.GetString((IDataReader) this.Reader, false),
        Depth = this.m_depth.GetNullableInt32((IDataReader) this.Reader)
      };
    }
  }
}
