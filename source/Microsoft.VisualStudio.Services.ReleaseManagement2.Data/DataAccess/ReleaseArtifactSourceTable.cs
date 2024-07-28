// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseArtifactSourceTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseArtifactSourceTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[2]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactSourceId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersion", SqlDbType.NVarChar, -1L)
    };

    public static void BindReleaseArtifactSourceTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseArtifactSourceTable", ReleaseArtifactSourceTable.GetSqlDataRecords(releaseArtifactSources));
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<PipelineArtifactSource> rows)
    {
      rows = rows ?? Enumerable.Empty<PipelineArtifactSource>();
      foreach (PipelineArtifactSource pipelineArtifactSource in rows.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseArtifactSourceTable.SqlMetaData);
        sqlDataRecord.SetInt32(ordinal, pipelineArtifactSource.ArtifactSourceId);
        int num;
        sqlDataRecord.SetString(num = ordinal + 1, pipelineArtifactSource.ArtifactVersion);
        yield return sqlDataRecord;
      }
    }
  }
}
