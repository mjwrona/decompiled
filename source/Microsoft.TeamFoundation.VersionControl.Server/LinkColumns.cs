// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LinkColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LinkColumns : VersionControlObjectBinder<VersionControlLink>
  {
    public SqlColumnBinder shelvesetId = new SqlColumnBinder("ShelvesetId");
    public SqlColumnBinder linkType = new SqlColumnBinder("LinkType");
    public SqlColumnBinder url = new SqlColumnBinder("Url");

    protected override VersionControlLink Bind() => new VersionControlLink()
    {
      LinkType = this.linkType.GetInt32((IDataReader) this.Reader),
      Url = this.url.GetString((IDataReader) this.Reader, false),
      ShelvesetId = this.shelvesetId.GetInt32((IDataReader) this.Reader)
    };
  }
}
