// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServiceObjectiveEnableStateColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseServiceObjectiveEnableStateColumns : 
    ObjectBinder<ServiceObjectiveEnableState>
  {
    private SqlColumnBinder objectiveStateColumn = new SqlColumnBinder("state_desc");

    protected override ServiceObjectiveEnableState Bind()
    {
      string str = this.objectiveStateColumn.GetString((IDataReader) this.Reader, false);
      try
      {
        return (ServiceObjectiveEnableState) Enum.Parse(typeof (ServiceObjectiveEnableState), str, true);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to parse the service objective enable state strings from Database to corresponding enum!", ex);
      }
    }
  }
}
