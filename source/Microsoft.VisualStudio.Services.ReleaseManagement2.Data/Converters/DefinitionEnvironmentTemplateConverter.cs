// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.DefinitionEnvironmentTemplateConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class DefinitionEnvironmentTemplateConverter
  {
    public static DefinitionEnvironmentTemplate FromWebApi(
      this ReleaseDefinitionEnvironmentTemplate webApiTemplate)
    {
      if (webApiTemplate == null)
        throw new ArgumentNullException(nameof (webApiTemplate));
      DefinitionEnvironmentTemplateConverter.TemplatizeEnvironment(webApiTemplate.Environment);
      return new DefinitionEnvironmentTemplate()
      {
        Id = webApiTemplate.Id,
        Name = webApiTemplate.Name,
        Description = webApiTemplate.Description,
        EnvironmentJson = JsonConvert.SerializeObject((object) webApiTemplate.Environment)
      };
    }

    private static void TemplatizeEnvironment(ReleaseDefinitionEnvironment environment)
    {
      environment.Id = 0;
      environment.Name = string.Empty;
      environment.Rank = 1;
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> keyValuePair in environment.Variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, bool>) (p => p.Value.IsSecret)))
      {
        keyValuePair.Value.IsSecret = false;
        keyValuePair.Value.Value = string.Empty;
      }
      DefinitionEnvironmentTemplateConverter.ResetIds(environment);
    }

    private static void ResetIds(ReleaseDefinitionEnvironment environment)
    {
      environment.DeployStep.Id = 0;
      List<ReleaseDefinitionApprovalStep> definitionApprovalStepList = new List<ReleaseDefinitionApprovalStep>();
      definitionApprovalStepList.AddRange((IEnumerable<ReleaseDefinitionApprovalStep>) ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(environment, EnvironmentStepType.PreDeploy));
      definitionApprovalStepList.AddRange((IEnumerable<ReleaseDefinitionApprovalStep>) ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(environment, EnvironmentStepType.PostDeploy));
      foreach (ReleaseDefinitionEnvironmentStep definitionEnvironmentStep in definitionApprovalStepList)
        definitionEnvironmentStep.Id = 0;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>) environment.Schedules)
        schedule.JobId = Guid.Empty;
    }

    public static ReleaseDefinitionEnvironmentTemplate ToWebApi(
      this DefinitionEnvironmentTemplate serverTemplate)
    {
      if (serverTemplate == null)
        throw new ArgumentNullException(nameof (serverTemplate));
      ReleaseDefinitionEnvironment definitionEnvironment = serverTemplate.EnvironmentJson != null ? JsonConvert.DeserializeObject<ReleaseDefinitionEnvironment>(serverTemplate.EnvironmentJson) : (ReleaseDefinitionEnvironment) null;
      return new ReleaseDefinitionEnvironmentTemplate()
      {
        Id = serverTemplate.Id,
        Name = serverTemplate.Name,
        Description = serverTemplate.Description,
        Environment = definitionEnvironment,
        CanDelete = true,
        Category = "Custom"
      };
    }
  }
}
