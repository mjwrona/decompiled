// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsWebClient
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsWebClient : WebClient
  {
    private TfsConnection m_tfs;

    public TfsWebClient(TfsConnection tfs)
    {
      this.m_tfs = tfs;
      this.Credentials = this.m_tfs.Credentials;
    }

    protected override WebRequest GetWebRequest(Uri address) => (WebRequest) TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) base.GetWebRequest(address), this.m_tfs);
  }
}
