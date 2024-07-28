// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ForeignKeyReferenceTable
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
  public static class ForeignKeyReferenceTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[2]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ChildId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ParentId", SqlDbType.UniqueIdentifier)
    };

    public static void BindForeignKeyReferenceTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ForeignKeyReference> foreignKeyReferences)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ForeignKeyReferenceTable", ForeignKeyReferenceTable.GetSqlDataRecords(foreignKeyReferences));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<ForeignKeyReference> rows)
    {
      rows = rows ?? Enumerable.Empty<ForeignKeyReference>();
      foreach (ForeignKeyReference foreignKeyReference in rows.Where<ForeignKeyReference>((System.Func<ForeignKeyReference, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ForeignKeyReferenceTable.SqlMetaData);
        sqlDataRecord.SetGuid(ordinal, foreignKeyReference.ChildId);
        int num;
        sqlDataRecord.SetGuid(num = ordinal + 1, foreignKeyReference.ParentId);
        yield return sqlDataRecord;
      }
    }
  }
}
