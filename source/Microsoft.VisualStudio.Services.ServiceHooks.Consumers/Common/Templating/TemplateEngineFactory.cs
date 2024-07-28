// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.TemplateEngineFactory
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating
{
  public static class TemplateEngineFactory
  {
    private static readonly ITemplateEngine s_defaultTemplatEngine = (ITemplateEngine) new MustacheTemplateEngine();

    public static ITemplateEngine CreateDefault() => TemplateEngineFactory.s_defaultTemplatEngine;
  }
}
