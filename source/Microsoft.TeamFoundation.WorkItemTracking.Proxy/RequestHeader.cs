// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RequestHeader
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class RequestHeader : TfsMessageHeader
  {
    private string m_requestId;

    public RequestHeader(string id) => this.m_requestId = id;

    public override string Name => nameof (RequestHeader);

    public override string Namespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer)
    {
      writer.WriteElementString("Id", this.Namespace, this.m_requestId);
      writer.WriteElementString("UseDisambiguatedIdentityString", this.Namespace, "1");
    }
  }
}
