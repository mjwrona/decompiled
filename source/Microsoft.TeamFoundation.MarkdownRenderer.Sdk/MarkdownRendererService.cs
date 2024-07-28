// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.MarkdownRenderer.MarkdownRendererService
// Assembly: Microsoft.TeamFoundation.MarkdownRenderer.Sdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF23BBEE-E2C7-4A34-A6FB-0292D3B7C69D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.MarkdownRenderer.Sdk.dll

using Markdig;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.MarkdownRenderer
{
  public class MarkdownRendererService : IMarkdownRendererService, IVssFrameworkService
  {
    private MarkdownPipeline defaultPipeline;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.defaultPipeline = this.GetNewPipelineBuilder().Build();

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.defaultPipeline = (MarkdownPipeline) null;

    public string ToHtml(string markdown) => Markdown.ToHtml(markdown, this.defaultPipeline);

    public RenderedHtml ToHtmlWithMentions(
      IVssRequestContext requestContext,
      IMentionSourceContext sourceContext,
      string markdown)
    {
      MarkdownPipelineBuilder newPipelineBuilder = this.GetNewPipelineBuilder();
      IReadOnlyCollection<Microsoft.TeamFoundation.Mention.Server.Mention> mentions = (IReadOnlyCollection<Microsoft.TeamFoundation.Mention.Server.Mention>) new List<Microsoft.TeamFoundation.Mention.Server.Mention>();
      string html;
      if (sourceContext != null)
      {
        MentionsExtension mentionsExtension = new MentionsExtension(requestContext, sourceContext);
        newPipelineBuilder.Extensions.Add((IMarkdownExtension) mentionsExtension);
        html = Markdown.ToHtml(markdown, newPipelineBuilder.Build());
        mentions = (IReadOnlyCollection<Microsoft.TeamFoundation.Mention.Server.Mention>) mentionsExtension.GetMentions();
      }
      else
        html = Markdown.ToHtml(markdown, newPipelineBuilder.Build());
      return new RenderedHtml(html, (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) mentions);
    }

    private MarkdownPipelineBuilder GetNewPipelineBuilder() => new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().DisableHtml().UseTaskLists().UsePipeTables().UseReferralLinks("nofollow").UseEmojiAndSmiley();
  }
}
