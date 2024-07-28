// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseArtifactSourceTable8
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseArtifactSourceTable8
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[10]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactTypeId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceData", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceId", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersionId", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersionName", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceBranch", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("IsPrimary", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersionCreatedOn", SqlDbType.DateTime)
    };

    public static void BindReleaseArtifactSourceTable8(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseArtifactSourceTableV8", ReleaseArtifactSourceTable8.GetSqlDataRecords8(releaseArtifactSources));
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords8(
      IEnumerable<PipelineArtifactSource> rows)
    {
      rows = rows ?? Enumerable.Empty<PipelineArtifactSource>();
      foreach (PipelineArtifactSource pipelineArtifactSource in rows.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (r => r != null)))
      {
        PipelineArtifactSource row = pipelineArtifactSource;
        int ordinal = 0;
        SqlDataRecord rec = new SqlDataRecord(ReleaseArtifactSourceTable8.SqlMetaData);
        List<string> list = row.SourceData.Keys.ToList<string>();
        list.Sort();
        string str = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) list.ToDictionary<string, string, InputValue>((System.Func<string, string>) (key => key), (System.Func<string, InputValue>) (key => row.SourceData[key])));
        DateTime? versionCreatedOn = ReleaseArtifactSourceTable8.GetArtifactVersionCreatedOn(row);
        rec.SetString(ordinal, row.ArtifactTypeId);
        int num1;
        rec.SetString(num1 = ordinal + 1, str);
        int num2;
        rec.SetString(num2 = num1 + 1, string.IsNullOrEmpty(row.SourceId) ? string.Empty : row.SourceId);
        int num3;
        rec.SetString(num3 = num2 + 1, string.IsNullOrEmpty(row.Version.Value) ? string.Empty : row.Version.Value);
        int num4;
        rec.SetString(num4 = num3 + 1, string.IsNullOrEmpty(row.Version.DisplayValue) ? string.Empty : row.Version.DisplayValue);
        int num5;
        rec.SetString(num5 = num4 + 1, string.IsNullOrEmpty(row.Alias) ? string.Empty : row.Alias);
        int num6;
        rec.SetString(num6 = num5 + 1, string.IsNullOrEmpty(row.SourceBranch) ? string.Empty : row.SourceBranch);
        int num7;
        rec.SetBoolean(num7 = num6 + 1, row.IsPrimary);
        int num8;
        rec.SetInt32(num8 = num7 + 1, 0);
        int num9;
        rec.SetNullableDateTime(num9 = num8 + 1, versionCreatedOn);
        yield return rec;
      }
    }

    private static DateTime? GetArtifactVersionCreatedOn(
      PipelineArtifactSource releaseArtifactSource)
    {
      InputValue inputValue = JsonConvert.DeserializeObject<InputValue>(releaseArtifactSource.ArtifactVersion);
      DateTime? nullable = new DateTime?();
      if (inputValue != null && inputValue.Data != null && inputValue.Data.ContainsKey("artifactVersionCreatedOn"))
        nullable = new DateTime?((DateTime) inputValue.Data["artifactVersionCreatedOn"]);
      return DateTime.Compare(DateTime.MinValue, nullable.GetValueOrDefault()) != 0 ? nullable : new DateTime?();
    }
  }
}
