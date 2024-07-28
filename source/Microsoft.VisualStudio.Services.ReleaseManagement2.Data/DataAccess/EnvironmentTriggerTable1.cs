// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.EnvironmentTriggerTable1
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class EnvironmentTriggerTable1
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[3]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("EnvironmentName", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerContent", SqlDbType.NVarChar, -1L)
    };

    public static void BindEnvironmentTriggerTable1(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IList<EnvironmentTrigger> environmentTriggers,
      IDictionary<int, string> environmentNamesByIds)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_EnvironmentTriggerTableV1", EnvironmentTriggerTable1.GetEnvironmentTriggerSqlDataRecords((IEnumerable<EnvironmentTrigger>) environmentTriggers, environmentNamesByIds));
    }

    private static IEnumerable<SqlDataRecord> GetEnvironmentTriggerSqlDataRecords(
      IEnumerable<EnvironmentTrigger> rows,
      IDictionary<int, string> environmentNamesByIds)
    {
      rows = rows ?? Enumerable.Empty<EnvironmentTrigger>();
      foreach (EnvironmentTrigger environmentTrigger in rows.Where<EnvironmentTrigger>((System.Func<EnvironmentTrigger, bool>) (r => r != null)))
      {
        string empty = string.Empty;
        if (environmentNamesByIds.TryGetValue(environmentTrigger.EnvironmentId, out empty))
        {
          int ordinal = 0;
          SqlDataRecord triggerSqlDataRecord = new SqlDataRecord(EnvironmentTriggerTable1.SqlMetaData);
          triggerSqlDataRecord.SetString(ordinal, empty);
          int num1;
          triggerSqlDataRecord.SetByte(num1 = ordinal + 1, environmentTrigger.TriggerType);
          int num2;
          if (environmentTrigger.TriggerContent != null)
            triggerSqlDataRecord.SetString(num2 = num1 + 1, environmentTrigger.TriggerContent);
          else
            triggerSqlDataRecord.SetDBNull(num2 = num1 + 1);
          yield return triggerSqlDataRecord;
        }
      }
    }
  }
}
