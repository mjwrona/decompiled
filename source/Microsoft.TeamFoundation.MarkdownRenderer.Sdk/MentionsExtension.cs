// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.MarkdownRenderer.MentionsExtension
// Assembly: Microsoft.TeamFoundation.MarkdownRenderer.Sdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF23BBEE-E2C7-4A34-A6FB-0292D3B7C69D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.MarkdownRenderer.Sdk.dll

using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.MarkdownRenderer
{
  public class MentionsExtension : IMarkdownExtension
  {
    private readonly WorkItemMentionParser WorkItemParser;
    private readonly MentionRenderer MentionsRenderer;

    public MentionsExtension(IVssRequestContext requestContext, IMentionSourceContext sourceContext)
    {
      this.WorkItemParser = new WorkItemMentionParser(sourceContext);
      this.MentionsRenderer = new MentionRenderer(requestContext, sourceContext.ProjectGuid);
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
      if (pipeline.InlineParsers.Contains<WorkItemMentionParser>())
        return;
      pipeline.InlineParsers.Insert(0, (InlineParser) this.WorkItemParser);
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
      if (!(renderer is HtmlRenderer htmlRenderer) || htmlRenderer.ObjectRenderers.Contains<MentionRenderer>())
        return;
      htmlRenderer.ObjectRenderers.Insert(0, (IMarkdownObjectRenderer) this.MentionsRenderer);
    }

    public IReadOnlyList<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions() => this.WorkItemParser.GetMentions();
  }
}
