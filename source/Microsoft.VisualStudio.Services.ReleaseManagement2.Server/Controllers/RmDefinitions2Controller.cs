// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitions2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "definitions", ResourceVersion = 2)]
  public class RmDefinitions2Controller : RmDefinitionsController
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientExample("GET__GetReleaseDefinition.json", "Get a release definition", null, null)]
    public override ReleaseDefinition GetReleaseDefinition(int definitionId, [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null) => base.GetReleaseDefinition(definitionId, propertyFilters).ToWebApiDefinitionV2();

    [HttpPost]
    [ReleaseManagementSecurityPermission("releaseDefinition", ReleaseManagementSecurityArgumentType.ReleaseDefinition, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    [ClientExample("POST__CreateReleaseDefinition.json", "Create release definition", null, null)]
    public override ReleaseDefinition CreateReleaseDefinition([FromBody] ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new InvalidRequestException(Resources.ReleaseDefinitionCannotBeNull);
      RmDefinitions2Controller.ValidateRetentionPolicyAtEnvironmentLevel(releaseDefinition);
      releaseDefinition.RetentionPolicy = (RetentionPolicy) null;
      return this.CreateReleaseDefinitionInternal(releaseDefinition).ToWebApiDefinitionV2();
    }

    [HttpPut]
    [ReleaseManagementSecurityPermission("releaseDefinition", ReleaseManagementSecurityArgumentType.UpdateReleaseDefinition, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    [ClientExample("PUT__UpdateReleaseDefinition.json", "Update the release definition", null, null)]
    public override ReleaseDefinition UpdateReleaseDefinition([FromBody] ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new InvalidRequestException(Resources.ReleaseDefinitionCannotBeNull);
      RmDefinitions2Controller.ValidateRetentionPolicyAtEnvironmentLevel(releaseDefinition);
      releaseDefinition.RetentionPolicy = (RetentionPolicy) null;
      return this.UpdateReleaseDefinitionInternal(releaseDefinition).ToWebApiDefinitionV2();
    }

    private static void ValidateRetentionPolicyAtEnvironmentLevel(
      ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition.Environments == null)
        return;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        if (environment.RetentionPolicy == null)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentRetentionPolicyCannotBeNull, (object) environment.Name));
      }
    }
  }
}
