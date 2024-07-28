// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RecordListenerComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RecordListenerComponent7 : RecordListenerComponent6
  {
    private static readonly SqlMetaData[] typ_CommandTable6 = new SqlMetaData[17]
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
      new SqlMetaData("CommandIdentifier", SqlDbType.NVarChar, 128L),
      new SqlMetaData("AuthenticationType", SqlDbType.VarChar, 64L),
      new SqlMetaData("AgentId", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ResponseCode", SqlDbType.Int),
      new SqlMetaData("TimeToFirstPage", SqlDbType.BigInt),
      new SqlMetaData("DelayTime", SqlDbType.BigInt)
    };

    protected override SqlMetaData[] CommandTableType => RecordListenerComponent7.typ_CommandTable6;

    protected override string CommandTableTypeString => "typ_CommandTable6";

    protected override SqlDataRecord BindCommandRow(
      RequestDetails requestDetails,
      SqlMetaData[] type)
    {
      SqlDataRecord sqlDataRecord = base.BindCommandRow(requestDetails, type);
      sqlDataRecord.SetInt64(16, requestDetails.DelayTime);
      return sqlDataRecord;
    }
  }
}
