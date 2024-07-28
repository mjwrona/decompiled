// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionEventsTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal static class ExtensionEventsTableValuedParameterExtensions
  {
    private static SqlMetaData[] typ_ExtensionEventsTable = new SqlMetaData[5]
    {
      new SqlMetaData("ExtensionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("StatisticDate", SqlDbType.DateTime),
      new SqlMetaData("EventType", SqlDbType.Int),
      new SqlMetaData("Properties", SqlDbType.NVarChar, 4000L)
    };
    private static SqlMetaData[] typ_ExtensionEventsTable2 = new SqlMetaData[5]
    {
      new SqlMetaData("ExtensionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("StatisticDate", SqlDbType.DateTime),
      new SqlMetaData("EventType", SqlDbType.Int),
      new SqlMetaData("Properties", SqlDbType.NVarChar, -1L)
    };
    private static SqlMetaData[] typ_ExtensionEventsTable3 = new SqlMetaData[6]
    {
      new SqlMetaData("Id", SqlDbType.BigInt),
      new SqlMetaData("ExtensionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("StatisticDate", SqlDbType.DateTime),
      new SqlMetaData("EventType", SqlDbType.Int),
      new SqlMetaData("Properties", SqlDbType.NVarChar, -1L)
    };

    public static SqlParameter BindExtensionEventsTable(
      this ExtensionDailyStatsComponent component,
      string parameterName,
      IEnumerable<ExtensionEvents> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionEvents>();
      System.Func<ExtensionEvents, IEnumerable<SqlDataRecord>> selector = (System.Func<ExtensionEvents, IEnumerable<SqlDataRecord>>) (row =>
      {
        List<SqlDataRecord> sqlDataRecordList = new List<SqlDataRecord>();
        if (row != null)
        {
          foreach (string key in (IEnumerable<string>) row.Events.Keys)
          {
            IEnumerable<ExtensionEvent> extensionEvents = row.Events[key];
            if (extensionEvents != null)
            {
              foreach (ExtensionEvent extensionEvent in extensionEvents)
              {
                SqlDataRecord record = new SqlDataRecord(ExtensionEventsTableValuedParameterExtensions.typ_ExtensionEventsTable);
                record.SetGuid(0, row.ExtensionId);
                record.SetString(1, extensionEvent.Version);
                record.SetDateTime(2, extensionEvent.StatisticDate);
                if (extensionEvent.Properties == null)
                  extensionEvent.Properties = new JObject();
                ExtensionLifecycleEventType lifecycleEventType;
                if (!ExtensionLifecycleWellKnownEvents.KnownEventsDictionary.TryGetValue(key, out lifecycleEventType))
                {
                  lifecycleEventType = ExtensionLifecycleEventType.Other;
                  if (!extensionEvent.Properties.TryGetValue("eventType", StringComparison.OrdinalIgnoreCase, out JToken _))
                    extensionEvent.Properties.Add("eventType", (JToken) key);
                }
                record.SetInt32(3, (int) lifecycleEventType);
                string str = extensionEvent.Properties.Serialize<JObject>();
                record.SetString(4, str, BindStringBehavior.EmptyStringToNull);
                sqlDataRecordList.Add(record);
              }
            }
          }
        }
        return (IEnumerable<SqlDataRecord>) sqlDataRecordList;
      });
      IEnumerable<SqlDataRecord> rows1 = rows.SelectMany<ExtensionEvents, SqlDataRecord>(selector);
      return component.BindTable(parameterName, "Gallery.typ_ExtensionEventsTable", rows1);
    }

    public static SqlParameter BindExtensionEventsTable2(
      this ExtensionDailyStatsComponent component,
      string parameterName,
      IEnumerable<ExtensionEvents> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionEvents>();
      System.Func<ExtensionEvents, IEnumerable<SqlDataRecord>> selector = (System.Func<ExtensionEvents, IEnumerable<SqlDataRecord>>) (row =>
      {
        List<SqlDataRecord> sqlDataRecordList = new List<SqlDataRecord>();
        if (row != null)
        {
          foreach (string key in (IEnumerable<string>) row.Events.Keys)
          {
            IEnumerable<ExtensionEvent> extensionEvents = row.Events[key];
            if (extensionEvents != null)
            {
              foreach (ExtensionEvent extensionEvent in extensionEvents)
              {
                SqlDataRecord record = new SqlDataRecord(ExtensionEventsTableValuedParameterExtensions.typ_ExtensionEventsTable2);
                record.SetGuid(0, row.ExtensionId);
                record.SetString(1, extensionEvent.Version);
                record.SetDateTime(2, extensionEvent.StatisticDate);
                if (extensionEvent.Properties == null)
                  extensionEvent.Properties = new JObject();
                ExtensionLifecycleEventType lifecycleEventType;
                if (!ExtensionLifecycleWellKnownEvents.KnownEventsDictionary.TryGetValue(key, out lifecycleEventType))
                {
                  lifecycleEventType = ExtensionLifecycleEventType.Other;
                  if (!extensionEvent.Properties.TryGetValue("eventType", StringComparison.OrdinalIgnoreCase, out JToken _))
                    extensionEvent.Properties.Add("eventType", (JToken) key);
                }
                record.SetInt32(3, (int) lifecycleEventType);
                string str = extensionEvent.Properties.Serialize<JObject>();
                record.SetString(4, str, BindStringBehavior.EmptyStringToNull);
                sqlDataRecordList.Add(record);
              }
            }
          }
        }
        return (IEnumerable<SqlDataRecord>) sqlDataRecordList;
      });
      IEnumerable<SqlDataRecord> rows1 = rows.SelectMany<ExtensionEvents, SqlDataRecord>(selector);
      return component.BindTable(parameterName, "Gallery.typ_ExtensionEventsTable2", rows1);
    }

    public static SqlParameter BindExtensionEventsTable3(
      this ExtensionDailyStatsComponent component,
      string parameterName,
      IEnumerable<ExtensionEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionEvent>();
      System.Func<ExtensionEvent, IEnumerable<SqlDataRecord>> selector = (System.Func<ExtensionEvent, IEnumerable<SqlDataRecord>>) (row =>
      {
        List<SqlDataRecord> sqlDataRecordList = new List<SqlDataRecord>();
        if (row != null)
        {
          ExtensionEvent extensionEvent = row;
          SqlDataRecord record = new SqlDataRecord(ExtensionEventsTableValuedParameterExtensions.typ_ExtensionEventsTable3);
          record.SetInt64(0, extensionEvent.Id);
          record.SetGuid(1, Guid.Empty);
          record.SetString(2, extensionEvent.Version);
          record.SetDateTime(3, extensionEvent.StatisticDate);
          record.SetInt32(4, 999);
          if (extensionEvent.Properties == null)
            extensionEvent.Properties = new JObject();
          string str = extensionEvent.Properties.Serialize<JObject>();
          record.SetString(5, str, BindStringBehavior.EmptyStringToNull);
          sqlDataRecordList.Add(record);
        }
        return (IEnumerable<SqlDataRecord>) sqlDataRecordList;
      });
      IEnumerable<SqlDataRecord> rows1 = rows.SelectMany<ExtensionEvent, SqlDataRecord>(selector);
      return component.BindTable(parameterName, "Gallery.typ_ExtensionEventsTable3", rows1);
    }
  }
}
