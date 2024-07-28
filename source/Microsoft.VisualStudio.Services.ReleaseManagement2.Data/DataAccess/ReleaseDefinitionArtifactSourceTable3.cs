// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseDefinitionArtifactSourceTable3
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
  public static class ReleaseDefinitionArtifactSourceTable3
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[5]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("SourceId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactTypeId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceData", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("IsPrimary", SqlDbType.Bit)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData4 = new Microsoft.SqlServer.Server.SqlMetaData[6]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("SourceId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactTypeId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceData", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("IsPrimary", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier)
    };

    public static void BindReleaseDefinitionArtifactSourceTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ArtifactSource> artifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseDefinitionArtifactSourceTableV3", ReleaseDefinitionArtifactSourceTable3.GetSqlDataRecords(artifactSources));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<ArtifactSource> rows)
    {
      rows = rows ?? Enumerable.Empty<ArtifactSource>();
      foreach (ArtifactSource artifactSource in rows.Where<ArtifactSource>((System.Func<ArtifactSource, bool>) (r => r != null)))
      {
        ArtifactSource row = artifactSource;
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseDefinitionArtifactSourceTable3.SqlMetaData);
        sqlDataRecord.SetString(ordinal, string.IsNullOrEmpty(row.SourceId) ? string.Empty : row.SourceId);
        int num1;
        sqlDataRecord.SetString(num1 = ordinal + 1, string.IsNullOrEmpty(row.Alias) ? string.Empty : row.Alias);
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, string.IsNullOrEmpty(row.ArtifactTypeId) ? string.Empty : row.ArtifactTypeId);
        List<string> list = row.SourceData.Keys.ToList<string>();
        list.Sort();
        string str = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) list.ToDictionary<string, string, InputValue>((System.Func<string, string>) (key => key), (System.Func<string, InputValue>) (key => row.SourceData[key])));
        int num3;
        sqlDataRecord.SetString(num3 = num2 + 1, string.IsNullOrEmpty(str) ? string.Empty : str);
        int num4;
        sqlDataRecord.SetBoolean(num4 = num3 + 1, row.IsPrimary);
        yield return sqlDataRecord;
      }
    }

    public static void BindReleaseDefinitionArtifactSourceTable4(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ArtifactSource> artifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseDefinitionArtifactSourceTableV4", ReleaseDefinitionArtifactSourceTable3.GetSqlDataRecords4(artifactSources));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords4(IEnumerable<ArtifactSource> rows)
    {
      rows = rows ?? Enumerable.Empty<ArtifactSource>();
      foreach (ArtifactSource artifactSource in rows.Where<ArtifactSource>((System.Func<ArtifactSource, bool>) (r => r != null)))
      {
        ArtifactSource row = artifactSource;
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseDefinitionArtifactSourceTable3.SqlMetaData4);
        record.SetString(ordinal, string.IsNullOrEmpty(row.SourceId) ? string.Empty : row.SourceId);
        int num1;
        record.SetString(num1 = ordinal + 1, string.IsNullOrEmpty(row.Alias) ? string.Empty : row.Alias);
        int num2;
        record.SetString(num2 = num1 + 1, string.IsNullOrEmpty(row.ArtifactTypeId) ? string.Empty : row.ArtifactTypeId);
        List<string> list = row.SourceData.Keys.ToList<string>();
        list.Sort();
        string str = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) list.ToDictionary<string, string, InputValue>((System.Func<string, string>) (key => key), (System.Func<string, InputValue>) (key => row.SourceData[key])));
        int num3;
        record.SetString(num3 = num2 + 1, string.IsNullOrEmpty(str) ? string.Empty : str);
        int num4;
        record.SetBoolean(num4 = num3 + 1, row.IsPrimary);
        int num5;
        record.SetNullableGuid(num5 = num4 + 1, new Guid?());
        yield return record;
      }
    }
  }
}
