// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseArtifactSourceTable7
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseArtifactSourceTable7
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[9]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactTypeId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceData", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceId", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersionId", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersionName", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceBranch", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("IsPrimary", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildId", SqlDbType.Int)
    };

    public static void BindReleaseArtifactSourceTable7(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseArtifactSourceTableV7", ReleaseArtifactSourceTable7.GetSqlDataRecords7(releaseArtifactSources));
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords7(
      IEnumerable<PipelineArtifactSource> rows)
    {
      rows = rows ?? Enumerable.Empty<PipelineArtifactSource>();
      foreach (PipelineArtifactSource pipelineArtifactSource in rows.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (r => r != null)))
      {
        PipelineArtifactSource row = pipelineArtifactSource;
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseArtifactSourceTable7.SqlMetaData);
        List<string> list = row.SourceData.Keys.ToList<string>();
        list.Sort();
        string str = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) list.ToDictionary<string, string, InputValue>((System.Func<string, string>) (key => key), (System.Func<string, InputValue>) (key => row.SourceData[key])));
        sqlDataRecord.SetString(ordinal, row.ArtifactTypeId);
        int num1;
        sqlDataRecord.SetString(num1 = ordinal + 1, str);
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, string.IsNullOrEmpty(row.SourceId) ? string.Empty : row.SourceId);
        int num3;
        sqlDataRecord.SetString(num3 = num2 + 1, string.IsNullOrEmpty(row.Version.Value) ? string.Empty : row.Version.Value);
        int num4;
        sqlDataRecord.SetString(num4 = num3 + 1, string.IsNullOrEmpty(row.Version.DisplayValue) ? string.Empty : row.Version.DisplayValue);
        int num5;
        sqlDataRecord.SetString(num5 = num4 + 1, string.IsNullOrEmpty(row.Alias) ? string.Empty : row.Alias);
        int num6;
        sqlDataRecord.SetString(num6 = num5 + 1, string.IsNullOrEmpty(row.SourceBranch) ? string.Empty : row.SourceBranch);
        int num7;
        sqlDataRecord.SetBoolean(num7 = num6 + 1, row.IsPrimary);
        int num8;
        sqlDataRecord.SetInt32(num8 = num7 + 1, 0);
        yield return sqlDataRecord;
      }
    }
  }
}
