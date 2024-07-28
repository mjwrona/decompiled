// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.MarkdownRenderer.RenderedHtml
// Assembly: Microsoft.TeamFoundation.MarkdownRenderer.Sdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF23BBEE-E2C7-4A34-A6FB-0292D3B7C69D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.MarkdownRenderer.Sdk.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.MarkdownRenderer
{
  public class RenderedHtml
  {
    public RenderedHtml(string html, IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions)
    {
      this.Html = html;
      this.Mentions = mentions;
    }

    public string Html { get; }

    public IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> Mentions { get; }
  }
}
