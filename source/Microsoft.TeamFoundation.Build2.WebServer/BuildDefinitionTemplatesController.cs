// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitionTemplatesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "templates", ResourceVersion = 1)]
  public class BuildDefinitionTemplatesController : BuildApiController
  {
    [HttpGet]
    public virtual IList<BuildDefinitionTemplate3_2> GetTemplates() => this.GetTemplatesInternal(false);

    [HttpGet]
    public virtual BuildDefinitionTemplate3_2 GetTemplate(string templateId) => this.GetTemplateInternal(templateId, false);

    [HttpPut]
    public BuildDefinitionTemplate3_2 SaveTemplate(
      string templateId,
      [FromBody] BuildDefinitionTemplate3_2 template)
    {
      this.CheckRequestContent((object) template);
      ArgumentUtility.CheckForNull<BuildDefinition3_2>(template.Template, "template.Template");
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate definitionTemplate = template.ToBuildDefinitionTemplate();
      definitionTemplate.Id = templateId;
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> keyValuePair in definitionTemplate.Template.Variables.Where<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>((Func<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>, bool>) (p => p.Value.IsSecret)))
      {
        keyValuePair.Value.IsSecret = false;
        keyValuePair.Value.Value = string.Empty;
      }
      return this.TfsRequestContext.GetService<IDefinitionTemplateService>().SaveTemplate(this.TfsRequestContext, this.ProjectId, definitionTemplate.ToBuild2ServerBuildDefinitionTemplate()).ToWebApiBuildDefinitionTemplate(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion).ToBuildDefinitionTemplate3_2();
    }

    [HttpDelete]
    public void DeleteTemplate(string templateId) => this.TfsRequestContext.GetService<IDefinitionTemplateService>().DeleteTemplate(this.TfsRequestContext, this.ProjectId, templateId);

    protected IList<BuildDefinitionTemplate3_2> GetTemplatesInternal(bool withProcessParameters) => (IList<BuildDefinitionTemplate3_2>) this.TfsRequestContext.GetService<IDefinitionTemplateService>().GetTemplates(this.TfsRequestContext, this.ProjectId, withProcessParameters).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate, BuildDefinitionTemplate3_2>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate, BuildDefinitionTemplate3_2>) (t => t.ToWebApiBuildDefinitionTemplate(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion).ToBuildDefinitionTemplate3_2())).ToList<BuildDefinitionTemplate3_2>();

    protected BuildDefinitionTemplate3_2 GetTemplateInternal(
      string templateId,
      bool withProcessParameters)
    {
      return (this.TfsRequestContext.GetService<IDefinitionTemplateService>().GetTemplate(this.TfsRequestContext, this.ProjectId, templateId, withProcessParameters) ?? throw new DefinitionTemplateNotFoundException(Resources.DefinitionTemplateNotFound((object) templateId))).ToWebApiBuildDefinitionTemplate(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion).ToBuildDefinitionTemplate3_2();
    }
  }
}
