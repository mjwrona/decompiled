// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CommandCountBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CommandCountBinder : ObjectBinder<CommandCount>
  {
    private SqlColumnBinder CallerColumn = new SqlColumnBinder("Caller");
    private SqlColumnBinder CountColumn = new SqlColumnBinder("CommandCount");
    private SqlColumnBinder ExecutionTimeInMinutesColumn = new SqlColumnBinder("ExecutionTimeInMinutes");

    protected override CommandCount Bind() => new CommandCount()
    {
      Caller = this.CallerColumn.GetString((IDataReader) this.Reader, true),
      Count = this.CountColumn.GetInt64((IDataReader) this.Reader, 0L),
      ExecutionTimeInMinutes = this.ExecutionTimeInMinutesColumn.GetDouble((IDataReader) this.Reader)
    };
  }
}
