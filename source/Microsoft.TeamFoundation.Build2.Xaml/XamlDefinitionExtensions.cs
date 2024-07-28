// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public static class XamlDefinitionExtensions
  {
    public static DefinitionReference ToReference(this XamlBuildDefinition definition) => new DefinitionReference()
    {
      Id = definition.Id,
      Name = definition.Name,
      Project = definition.Project,
      QueueStatus = definition.QueueStatus,
      Revision = definition.Revision,
      Type = definition.Type,
      Uri = definition.Uri,
      Url = definition.Url,
      CreatedDate = definition.CreatedDate
    };

    public static DefinitionReference ToReference(
      this Microsoft.TeamFoundation.Build.Server.BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      int definitionId = int.Parse(LinkingUtilities.DecodeUri(definition.Uri).ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture);
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      BuildDefinitionReference reference = new BuildDefinitionReference();
      reference.Id = definitionId;
      reference.Type = Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Xaml;
      reference.Name = definition.Name;
      reference.Url = service.GetDefinitionRestUrl(requestContext, definition.TeamProject.Id, definitionId);
      reference.CreatedDate = definition.DateCreated;
      return (DefinitionReference) reference;
    }

    public static void AddLinks(
      this XamlBuildDefinition xamlDefinition,
      IVssRequestContext requestContext)
    {
      if (xamlDefinition == null || string.IsNullOrEmpty(xamlDefinition.Url))
        return;
      xamlDefinition.Links.TryAddLink("self", (ISecuredObject) xamlDefinition, xamlDefinition.Url);
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus ToQueueStatus(
      this Microsoft.TeamFoundation.Build.Server.DefinitionQueueStatus status)
    {
      switch (status)
      {
        case Microsoft.TeamFoundation.Build.Server.DefinitionQueueStatus.Enabled:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus.Enabled;
        case Microsoft.TeamFoundation.Build.Server.DefinitionQueueStatus.Paused:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus.Paused;
        default:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus.Disabled;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType ToTriggerType(
      this Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType triggerType)
    {
      switch (triggerType)
      {
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.ContinuousIntegration:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.ContinuousIntegration;
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.BatchedContinuousIntegration:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.BatchedContinuousIntegration;
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.Schedule:
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.ScheduleForced:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.Schedule;
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.GatedCheckIn:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.GatedCheckIn;
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.BatchedGatedCheckIn:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.BatchedGatedCheckIn;
        case Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType.All:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.All;
        default:
          return Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.None;
      }
    }
  }
}
