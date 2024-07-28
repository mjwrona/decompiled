// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TextTemplating.IMustacheTemplateParserService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.TextTemplating
{
  [DefaultServiceImplementation(typeof (MustacheTemplateParserService))]
  public interface IMustacheTemplateParserService : IVssFrameworkService
  {
    MustacheTemplateParser GetTemplateParser(
      IVssRequestContext requestContext,
      bool usePluginHelpers = true,
      bool useDefaultHandlebarHelpers = true,
      bool useCommonTemplateHelpers = true);

    IServerMustacheTemplateExpression ParseServerTemplate(
      IVssRequestContext requestContext,
      string templateExpression,
      bool usePluginHelpers = true,
      bool useDefaultHandlebarHelpers = true,
      bool useCommonTemplateHelpers = true);
  }
}
