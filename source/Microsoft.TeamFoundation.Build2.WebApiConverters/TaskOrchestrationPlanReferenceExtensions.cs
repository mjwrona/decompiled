// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.TaskOrchestrationPlanReferenceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class TaskOrchestrationPlanReferenceExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference ToWebApiTaskOrchestrationPlanReference(
      this Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference srvTaskOrchPlanRef,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvTaskOrchPlanRef == null)
        return (Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference) null;
      return new Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference(securedObject)
      {
        PlanId = srvTaskOrchPlanRef.PlanId,
        OrchestrationType = srvTaskOrchPlanRef.OrchestrationType
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference ToServerTaskOrchPlanRef(
      this Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference webApiTaskOrchPlanRef)
    {
      if (webApiTaskOrchPlanRef == null)
        return (Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference) null;
      return new Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference()
      {
        PlanId = webApiTaskOrchPlanRef.PlanId,
        OrchestrationType = webApiTaskOrchPlanRef.OrchestrationType
      };
    }
  }
}
