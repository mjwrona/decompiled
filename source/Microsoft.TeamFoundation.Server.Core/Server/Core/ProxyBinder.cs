// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyBinder
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProxyBinder : ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>
  {
    private SqlColumnBinder UrlColumn = new SqlColumnBinder("Url");
    private SqlColumnBinder SiteColumn = new SqlColumnBinder("Site");
    private SqlColumnBinder FriendlyNameColumn = new SqlColumnBinder("FriendlyName");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder SiteDefaultColumn = new SqlColumnBinder("SiteDefault");
    private SqlColumnBinder GlobalDefaultColumn = new SqlColumnBinder("GlobalDefault");

    protected override Microsoft.TeamFoundation.Core.WebApi.Proxy Bind() => new Microsoft.TeamFoundation.Core.WebApi.Proxy()
    {
      Url = this.UrlColumn.GetString((IDataReader) this.Reader, false),
      Site = this.SiteColumn.GetString((IDataReader) this.Reader, true),
      FriendlyName = this.FriendlyNameColumn.GetString((IDataReader) this.Reader, true),
      Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
      SiteDefault = new bool?(this.SiteDefaultColumn.GetBoolean((IDataReader) this.Reader, false)),
      GlobalDefault = new bool?(this.GlobalDefaultColumn.GetBoolean((IDataReader) this.Reader, false))
    };
  }
}
