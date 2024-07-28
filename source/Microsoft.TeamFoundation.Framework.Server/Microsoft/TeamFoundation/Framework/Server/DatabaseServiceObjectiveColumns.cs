// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServiceObjectiveColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseServiceObjectiveColumns : ObjectBinder<DatabaseServiceObjective>
  {
    private SqlColumnBinder objectiveColumn = new SqlColumnBinder("Objective");

    protected override DatabaseServiceObjective Bind()
    {
      string str = this.objectiveColumn.GetString((IDataReader) this.Reader, false);
      try
      {
        return (DatabaseServiceObjective) Enum.Parse(typeof (DatabaseServiceObjective), str, true);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to parse the service obejective strings from Database to corresponding enum!", ex);
      }
    }
  }
}
