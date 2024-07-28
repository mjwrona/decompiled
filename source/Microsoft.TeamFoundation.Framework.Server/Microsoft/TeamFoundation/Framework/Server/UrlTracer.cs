// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UrlTracer : IUrlTracer
  {
    private readonly object m_url;
    private string m_scrubbedUrl;

    public UrlTracer(string url) => this.m_url = (object) url;

    public UrlTracer(Uri url) => this.m_url = (object) url;

    public override string ToString()
    {
      if (this.m_scrubbedUrl == null && this.m_url != null)
        this.m_scrubbedUrl = SecretUtility.ScrubUrlSignatures(this.m_url.ToString(), false);
      return this.m_scrubbedUrl;
    }
  }
}
