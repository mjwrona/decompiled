// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.FolderTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class FolderTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[6]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Path", SqlDbType.NVarChar, 400L),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Description", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ChangedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ChangedOn", SqlDbType.DateTime)
    };

    public static void BindFolderTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Folder> folders)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_Folder", FolderTable.GetSqlDataRecords(folders));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<Folder> rows)
    {
      rows = rows ?? Enumerable.Empty<Folder>();
      foreach (Folder folder in rows.Where<Folder>((System.Func<Folder, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FolderTable.SqlMetaData);
        sqlDataRecord.SetString(ordinal, PathHelper.UserToDBPath(folder.Path));
        int num1;
        sqlDataRecord.SetGuid(num1 = ordinal + 1, Guid.Parse(folder.CreatedBy.Id));
        int num2;
        sqlDataRecord.SetDateTime(num2 = num1 + 1, folder.CreatedOn);
        int num3;
        sqlDataRecord.SetNullableString(num3 = num2 + 1, folder.Description);
        int num4;
        sqlDataRecord.SetNullableGuid(num4 = num3 + 1, Guid.Parse(folder.LastChangedBy.Id));
        int num5;
        sqlDataRecord.SetNullableDateTime(num5 = num4 + 1, folder.LastChangedDate);
        yield return sqlDataRecord;
      }
    }
  }
}
