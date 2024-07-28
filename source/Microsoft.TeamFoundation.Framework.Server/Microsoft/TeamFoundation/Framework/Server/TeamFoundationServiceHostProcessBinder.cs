// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostProcessBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class TeamFoundationServiceHostProcessBinder : 
    ObjectBinder<TeamFoundationServiceHostProcess>
  {
    private SqlColumnBinder MachineIdColumn = new SqlColumnBinder("MachineId");
    private SqlColumnBinder MachineNameColumn = new SqlColumnBinder("MachineName");
    private SqlColumnBinder ProcessIdColumn = new SqlColumnBinder("ProcessId");
    private SqlColumnBinder ProcessNameColumn = new SqlColumnBinder("ProcessName");
    private SqlColumnBinder PIDColumn = new SqlColumnBinder("PID");
    private SqlColumnBinder ProcessIdentityColumn = new SqlColumnBinder("ProcessIdentity");
    private SqlColumnBinder StartTimeColumn = new SqlColumnBinder("StartTime");

    protected override TeamFoundationServiceHostProcess Bind() => new TeamFoundationServiceHostProcess()
    {
      MachineId = this.MachineIdColumn.GetGuid((IDataReader) this.Reader),
      MachineName = this.MachineNameColumn.GetString((IDataReader) this.Reader, false),
      ProcessId = this.ProcessIdColumn.GetGuid((IDataReader) this.Reader, true),
      ProcessName = this.ProcessNameColumn.GetString((IDataReader) this.Reader, false),
      OSProcessId = this.PIDColumn.GetInt32((IDataReader) this.Reader),
      ProcessIdentity = this.ProcessIdentityColumn.GetString((IDataReader) this.Reader, false),
      StartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
