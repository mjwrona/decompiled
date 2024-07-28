// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent8
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
  internal class AnalyticsMetadataComponent8 : AnalyticsMetadataComponent7
  {
    public override List<Iteration> GetIterations(ICollection<Guid> iterationIds)
    {
      if (iterationIds.Count == 0)
        return new List<Iteration>();
      this.PrepareStoredProcedure("AnalyticsModel.prc_GetIterations");
      this.BindGuidTable("@iterationIds", (IEnumerable<Guid>) iterationIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Iteration>((ObjectBinder<Iteration>) new AnalyticsMetadataComponent8.IterationColumns());
        return resultCollection.GetCurrent<Iteration>().Items;
      }
    }

    internal class IterationColumns : ObjectBinder<Iteration>
    {
      private SqlColumnBinder m_projectSK = new SqlColumnBinder("ProjectSK");
      private SqlColumnBinder m_iterationId = new SqlColumnBinder("IterationId");
      private SqlColumnBinder m_iterationName = new SqlColumnBinder("IterationName");
      private SqlColumnBinder m_number = new SqlColumnBinder("Number");
      private SqlColumnBinder m_iterationPath = new SqlColumnBinder("IterationPath");
      private SqlColumnBinder m_depth = new SqlColumnBinder("Depth");

      protected override Iteration Bind() => new Iteration()
      {
        ProjectSK = new Guid?(this.m_projectSK.GetGuid((IDataReader) this.Reader, false)),
        IterationId = this.m_iterationId.GetGuid((IDataReader) this.Reader, false),
        IterationName = this.m_iterationName.GetString((IDataReader) this.Reader, false),
        Number = this.m_number.GetNullableInt32((IDataReader) this.Reader),
        IterationPath = this.m_iterationPath.GetString((IDataReader) this.Reader, false),
        Depth = this.m_depth.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
