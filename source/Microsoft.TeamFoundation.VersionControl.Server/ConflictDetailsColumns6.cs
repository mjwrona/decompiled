// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ConflictDetailsColumns6
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ConflictDetailsColumns6 : ConflictDetailsColumns
  {
    protected SqlColumnBinder yourPropertyId = new SqlColumnBinder("YourPropertyId");
    protected SqlColumnBinder theirPropertyId = new SqlColumnBinder("TheirPropertyId");
    protected SqlColumnBinder basePropertyId = new SqlColumnBinder("BasePropertyId");

    public ConflictDetailsColumns6()
    {
    }

    public ConflictDetailsColumns6(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Conflict Bind()
    {
      Conflict conflict = base.Bind();
      conflict.YourPropertyId = this.yourPropertyId.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirPropertyId = this.theirPropertyId.GetInt32((IDataReader) this.Reader, 0);
      conflict.BasePropertyId = this.basePropertyId.GetInt32((IDataReader) this.Reader, 0);
      return conflict;
    }
  }
}
