// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CompatibilityComponent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal class CompatibilityComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[17]
    {
      (IComponentCreator) new ComponentCreator<CompatibilityComponent>(1),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent2>(2),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(3),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(4),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(5),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(6),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(7),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(8),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(9),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(10),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(11),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(12),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent3>(13),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent4>(14),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent4>(15),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent4>(16),
      (IComponentCreator) new ComponentCreator<CompatibilityComponent4>(17)
    }, "Build", "Build");
    protected static SqlMetaData[] typ_QueryBuildsByUriTable = new SqlMetaData[3]
    {
      new SqlMetaData("ItemIndex", SqlDbType.Int),
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L)
    };

    protected override string TraceArea => "Build";

    internal virtual ResultCollection QueryBuilds(
      BuildDetailSpec2010 spec,
      QueryOptions2010 options)
    {
      this.TraceEnter(0, nameof (QueryBuilds));
      this.PrepareStoredProcedure("prc_QueryBuilds2010");
      if (spec.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        this.BindTeamProject(this.RequestContext, "@teamProject", BuildCommonUtil.IsStar(spec.TeamProject) ? (string) null : spec.TeamProject, true);
        this.BindItemPath("@definitionPath", spec.GroupPath, false);
      }
      else
        this.BindTable<int>("@definitionUriTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) spec.DefinitionFilter));
      this.BindItemPath("@buildNumber", spec.BuildNumber, false);
      this.BindUtcDateTime("@minFinishTime", spec.MinFinishTime);
      this.BindUtcDateTime("@maxFinishTime", spec.MaxFinishTime);
      this.BindUtcDateTime("@minChangedTime", spec.MinChangedTime);
      this.BindNullableInt("@reasonFilter", (int) spec.Reason, (int) byte.MaxValue);
      this.BindNullableInt("@statusFilter", (int) spec.Status, 63);
      this.BindString("@qualityFilter", BuildQuality.TryConvertBuildQualityToResId(spec.Quality), 256, true, SqlDbType.NVarChar);
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryOrder", (int) spec.QueryOrder);
      this.BindIdentityFilter("@requestedFor", spec.RequestedFor, spec.RequestedForIdentity);
      this.BindInt("@maxBuildsPerDefinition", spec.MaxBuildsPerDefinition);
      this.BindInt("@queryDeletedOption", (int) spec.QueryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition2010>((ObjectBinder<BuildDefinition2010>) new BuildDefinition2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy2010>((ObjectBinder<RetentionPolicy2010>) new RetentionPolicy2010Binder());
      resultCollection.AddBinder<Schedule2010>((ObjectBinder<Schedule2010>) new Schedule2010Binder());
      resultCollection.AddBinder<ProcessTemplate2010>((ObjectBinder<ProcessTemplate2010>) new ProcessTemplate2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate2010>((ObjectBinder<WorkspaceTemplate2010>) new WorkspaceTemplate2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping2010>((ObjectBinder<WorkspaceMapping2010>) new WorkspaceMapping2010Binder());
      resultCollection.AddBinder<BuildDetail2010>((ObjectBinder<BuildDetail2010>) new BuildDetail2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController2010>((ObjectBinder<BuildController2010>) new BuildController2010Binder());
      resultCollection.AddBinder<BuildAgent2010>((ObjectBinder<BuildAgent2010>) new BuildAgent2010Binder());
      resultCollection.AddBinder<BuildServiceHost2010>((ObjectBinder<BuildServiceHost2010>) new BuildServiceHost2010Binder());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildsByUri(
      IList<string> uris,
      QueryOptions2010 options,
      QueryDeletedOption2010 queryDeletedOption)
    {
      this.TraceEnter(0, nameof (QueryBuildsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildsByUri2010");
      this.BindQueryBuildsByUriTable("@buildUriTable", (IEnumerable<string>) uris);
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryDeletedOption", (int) queryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition2010>((ObjectBinder<BuildDefinition2010>) new BuildDefinition2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy2010>((ObjectBinder<RetentionPolicy2010>) new RetentionPolicy2010Binder());
      resultCollection.AddBinder<Schedule2010>((ObjectBinder<Schedule2010>) new Schedule2010Binder());
      resultCollection.AddBinder<ProcessTemplate2010>((ObjectBinder<ProcessTemplate2010>) new ProcessTemplate2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate2010>((ObjectBinder<WorkspaceTemplate2010>) new WorkspaceTemplate2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping2010>((ObjectBinder<WorkspaceMapping2010>) new WorkspaceMapping2010Binder());
      resultCollection.AddBinder<BuildDetail2010>((ObjectBinder<BuildDetail2010>) new BuildDetail2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController2010>((ObjectBinder<BuildController2010>) new BuildController2010Binder());
      resultCollection.AddBinder<BuildAgent2010>((ObjectBinder<BuildAgent2010>) new BuildAgent2010Binder());
      resultCollection.AddBinder<BuildServiceHost2010>((ObjectBinder<BuildServiceHost2010>) new BuildServiceHost2010Binder());
      this.TraceLeave(0, nameof (QueryBuildsByUri));
      return resultCollection;
    }

    internal IDictionary<string, string> ResolveBuildUris(
      ICollection<string> buildUris,
      bool useCompatibilityFormat)
    {
      this.TraceEnter(0, nameof (ResolveBuildUris));
      this.PrepareStoredProcedure("prc_ResolveBuildUris");
      this.BindTable<string>("@buildUriTable", (TeamFoundationTableValueParameter<string>) new ResolveBuildUrisTable(buildUris));
      this.BindString("@requestedBy", this.RequestContext.DomainUserName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      while (sqlDataReader.Read())
      {
        string str1 = sqlDataReader.GetString(0);
        string key;
        if (!sqlDataReader.IsDBNull(1))
        {
          key = RosarioHelper.ConvertBuildUri(str1, sqlDataReader.GetInt32(1), !sqlDataReader.IsDBNull(2) && sqlDataReader.GetBoolean(2));
          this.Trace(0, TraceLevel.Info, "Uri is converted to '{0}'", (object) key);
        }
        else
        {
          key = DBHelper.CreateArtifactUri("Build", str1);
          this.Trace(0, TraceLevel.Info, "Uri is recreated '{0}'", (object) key);
        }
        if (sqlDataReader.IsDBNull(3))
        {
          this.Trace(0, TraceLevel.Info, "Skipped processing uri '{0}'", (object) key);
        }
        else
        {
          string str2 = sqlDataReader.GetString(3);
          if (!string.IsNullOrEmpty(str2))
          {
            this.Trace(0, TraceLevel.Verbose, "Processing uri '{0}' with new uri '{1}'", (object) key, (object) str2);
            if (useCompatibilityFormat)
            {
              dictionary[key] = RosarioHelper.ConvertBuildUri(str2, sqlDataReader.GetInt32(4), false);
              this.Trace(0, TraceLevel.Info, "New build uri is '{0}' when using compatibility format", (object) dictionary[key]);
            }
            else
            {
              dictionary[key] = DBHelper.CreateArtifactUri("Build", str2);
              this.Trace(0, TraceLevel.Info, "New build uri is '{0}'", (object) dictionary[key]);
            }
          }
        }
      }
      this.TraceLeave(0, nameof (ResolveBuildUris));
      return (IDictionary<string, string>) dictionary;
    }

    protected virtual SqlParameter BindQueryBuildsByUriTable(
      string parameterName,
      IEnumerable<string> rows)
    {
      return this.BindTable(parameterName, "typ_QueryBuildsByUriTable", (rows ?? Enumerable.Empty<string>()).Select<string, SqlDataRecord>(new System.Func<string, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(string row)
    {
      int num1 = 0;
      SqlDataRecord sqlDataRecord = new SqlDataRecord(CompatibilityComponent.typ_QueryBuildsByUriTable);
      string dbId = DBHelper.ExtractDbId(row);
      int length = dbId.IndexOf("?queueId=", StringComparison.OrdinalIgnoreCase);
      int num2;
      sqlDataRecord.SetInt32(0, num2 = num1 + 1);
      if (length > 0)
      {
        sqlDataRecord.SetInt32(1, int.Parse(dbId.Substring(length + 9), (IFormatProvider) CultureInfo.InvariantCulture));
        sqlDataRecord.SetString(2, dbId.Substring(0, length));
      }
      else
      {
        sqlDataRecord.SetInt32(1, -1);
        sqlDataRecord.SetString(2, dbId);
      }
      return sqlDataRecord;
    }
  }
}
