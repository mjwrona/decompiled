// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitionTemplates4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "templates", ResourceVersion = 3)]
  public class BuildDefinitionTemplates4Controller : BuildApiController
  {
    [HttpGet]
    public IList<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate> GetTemplates() => this.GetTemplatesInternal(true);

    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate GetTemplate(
      string templateId)
    {
      return this.GetTemplateInternal(templateId, true);
    }

    [HttpPut]
    public Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate SaveTemplate(
      string templateId,
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate template)
    {
      this.CheckRequestContent((object) template);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.BuildDefinition>(template.Template, "template.Template");
      template.Id = templateId;
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> keyValuePair in template.Template.Variables.Where<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>((Func<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>, bool>) (p => p.Value.IsSecret)))
      {
        keyValuePair.Value.IsSecret = false;
        keyValuePair.Value.Value = string.Empty;
      }
      return this.TfsRequestContext.GetService<IDefinitionTemplateService>().SaveTemplate(this.TfsRequestContext, this.ProjectId, template.ToBuild2ServerBuildDefinitionTemplate()).ToWebApiBuildDefinitionTemplate(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    [HttpDelete]
    public void DeleteTemplate(string templateId) => this.TfsRequestContext.GetService<IDefinitionTemplateService>().DeleteTemplate(this.TfsRequestContext, this.ProjectId, templateId);

    protected IList<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate> GetTemplatesInternal(
      bool withProcessParameters)
    {
      IDefinitionTemplateService service = this.TfsRequestContext.GetService<IDefinitionTemplateService>();
      IdentityMap identityMap = new IdentityMap(this.TfsRequestContext.GetService<IdentityService>());
      AgentPoolQueueCache queueCache = new AgentPoolQueueCache(this.TfsRequestContext);
      BuildRepositoryCache repositoryCache = new BuildRepositoryCache(this.TfsRequestContext);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      int num = withProcessParameters ? 1 : 0;
      return (IList<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate>) service.GetTemplates(tfsRequestContext, projectId, num != 0).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate>) (x => x.ToWebApiBuildDefinitionTemplate(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion, identityMap, queueCache, repositoryCache))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate>();
    }

    protected Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate GetTemplateInternal(
      string templateId,
      bool withProcessParameters)
    {
      return (this.TfsRequestContext.GetService<IDefinitionTemplateService>().GetTemplate(this.TfsRequestContext, this.ProjectId, templateId, withProcessParameters) ?? throw new DefinitionTemplateNotFoundException(Resources.DefinitionTemplateNotFound((object) templateId))).ToWebApiBuildDefinitionTemplate(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }
  }
}
