// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ManualInterventionConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ManualInterventionConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention manualIntervention,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (manualIntervention == null)
        throw new ArgumentNullException(nameof (manualIntervention));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention webApi = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention();
      webApi.Id = manualIntervention.Id;
      webApi.Status = (ManualInterventionStatus) manualIntervention.Status;
      webApi.Comments = manualIntervention.Comments;
      webApi.CreatedOn = manualIntervention.CreatedOn;
      webApi.ModifiedOn = manualIntervention.ModifiedOn;
      webApi.ReleaseReference = new ReleaseShallowReference()
      {
        Id = manualIntervention.ReleaseId,
        Name = manualIntervention.ReleaseName
      };
      webApi.ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
      {
        Id = manualIntervention.ReleaseDefinitionId,
        Name = manualIntervention.ReleaseDefinitionName,
        Path = manualIntervention.ReleaseDefinitionPath
      };
      webApi.ReleaseEnvironmentReference = new ReleaseEnvironmentShallowReference()
      {
        Id = manualIntervention.ReleaseEnvironmentId,
        Name = manualIntervention.ReleaseEnvironmentName
      };
      webApi.TaskInstanceId = manualIntervention.TaskActivityData.TaskInstanceId;
      webApi.Instructions = manualIntervention.Instructions;
      webApi.Name = manualIntervention.TaskActivityData.TaskName;
      webApi.ReleaseReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectId.ToString(), manualIntervention.ReleaseId));
      webApi.ReleaseReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseRestUrl(requestContext, projectId, manualIntervention.ReleaseId));
      webApi.ReleaseDefinitionReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(requestContext, projectId.ToString(), manualIntervention.ReleaseDefinitionId));
      webApi.ReleaseDefinitionReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(requestContext, projectId, manualIntervention.ReleaseDefinitionId));
      webApi.ReleaseEnvironmentReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseEnvironmentRestUri(requestContext, projectId, manualIntervention.ReleaseId, manualIntervention.ReleaseEnvironmentId));
      IdentityRef identityRef;
      if (!(manualIntervention.Approver == Guid.Empty))
        identityRef = new IdentityRef()
        {
          Id = manualIntervention.Approver.ToString()
        };
      else
        identityRef = (IdentityRef) null;
      webApi.Approver = identityRef;
      return webApi;
    }
  }
}
