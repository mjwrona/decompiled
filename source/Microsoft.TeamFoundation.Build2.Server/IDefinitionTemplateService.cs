// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IDefinitionTemplateService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (DefinitionTemplateService))]
  public interface IDefinitionTemplateService : IVssFrameworkService
  {
    IList<BuildDefinitionTemplate> GetTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      bool withProcessParameters = false);

    IList<BuildDefinitionTemplate> GetCustomTemplates(
      IVssRequestContext requestContext,
      Guid projectId);

    BuildDefinitionTemplate GetTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      string templateId,
      bool withProcessParameters = false);

    BuildDefinitionTemplate SaveTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildDefinitionTemplate template);

    void DeleteTemplate(IVssRequestContext requestContext, Guid projectId, string templateId);

    void DeleteTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      List<string> templateIds);
  }
}
