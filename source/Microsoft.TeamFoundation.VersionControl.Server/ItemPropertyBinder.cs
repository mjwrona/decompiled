// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemPropertyBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemPropertyBinder : VersionControlObjectBinder<ItemPropertyPair>
  {
    private SqlColumnBinder serverItem = new SqlColumnBinder("ServerItem");
    private SqlColumnBinder propertyId = new SqlColumnBinder("PropertyId");

    protected override ItemPropertyPair Bind() => new ItemPropertyPair()
    {
      ServerItem = DBPath.DatabaseToServerPath(this.serverItem.GetString((IDataReader) this.Reader, false)),
      PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1)
    };
  }
}
