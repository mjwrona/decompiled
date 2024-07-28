// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase100
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase100 : TestManagementDatabase99
  {
    internal override void AddOrUpdateCoverageDetailedSummary(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCoverageDetailedSummary");
        this.PrepareStoredProcedure("prc_AddOrUpdateCoverageDetailedSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        List<BuildConfiguration> builds;
        if (buildRef == null)
        {
          builds = (List<BuildConfiguration>) null;
        }
        else
        {
          builds = new List<BuildConfiguration>();
          builds.Add(buildRef);
        }
        this.BindBuildRefTypeTable3("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        this.BindCoverageSummaryTypeTable("@coverageStatsDataTable", (IEnumerable<CodeCoverageStatistics>) coverageData.CoverageStats);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCoverageDetailedSummary");
      }
    }

    public override void AddOrUpdateCoverageDetailedSummaryWithStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus status,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCoverageDetailedSummaryWithStatus");
        this.PrepareStoredProcedure("prc_AddOrUpdateCoverageDetailedSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        List<BuildConfiguration> builds;
        if (buildRef == null)
        {
          builds = (List<BuildConfiguration>) null;
        }
        else
        {
          builds = new List<BuildConfiguration>();
          builds.Add(buildRef);
        }
        this.BindBuildRefTypeTable3("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        this.BindCoverageSummaryTypeTable("@coverageStatsDataTable", (IEnumerable<CodeCoverageStatistics>) coverageData.CoverageStats);
        this.BindInt("@coverageStatus", (int) status);
        this.BindInt("@coverageDetailedSummaryStatus", (int) coverageDetailedSummaryStatus);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.RequestContext.Trace(1015021, TraceLevel.Error, "CodeCoverageSummaryDatabase", nameof (AddOrUpdateCoverageDetailedSummaryWithStatus), "BuildID: " + buildRef.BuildId.ToString() + ", Buildflavor: " + buildRef.BuildFlavor + ", BuildPlatform: " + buildRef.BuildFlavor + " ==> " + ex.Message);
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCoverageDetailedSummaryWithStatus");
      }
    }

    public override void AddOrUpdateCoverageDetailedSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CoverageSummaryStatus status,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCoverageDetailedSummaryStatus");
        this.PrepareStoredProcedure("prc_AddOrUpdateCoverageDetailedSummaryStatus");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@buildId", buildRef.BuildId);
        this.BindString("@buildUri", buildRef.BuildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@coverageStatus", (int) status);
        this.BindInt("@coverageDetailedSummaryStatus", (int) coverageDetailedSummaryStatus);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.RequestContext.Trace(1015022, TraceLevel.Error, "CodeCoverageSummaryDatabase", nameof (AddOrUpdateCoverageDetailedSummaryStatus), "BuildID: " + buildRef.BuildId.ToString() + ", Buildflavor: " + buildRef.BuildFlavor + ", BuildPlatform: " + buildRef.BuildFlavor + " ==> " + ex.Message);
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCoverageDetailedSummaryStatus");
      }
    }

    internal override CoverageSummaryStatusResult QueryCoverageDetailedSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.QueryCoverageDetailedSummaryStatus");
        this.PrepareStoredProcedure("prc_QueryCoverageDetailedSummaryStatus");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
        this.BindInt("@buildId", buildRef.BuildId);
        this.BindString("@buildUri", buildRef.BuildUri, 256, false, SqlDbType.NVarChar);
        using (SqlDataReader sqlDataReader = this.ExecuteReader())
        {
          if (sqlDataReader.Read())
            return new CoverageSummaryStatusResult()
            {
              SummaryStatus = (CoverageSummaryStatus) sqlDataReader.GetByte(sqlDataReader.GetOrdinal("Status")),
              CoverageDetailedSummaryStatus = (CoverageDetailedSummaryStatus) sqlDataReader.GetByte(sqlDataReader.GetOrdinal("CoverageDetailedSummaryStatus")),
              RequestedDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("RequestedDate"))
            };
        }
        return base.QueryCoverageDetailedSummaryStatus(projectGuid, buildRef);
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.QueryCoverageDetailedSummaryStatus");
      }
    }

    internal override CodeCoverageSummary QueryCoverageDetailedSummary(
      Guid projectGuid,
      string buildUri,
      string deltaBuildUri)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.QueryCoverageDetailedSummary");
        CodeCoverageSummary codeCoverageSummary = new CodeCoverageSummary();
        this.PrepareStoredProcedure("prc_QueryCoverageDetailedSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
        this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
        this.BindString("@deltaBuildUri", deltaBuildUri, 256, true, SqlDbType.NVarChar);
        List<CodeCoverageData> source = new List<CodeCoverageData>();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
          {
            string buildFlavor;
            string buildPlatform;
            CodeCoverageStatistics coverageStatistics = new TestManagementDatabase.CodeCoverageSummaryColumns().Bind(reader, out buildPlatform, out buildFlavor);
            CodeCoverageData codeCoverageData = source.FirstOrDefault<CodeCoverageData>((System.Func<CodeCoverageData, bool>) (data => buildFlavor.Equals(data.BuildFlavor) && buildPlatform.Equals(data.BuildPlatform)));
            if (codeCoverageData == null)
            {
              codeCoverageData = new CodeCoverageData()
              {
                CoverageStats = (IList<CodeCoverageStatistics>) new List<CodeCoverageStatistics>(),
                BuildPlatform = buildPlatform,
                BuildFlavor = buildFlavor
              };
              source.Add(codeCoverageData);
            }
            codeCoverageData.CoverageStats.Add(coverageStatistics);
          }
          if (reader.NextResult())
          {
            if (reader.Read())
            {
              codeCoverageSummary.SummaryStatus = (CoverageSummaryStatus) reader.GetInt32(reader.GetOrdinal("SummaryStatus"));
              codeCoverageSummary.CoverageDetailedSummaryStatus = (CoverageDetailedSummaryStatus) reader.GetInt32(reader.GetOrdinal("CoverageDetailedSummaryStatus"));
            }
          }
        }
        codeCoverageSummary.CoverageData = (IList<CodeCoverageData>) source;
        codeCoverageSummary.BuildUri = buildUri;
        codeCoverageSummary.DeltaBuildUri = deltaBuildUri;
        return codeCoverageSummary;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.QueryCoverageDetailedSummary");
      }
    }

    internal TestManagementDatabase100(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase100()
    {
    }
  }
}
