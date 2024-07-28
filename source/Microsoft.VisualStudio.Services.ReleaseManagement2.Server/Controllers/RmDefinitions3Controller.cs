// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitions3Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "definitions", ResourceVersion = 3)]
  public class RmDefinitions3Controller : RmDefinitions2Controller
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition IncomingToLatest(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition incomingDefinition,
      bool isUpdateReleaseDefinition = true)
    {
      if (incomingDefinition == null)
        return incomingDefinition;
      if (incomingDefinition.Environments != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition) null;
        if (isUpdateReleaseDefinition)
          releaseDefinition = this.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(this.TfsRequestContext, this.ProjectId, incomingDefinition.Id);
        foreach (ReleaseDefinitionEnvironment environment1 in (IEnumerable<ReleaseDefinitionEnvironment>) incomingDefinition.Environments)
        {
          if (releaseDefinition != null)
          {
            DefinitionEnvironment environment2 = releaseDefinition.GetEnvironment(environment1.Id);
            if (environment2 != null)
            {
              environment1.HandleDeploymentGatesCompatibility(environment2);
              environment1.HandleEnvironmentTriggersCompatibility(environment2);
            }
          }
        }
      }
      this.ValidateObsoleteDeployStep(incomingDefinition);
      incomingDefinition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DesignerPipelineProcess();
      return incomingDefinition;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      definition.ToUndefinedDeployStepTasks();
      definition.ToUndefinedReleaseDefinitionSource();
      definition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) null;
      return definition;
    }

    protected virtual void ValidateObsoleteDeployStep(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null || definition.Environments == null)
        return;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        if (environment != null && environment.DeployStep != null && environment.DeployStep.Tasks != null && environment.DeployStep.Tasks.Count > 0)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TasksNotAllowedInDeployStep, (object) environment.Name));
      }
    }
  }
}
