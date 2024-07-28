// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementViewResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementViewResult
  {
    public short SessionId;
    public int Seconds;
    public double ElapsedTime;
    public string Command;
    public short BlockingSessionId;
    public short HeadBlockerSessionId;
    public int BlockingLevel;
    public string Text;
    public string Statement;
    public string BlockerQueryText;
    public string WaitType;
    public int WaitTime;
    public string LastWaitType;
    public string WaitResource;
    public long Reads;
    public long Writes;
    public long LogicalReads;
    public int CpuTime;
    public int GrantedQueryMemory;
    public long RequestedMemory;
    public long MaxUsedMemory;
    public short Dop;
    public string QueryPlan;
    public string DatabaseName;
    public string LoginName;

    public override string ToString() => string.Format("session_id:{0} seconds:{1} elapsed_time:{2} command:{3} blocking_session_id:{4} head_blocker_session_id:{5} blocking_level:{6} text:{7} stmt:{8}  blocker_query_text:{9} wait_type:{10} wait_time:{11} last_wait_type:{12} wait_resource:{13} reads:{14} writes:{15} logical_reads:{16} cpu_time:{17} granted_query_memory:{18} requested_memory: {19} kb max_used_memory: {20} kb, dop: {21}, query_plan: {22}, databaseName: {23}, login_name:{24}", (object) this.SessionId, (object) this.Seconds, (object) this.ElapsedTime, (object) this.Command, (object) this.BlockingSessionId, (object) this.HeadBlockerSessionId, (object) this.BlockingLevel, (object) this.Text, (object) this.Statement, (object) this.BlockerQueryText, (object) this.WaitType, (object) this.WaitTime, (object) this.LastWaitType, (object) this.WaitResource, (object) this.Reads, (object) this.Writes, (object) this.LogicalReads, (object) this.CpuTime, (object) this.GrantedQueryMemory, (object) this.RequestedMemory, (object) this.MaxUsedMemory, (object) this.Dop, (object) this.QueryPlan, (object) this.DatabaseName, (object) this.LoginName);
  }
}
