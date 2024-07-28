// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component88
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component88 : Build2Component87
  {
    protected static readonly SqlMetaData[] typ_CheckEvents = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("EventType", SqlDbType.TinyInt),
      new SqlMetaData("Payload", SqlDbType.VarBinary, -1L)
    };

    public override List<BuildCheckEvent> AddCheckEvents(
      List<BuildCheckEvent> checkEvents,
      bool securityFixEnabled)
    {
      using (this.TraceScope(method: nameof (AddCheckEvents)))
      {
        this.PrepareStoredProcedure("Build.prc_AddCheckEvents");
        this.BindBuildCheckEventsTable("@checkEvents", (IEnumerable<BuildCheckEvent>) checkEvents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildCheckEvent>((ObjectBinder<BuildCheckEvent>) this.GetCheckEventBinder(securityFixEnabled));
          return resultCollection.GetCurrent<BuildCheckEvent>().Items;
        }
      }
    }

    protected SqlParameter BindBuildCheckEventsTable(
      string parameterName,
      IEnumerable<BuildCheckEvent> checkEvents)
    {
      checkEvents = checkEvents ?? Enumerable.Empty<BuildCheckEvent>();
      return this.BindTable(parameterName, "Build.typ_CheckEvents", checkEvents.Select<BuildCheckEvent, SqlDataRecord>((System.Func<BuildCheckEvent, SqlDataRecord>) (checkEvent =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component88.typ_CheckEvents);
        record.SetInt32(0, this.GetDataspaceId(checkEvent.ProjectId));
        record.SetInt32(1, checkEvent.BuildId);
        record.SetByte(2, (byte) checkEvent.EventType);
        if (checkEvent.Payload != null)
          record.SetNullableBinary(3, CheckEventSerializerUtil.Serialize((object) checkEvent.Payload));
        return record;
      })));
    }
  }
}
