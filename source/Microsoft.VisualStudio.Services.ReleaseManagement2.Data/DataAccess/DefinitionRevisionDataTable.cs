// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DefinitionRevisionDataTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class DefinitionRevisionDataTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[6]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionRevision", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ChangedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ChangedDate", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Comment", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("FileId", SqlDbType.Int)
    };

    public static void BindDefinitionRevisionDataTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ServerReleaseDefinitionRevision> definitionRevisions)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionRevision", DefinitionRevisionDataTable.GetSqlDataRecords(definitionRevisions));
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Reviewed")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<ServerReleaseDefinitionRevision> rows)
    {
      rows = rows ?? Enumerable.Empty<ServerReleaseDefinitionRevision>();
      foreach (ServerReleaseDefinitionRevision definitionRevision in rows.Where<ServerReleaseDefinitionRevision>((System.Func<ServerReleaseDefinitionRevision, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionRevisionDataTable.SqlMetaData);
        record.SetInt32(ordinal, definitionRevision.BuildDefinitionId);
        int num1;
        record.SetInt32(num1 = ordinal + 1, definitionRevision.Revision);
        int num2;
        record.SetGuid(num2 = num1 + 1, Guid.Parse(definitionRevision.ChangedBy.Id));
        int num3;
        record.SetDateTime(num3 = num2 + 1, definitionRevision.ChangedDate);
        int num4;
        record.SetNullableString(num4 = num3 + 1, definitionRevision.Comment);
        int num5;
        record.SetInt32(num5 = num4 + 1, definitionRevision.FileId);
        yield return record;
      }
    }
  }
}
