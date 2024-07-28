// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.DefinitionTriggerTypeExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class DefinitionTriggerTypeExtensions
  {
    public static Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType ToBuildServerDefinitionTriggerType(
      this Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType webApiTriggerType)
    {
      switch (webApiTriggerType)
      {
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.None:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.None;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.ContinuousIntegration:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.BatchedContinuousIntegration:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.BatchedContinuousIntegration;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.Schedule:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.Schedule;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.GatedCheckIn:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.GatedCheckIn;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.BatchedGatedCheckIn:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.BatchedGatedCheckIn;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.PullRequest:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest;
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.All:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.All;
        default:
          return Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.None;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType ToWebApiDefinitionTriggerType(
      this Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType srvTriggerType)
    {
      switch (srvTriggerType)
      {
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.None:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.None;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.ContinuousIntegration;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.BatchedContinuousIntegration:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.BatchedContinuousIntegration;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.Schedule:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.Schedule;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.GatedCheckIn:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.GatedCheckIn;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.BatchedGatedCheckIn:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.BatchedGatedCheckIn;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.PullRequest;
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.All:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.All;
        default:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.None;
      }
    }
  }
}
