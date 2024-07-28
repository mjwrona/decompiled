// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InspectShelvesetsBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class InspectShelvesetsBinder : VersionControlObjectBinder<InspectShelvesetInfo>
  {
    private SqlColumnBinder rows = new SqlColumnBinder("Rows");
    private SqlColumnBinder date = new SqlColumnBinder("Date");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder owner = new SqlColumnBinder("Owner");

    protected override InspectShelvesetInfo Bind() => new InspectShelvesetInfo()
    {
      Rows = this.rows.GetInt64((IDataReader) this.Reader),
      Date = this.date.GetDateTime((IDataReader) this.Reader),
      ShelvesetName = this.name.GetString((IDataReader) this.Reader, false),
      OwnerId = this.owner.GetGuid((IDataReader) this.Reader)
    };
  }
}
