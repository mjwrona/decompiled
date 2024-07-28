// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyBinderOld
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProxyBinderOld : ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>
  {
    private SqlColumnBinder UrlColumn = new SqlColumnBinder("Url");
    private SqlColumnBinder SiteColumn = new SqlColumnBinder("Site");
    private SqlColumnBinder FriendlyNameColumn = new SqlColumnBinder("FriendlyName");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder FlagsColumn = new SqlColumnBinder("Flags");

    protected override Microsoft.TeamFoundation.Core.WebApi.Proxy Bind()
    {
      Microsoft.TeamFoundation.Core.WebApi.Proxy proxy = new Microsoft.TeamFoundation.Core.WebApi.Proxy();
      proxy.Url = this.UrlColumn.GetString((IDataReader) this.Reader, false);
      proxy.Site = this.SiteColumn.GetString((IDataReader) this.Reader, true);
      proxy.FriendlyName = this.FriendlyNameColumn.GetString((IDataReader) this.Reader, true);
      proxy.Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true);
      int int32 = this.FlagsColumn.GetInt32((IDataReader) this.Reader, 0);
      if ((int32 & 1) == 1)
        proxy.SiteDefault = new bool?(true);
      if ((int32 & 2) == 2)
        proxy.GlobalDefault = new bool?(true);
      return proxy;
    }
  }
}
