// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRequestMessageContextWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HttpRequestMessageContextWrapper : HttpContextBase
  {
    private HttpRequestMessageWrapper m_httpWrapper;
    private HttpRequestMessageContextWrapper.ShimHttpResponseMessage m_httpResponse;
    private Hashtable m_items;

    public HttpRequestMessageContextWrapper(string virtualPathRoot, HttpRequestMessage httpRequest)
    {
      this.m_httpWrapper = new HttpRequestMessageWrapper(virtualPathRoot, httpRequest);
      this.m_httpResponse = new HttpRequestMessageContextWrapper.ShimHttpResponseMessage();
      this.m_items = new Hashtable();
    }

    public override HttpRequestBase Request => (HttpRequestBase) this.m_httpWrapper;

    public override HttpResponseBase Response => (HttpResponseBase) this.m_httpResponse;

    public override IDictionary Items => (IDictionary) this.m_items;

    internal class ShimHttpResponseMessage : HttpResponseBase
    {
      public override string ApplyAppPathModifier(string virtualPath) => virtualPath;
    }
  }
}
