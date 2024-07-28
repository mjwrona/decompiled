// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseDefinitionsSecureFileValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public class ReleaseDefinitionsSecureFileValidations
  {
    private const string SecureFileInputType = "secureFile";
    private readonly Func<IVssRequestContext, Guid, int, ReleaseDefinition> getReleaseDefinition;

    internal ReleaseDefinitionsSecureFileValidations(
      Func<IVssRequestContext, Guid, int, ReleaseDefinition> getReleaseDefinition)
    {
      this.getReleaseDefinition = getReleaseDefinition;
    }

    public ReleaseDefinitionsSecureFileValidations()
      : this((Func<IVssRequestContext, Guid, int, ReleaseDefinition>) ((requestContext, projectId, id) => requestContext.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(requestContext, projectId, id)))
    {
    }

    public void ValidateSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseDefinition releaseDefinition)
    {
      this.ValidateSecureFiles(requestContext, projectId, releaseDefinition, false);
    }

    public void ValidateSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      bool isUpdate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      HashSet<Guid> guidSet = releaseDefinition != null ? ReleaseDefinitionsSecureFileValidations.GetSecureFilesFromReleaseDefinition(requestContext, releaseDefinition) : throw new ArgumentNullException(nameof (releaseDefinition));
      HashSet<Guid> hashSet = requestContext.GetService<ISecureFileService>().GetSecureFiles(requestContext, projectId, (string) null, actionFilter: SecureFileActionFilter.Use).Select<SecureFile, Guid>((Func<SecureFile, Guid>) (x => x.Id)).ToHashSet<Guid>();
      if (isUpdate)
      {
        ReleaseDefinition releaseDefinition1 = this.getReleaseDefinition(requestContext, projectId, releaseDefinition.Id);
        HashSet<Guid> releaseDefinition2 = ReleaseDefinitionsSecureFileValidations.GetSecureFilesFromReleaseDefinition(requestContext, releaseDefinition1);
        ReleaseDefinitionsSecureFileValidations.ValidateSecureFiles(guidSet.Except<Guid>((IEnumerable<Guid>) releaseDefinition2), hashSet);
      }
      else
        ReleaseDefinitionsSecureFileValidations.ValidateSecureFiles((IEnumerable<Guid>) guidSet, hashSet);
    }

    private static HashSet<Guid> GetSecureFilesFromReleaseDefinition(
      IVssRequestContext requestContext,
      ReleaseDefinition releaseDefinition)
    {
      HashSet<Guid> releaseDefinition1 = new HashSet<Guid>();
      if (releaseDefinition?.Environments == null)
        return releaseDefinition1;
      IEnumerable<WorkflowTask> workflowTasks = releaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (x => x != null)).SelectMany<DefinitionEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>((Func<DefinitionEnvironment, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>>) (x => (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>) x.DeployPhases)).Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase, bool>) (x => x != null)).SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase, WorkflowTask>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase, IEnumerable<WorkflowTask>>) (x => (IEnumerable<WorkflowTask>) ReleaseDefinitionsSecureFileValidations.DeserializeWorkflowTasks(requestContext, x.Workflow)));
      if (workflowTasks != null)
      {
        foreach (WorkflowTask workflowTask in workflowTasks)
        {
          string input;
          Guid result;
          if (workflowTask.Inputs.TryGetValue("secureFile", out input) && Guid.TryParse(input, out result))
            releaseDefinition1.Add(result);
        }
      }
      return releaseDefinition1;
    }

    private static IReadOnlyList<WorkflowTask> DeserializeWorkflowTasks(
      IVssRequestContext requestContext,
      string workflow)
    {
      try
      {
        if (workflow != null)
        {
          List<WorkflowTask> workflowTaskList = JsonConvert.DeserializeObject<List<WorkflowTask>>(workflow);
          if (workflowTaskList != null)
            return (IReadOnlyList<WorkflowTask>) workflowTaskList;
        }
      }
      catch (JsonException ex)
      {
        requestContext.TraceException(1961233, "ReleaseManagementService", "Service", (Exception) ex);
      }
      return (IReadOnlyList<WorkflowTask>) new List<WorkflowTask>();
    }

    private static void ValidateSecureFiles(
      IEnumerable<Guid> secureFiles,
      HashSet<Guid> permittedSecureFiles)
    {
      foreach (Guid secureFile in secureFiles)
      {
        if (!permittedSecureFiles.Contains(secureFile))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UserNotHavingUsePermissionOnSecureFile, (object) secureFile));
      }
    }
  }
}
