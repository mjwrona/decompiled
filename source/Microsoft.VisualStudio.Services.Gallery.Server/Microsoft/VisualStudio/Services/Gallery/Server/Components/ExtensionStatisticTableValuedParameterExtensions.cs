// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionStatisticTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal static class ExtensionStatisticTableValuedParameterExtensions
  {
    private static SqlMetaData[] typ_ExtensionStatisticTable = new SqlMetaData[5]
    {
      new SqlMetaData("PublisherName", SqlDbType.VarChar, 100L),
      new SqlMetaData("ExtensionName", SqlDbType.VarChar, 100L),
      new SqlMetaData("StatisticName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Operation", SqlDbType.SmallInt),
      new SqlMetaData("Value", SqlDbType.Float)
    };

    public static SqlParameter BindExtensionStatisticTable(
      this PublishedExtensionStatisticComponent component,
      string parameterName,
      IEnumerable<ExtensionStatisticUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionStatisticUpdate>();
      System.Func<ExtensionStatisticUpdate, SqlDataRecord> selector = (System.Func<ExtensionStatisticUpdate, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(ExtensionStatisticTableValuedParameterExtensions.typ_ExtensionStatisticTable);
        record.SetString(0, row.PublisherName, BindStringBehavior.EmptyStringToNull);
        record.SetString(1, row.ExtensionName, BindStringBehavior.EmptyStringToNull);
        record.SetString(2, row.Statistic.StatisticName, BindStringBehavior.EmptyStringToNull);
        record.SetInt16(3, (short) row.Operation);
        record.SetDouble(4, row.Statistic.Value);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_ExtensionStatisticTable", rows.Select<ExtensionStatisticUpdate, SqlDataRecord>(selector));
    }
  }
}
