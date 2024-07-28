// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.MarkdownRenderer.IMarkdownRendererService
// Assembly: Microsoft.TeamFoundation.MarkdownRenderer.Sdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF23BBEE-E2C7-4A34-A6FB-0292D3B7C69D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.MarkdownRenderer.Sdk.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;

namespace Microsoft.TeamFoundation.MarkdownRenderer
{
  [DefaultServiceImplementation(typeof (MarkdownRendererService))]
  public interface IMarkdownRendererService : IVssFrameworkService
  {
    string ToHtml(string markdown);

    RenderedHtml ToHtmlWithMentions(
      IVssRequestContext requestContext,
      IMentionSourceContext sourceContext,
      string markdown);
  }
}
