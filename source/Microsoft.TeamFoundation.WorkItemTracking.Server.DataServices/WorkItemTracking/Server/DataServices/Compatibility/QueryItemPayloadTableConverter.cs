// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.QueryItemPayloadTableConverter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal class QueryItemPayloadTableConverter : PayloadTableConverter
  {
    public QueryItemPayloadTableConverter(KeyValuePair<string, Type>[] columns)
    {
      if (columns == null)
        throw new ArgumentNullException(nameof (columns));
      for (int dsFieldIndex = 0; dsFieldIndex < columns.Length; ++dsFieldIndex)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.AddReadAction(dsFieldIndex, QueryItemPayloadTableConverter.\u003C\u003EO.\u003C0\u003E__PayloadTableReadAction ?? (QueryItemPayloadTableConverter.\u003C\u003EO.\u003C0\u003E__PayloadTableReadAction = new Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadTableReadAction(QueryItemPayloadTableConverter.PayloadTableReadAction)), true);
      }
    }

    private static void PayloadTableReadAction(
      IDataReader reader,
      PayloadTable table,
      int dsFieldIndex,
      PayloadTable.PayloadRow row)
    {
      int indexOfDataSetColumn = table.Columns.GetPayloadTableIndexOfDataSetColumn(dsFieldIndex);
      if (indexOfDataSetColumn == -1)
        return;
      Type dataType = table.Columns[indexOfDataSetColumn].DataType;
      if (reader.IsDBNull(dsFieldIndex))
        return;
      if (dataType.Equals(typeof (int)))
        row.SetValue(indexOfDataSetColumn, reader.GetInt32(dsFieldIndex));
      else if (dataType.Equals(typeof (short)))
        row.SetValue(indexOfDataSetColumn, reader.GetInt16(dsFieldIndex));
      else if (dataType.Equals(typeof (bool)))
        row.SetValue(indexOfDataSetColumn, reader.GetBoolean(dsFieldIndex));
      else if (dataType.Equals(typeof (ulong)))
      {
        object obj = reader.GetValue(dsFieldIndex);
        row.SetValueNoTypeChecks(indexOfDataSetColumn, (object) Convert.ToUInt64(obj));
      }
      else if (dataType.Equals(typeof (DateTime)))
      {
        DateTime dateTime = DateTime.SpecifyKind(reader.GetDateTime(dsFieldIndex), DateTimeKind.Utc);
        row.SetValueNoTypeChecks(indexOfDataSetColumn, (object) dateTime);
      }
      else if (dataType.Equals(typeof (Guid)))
        row.SetValueNoTypeChecks(indexOfDataSetColumn, (object) reader.GetGuid(dsFieldIndex));
      else
        row.SetValueNoTypeChecks(indexOfDataSetColumn, reader.GetValue(dsFieldIndex));
    }
  }
}
