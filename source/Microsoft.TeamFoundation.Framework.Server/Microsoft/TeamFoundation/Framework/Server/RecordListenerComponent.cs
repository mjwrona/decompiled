// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RecordListenerComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RecordListenerComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<RecordListenerComponent>(1, true),
      (IComponentCreator) new ComponentCreator<RecordListenerComponent2>(2),
      (IComponentCreator) new ComponentCreator<RecordListenerComponent3>(3),
      (IComponentCreator) new ComponentCreator<RecordListenerComponent4>(4),
      (IComponentCreator) new ComponentCreator<RecordListenerComponent5>(5),
      (IComponentCreator) new ComponentCreator<RecordListenerComponent6>(6),
      (IComponentCreator) new ComponentCreator<RecordListenerComponent7>(7)
    }, "RecordListener");
    private static readonly SqlMetaData[] typ_ParamTable = new SqlMetaData[4]
    {
      new SqlMetaData("TempCorrelationId", SqlDbType.Int),
      new SqlMetaData("ParameterName", SqlDbType.VarChar, 64L),
      new SqlMetaData("ParameterIndex", SqlDbType.Int),
      new SqlMetaData("ParameterValue", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_CommandTable = new SqlMetaData[12]
    {
      new SqlMetaData("TempCorrelationId", SqlDbType.Int),
      new SqlMetaData("Application", SqlDbType.VarChar, 64L),
      new SqlMetaData("Command", SqlDbType.VarChar, 256L),
      new SqlMetaData("ExecutionCount", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StartTime", SqlDbType.DateTime2),
      new SqlMetaData("ExecutionTime", SqlDbType.BigInt),
      new SqlMetaData("IdentityName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("IPAddress", SqlDbType.VarChar, 40L),
      new SqlMetaData("UniqueIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UserAgent", SqlDbType.NVarChar, 128L),
      new SqlMetaData("CommandIdentifier", SqlDbType.NVarChar, 128L)
    };
    private const int s_parameterKeyLength = 64;

    protected SqlParameter BindParameterTable(
      string parameterName,
      IEnumerable<RequestDetails> rows,
      bool includeDetails,
      int maxCompressedThresholdTime)
    {
      rows = rows ?? Enumerable.Empty<RequestDetails>();
      System.Func<RequestDetails, IEnumerable<SqlDataRecord>> selector = (System.Func<RequestDetails, IEnumerable<SqlDataRecord>>) (requestDetails => this.BindRequestDetails(requestDetails, includeDetails, maxCompressedThresholdTime, RecordListenerComponent.typ_ParamTable));
      return this.BindTable(parameterName, "typ_ParamTable", (IEnumerable<SqlDataRecord>) rows.SelectMany<RequestDetails, SqlDataRecord>(selector).ToList<SqlDataRecord>());
    }

    protected virtual IEnumerable<SqlDataRecord> BindRequestDetails(
      RequestDetails requestDetails,
      bool includeDetails,
      int maxCompressedThresholdTime,
      SqlMetaData[] type)
    {
      int paramIndex = 0;
      bool includeMethodDetails = requestDetails.IncludeMethodDetails(includeDetails, maxCompressedThresholdTime);
      if (includeMethodDetails)
      {
        string[] strArray = requestDetails.Method.Parameters.AllKeys;
        for (int index = 0; index < strArray.Length; ++index)
        {
          string name = strArray[index];
          SqlDataRecord record = new SqlDataRecord(type);
          record.SetInt32(0, requestDetails.TempCorrelationId);
          record.SetString(1, RecordListenerComponent.MakeStringValue(name, 64L));
          record.SetInt32(2, paramIndex++);
          record.SetNullableString(3, requestDetails.Method.Parameters[name]);
          yield return record;
        }
        strArray = (string[]) null;
      }
      paramIndex = -1;
      if (requestDetails.Status != null)
      {
        SqlDataRecord record1 = new SqlDataRecord(type);
        record1.SetInt32(0, requestDetails.TempCorrelationId);
        record1.SetString(1, "ExceptionType");
        record1.SetInt32(2, paramIndex--);
        record1.SetNullableString(3, requestDetails.Status.GetType().Name);
        yield return record1;
        SqlDataRecord record2 = new SqlDataRecord(type);
        record2.SetInt32(0, requestDetails.TempCorrelationId);
        record2.SetString(1, "ExceptionMessage");
        record2.SetInt32(2, paramIndex--);
        record2.SetNullableString(3, requestDetails.Status.Message);
        yield return record2;
      }
      if (includeMethodDetails)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(type);
        sqlDataRecord.SetInt32(0, requestDetails.TempCorrelationId);
        sqlDataRecord.SetString(1, "ActivityId");
        sqlDataRecord.SetInt32(2, paramIndex--);
        sqlDataRecord.SetSqlString(3, (SqlString) requestDetails.ActivityId.ToString());
        yield return sqlDataRecord;
      }
      IList<MethodTime> recursiveSqlCalls = requestDetails.RecursiveSqlCalls;
      if (recursiveSqlCalls != null && recursiveSqlCalls.Count > 0)
      {
        foreach (MethodTime methodTime in (IEnumerable<MethodTime>) recursiveSqlCalls)
        {
          string str = methodTime.LogicalReads != 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ms, {1} logical reads.", (object) (int) methodTime.Time.TotalMilliseconds, (object) methodTime.LogicalReads) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ms", (object) (int) methodTime.Time.TotalMilliseconds);
          SqlDataRecord sqlDataRecord = new SqlDataRecord(type);
          sqlDataRecord.SetInt32(0, requestDetails.TempCorrelationId);
          sqlDataRecord.SetString(1, methodTime.Name);
          sqlDataRecord.SetInt32(2, paramIndex--);
          sqlDataRecord.SetString(3, str);
          yield return sqlDataRecord;
        }
      }
      if (requestDetails.LogItemsObject != null && requestDetails.LogItemsObject is Dictionary<string, string> logItemsObject && logItemsObject.Count > 0)
      {
        foreach (KeyValuePair<string, string> keyValuePair in logItemsObject)
        {
          SqlDataRecord record = new SqlDataRecord(type);
          record.SetInt32(0, requestDetails.TempCorrelationId);
          record.SetString(1, RecordListenerComponent.MakeStringValue(keyValuePair.Key, 64L));
          record.SetInt32(2, paramIndex--);
          record.SetNullableString(3, keyValuePair.Value);
          yield return record;
        }
      }
    }

    protected static string MakeStringValue(string value, long maxLength)
    {
      if (value == null)
        return string.Empty;
      return value.Length > (int) maxLength ? value.Substring(0, (int) maxLength) : value;
    }

    protected SqlParameter BindCommandTable(string parameterName, IEnumerable<RequestDetails> rows)
    {
      rows = rows ?? Enumerable.Empty<RequestDetails>();
      System.Func<RequestDetails, SqlDataRecord> selector = (System.Func<RequestDetails, SqlDataRecord>) (requestDetails => this.BindCommandRow(requestDetails, this.CommandTableType));
      return this.BindTable(parameterName, this.CommandTableTypeString, (IEnumerable<SqlDataRecord>) rows.Select<RequestDetails, SqlDataRecord>(selector).ToList<SqlDataRecord>());
    }

    protected virtual SqlMetaData[] CommandTableType => RecordListenerComponent.typ_CommandTable;

    protected virtual string CommandTableTypeString => "typ_CommandTable";

    protected virtual SqlDataRecord BindCommandRow(
      RequestDetails requestDetails,
      SqlMetaData[] type)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(type);
      sqlDataRecord.SetInt32(0, requestDetails.TempCorrelationId);
      sqlDataRecord.SetString(1, RecordListenerComponent.MakeStringValue(requestDetails.ServiceName, type[1].MaxLength));
      sqlDataRecord.SetString(2, RecordListenerComponent.MakeStringValue(requestDetails.Title, type[2].MaxLength));
      sqlDataRecord.SetInt32(3, requestDetails.Count);
      sqlDataRecord.SetInt32(4, requestDetails.Status == null ? 0 : -1);
      sqlDataRecord.SetDateTime(5, !requestDetails.StartTime.Equals(DateTime.MinValue) ? requestDetails.StartTime : DateTime.UtcNow);
      sqlDataRecord.SetInt64(6, requestDetails.ExecutionTime);
      if (requestDetails.AuthenticatedUserName == requestDetails.DomainUserName)
        sqlDataRecord.SetString(7, RecordListenerComponent.MakeStringValue(requestDetails.AuthenticatedUserName, type[7].MaxLength));
      else
        sqlDataRecord.SetString(7, RecordListenerComponent.MakeStringValue(requestDetails.AuthenticatedUserName + " (" + requestDetails.DomainUserName + ")", (long) (int) type[7].MaxLength));
      sqlDataRecord.SetString(8, RecordListenerComponent.MakeStringValue(requestDetails.RemoteIPAddress, type[8].MaxLength));
      sqlDataRecord.SetGuid(9, requestDetails.UniqueIdentifier);
      sqlDataRecord.SetString(10, RecordListenerComponent.MakeStringValue(requestDetails.UserAgent, type[10].MaxLength));
      sqlDataRecord.SetString(11, RecordListenerComponent.MakeStringValue(requestDetails.Command, type[11].MaxLength));
      return sqlDataRecord;
    }

    public virtual void SubmitRecords(
      IEnumerable<RequestDetails> requestList,
      bool includeDetails,
      int maxCompressedThresholdTime)
    {
      this.PrepareStoredProcedure("prc_LogActivity");
      this.BindCommandTable("@commands", requestList);
      this.BindParameterTable("@params", requestList, includeDetails, maxCompressedThresholdTime);
      this.ExecuteNonQuery();
    }

    public void PruneRecords(int maxRecordAgeInDays)
    {
      this.PrepareStoredProcedure("prc_PruneCommands", 3600);
      this.BindInt("@maxAgeDays", maxRecordAgeInDays);
      this.ExecuteNonQuery();
    }
  }
}
