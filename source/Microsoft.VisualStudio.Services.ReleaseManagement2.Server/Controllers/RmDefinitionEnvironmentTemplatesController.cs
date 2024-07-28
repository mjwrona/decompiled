// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitionEnvironmentTemplatesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environmenttemplates")]
  public class RmDefinitionEnvironmentTemplatesController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a REST API.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<ReleaseDefinitionEnvironmentTemplate> ListDefinitionEnvironmentTemplates(
      bool isDeleted = false)
    {
      return this.LatestToIncoming(this.GetService<DefinitionEnvironmentTemplatesService>().ListDefinitionEnvironmentTemplates(this.TfsRequestContext, this.ProjectId, isDeleted));
    }

    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public ReleaseDefinitionEnvironmentTemplate GetDefinitionEnvironmentTemplate(Guid templateId) => this.LatestToIncoming(this.GetService<DefinitionEnvironmentTemplatesService>().GetDefinitionEnvironmentTemplate(this.TfsRequestContext, this.ProjectId, templateId));

    [HttpPost]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    public ReleaseDefinitionEnvironmentTemplate CreateDefinitionEnvironmentTemplate(
      [FromBody] ReleaseDefinitionEnvironmentTemplate template)
    {
      template = this.IncomingToLatest(template);
      return this.LatestToIncoming(this.GetService<DefinitionEnvironmentTemplatesService>().SaveDefinitionEnvironmentTemplate(this.TfsRequestContext, this.ProjectId, template));
    }

    [HttpDelete]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.DeleteReleaseDefinition)]
    public void DeleteDefinitionEnvironmentTemplate(Guid templateId) => this.GetService<DefinitionEnvironmentTemplatesService>().SoftDeleteDefinitionEnvironmentTemplate(this.TfsRequestContext, this.ProjectId, new Guid?(templateId));

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected IEnumerable<ReleaseDefinitionEnvironmentTemplate> LatestToIncoming(
      IEnumerable<ReleaseDefinitionEnvironmentTemplate> environmentTemplates)
    {
      return environmentTemplates != null ? environmentTemplates.Select<ReleaseDefinitionEnvironmentTemplate, ReleaseDefinitionEnvironmentTemplate>((Func<ReleaseDefinitionEnvironmentTemplate, ReleaseDefinitionEnvironmentTemplate>) (et => this.LatestToIncoming(et))) : throw new ArgumentNullException(nameof (environmentTemplates));
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected virtual ReleaseDefinitionEnvironmentTemplate LatestToIncoming(
      ReleaseDefinitionEnvironmentTemplate environmentTemplate)
    {
      if (environmentTemplate == null)
        throw new InvalidRequestException(Resources.EnvironmentTemplateCannotBeNull);
      if (environmentTemplate.Environment != null && environmentTemplate.Environment.DeployPhases.Any<DeployPhase>())
        environmentTemplate.Environment.ToNoPhasesFormat();
      return environmentTemplate;
    }

    protected virtual ReleaseDefinitionEnvironmentTemplate IncomingToLatest(
      ReleaseDefinitionEnvironmentTemplate environmentTemplate)
    {
      if (environmentTemplate == null)
        throw new InvalidRequestException(Resources.EnvironmentTemplateCannotBeNull);
      if (environmentTemplate.Environment != null)
        environmentTemplate.Environment.ToDeployPhasesFormat();
      return environmentTemplate;
    }
  }
}
