// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.RepositoryInformationHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class RepositoryInformationHelper
  {
    private const string c_layer = "RepositoryInformationHelper";
    private const string c_definitionProperty_lastRepositoryUpdate = "LastRepositoryUpdate";

    private static bool ShouldUpdateRepositoryInformation(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      definition.PopulateProperties(requestContext, (IEnumerable<string>) new string[1]
      {
        "LastRepositoryUpdate"
      });
      DateTime dateTime;
      if (definition.Properties.TryGetValue<DateTime>("LastRepositoryUpdate", out dateTime))
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Pipelines/UpdateRepositoryInformation/IntervalSeconds", (int) TimeSpan.FromDays(1.0).TotalSeconds);
        if (dateTime >= DateTime.UtcNow.AddSeconds((double) -num))
        {
          requestContext.TraceInfo(TracePoints.Events.TryUpdateRepositoryInformation, nameof (RepositoryInformationHelper), "Skipping update for {0} repository {1}. lastRepositoryUpdate={2}, intervalInSeconds={3}", (object) definition.Repository?.Type, (object) definition.Repository?.Id, (object) dateTime, (object) num);
          return false;
        }
      }
      return true;
    }

    public static bool TryUpdateRepositoryInformation(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(definition, nameof (definition));
      if (!RepositoryInformationHelper.ShouldUpdateRepositoryInformation(requestContext, definition))
        return false;
      try
      {
        if (definition.Repository == null)
        {
          string message = string.Format("Unable to find the correct definition for the repository. projectId={0}, definitionId={1}", (object) definition.ProjectId, (object) definition.Id);
          requestContext.TraceError(TracePoints.Events.TryUpdateRepositoryInformation, nameof (RepositoryInformationHelper), message);
          return false;
        }
        if (!RepositoryInformationHelper.TryPublishRepositoryTelemetry(requestContext, definition))
          return false;
        IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
        PropertiesCollection propertiesCollection = new PropertiesCollection()
        {
          ["LastRepositoryUpdate"] = (object) DateTime.UtcNow
        };
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = definition.ProjectId;
        int id = definition.Id;
        PropertiesCollection properties = propertiesCollection;
        service.UpdateProperties(requestContext1, projectId, id, properties);
        requestContext.TraceInfo(TracePoints.Events.TryUpdateRepositoryInformation, nameof (RepositoryInformationHelper), "Successfully updated CI info for {0} repository {1}.", (object) definition.Repository.Type, (object) definition.Repository.Id);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TracePoints.Events.TryUpdateRepositoryInformation, TracePoints.Area, nameof (RepositoryInformationHelper), ex);
        return false;
      }
    }

    private static bool TryPublishRepositoryTelemetry(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      try
      {
        if (definition.Process.Type == 4)
        {
          requestContext.TraceInfo(TracePoints.Events.TryUpdateRepositoryInformation, nameof (RepositoryInformationHelper), "Not publishing repo information for JIT definition {0} in project {1}.", (object) definition.Id, (object) definition.ProjectId);
          return false;
        }
        Guid serviceEndpointId;
        definition.Repository.TryGetServiceEndpointId(out serviceEndpointId);
        Microsoft.TeamFoundation.Build2.Server.SourceRepository userRepository = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type).GetUserRepository(requestContext, definition.ProjectId, new Guid?(serviceEndpointId), definition.Repository.Id, true);
        if (userRepository == null)
        {
          requestContext.TraceWarning(TracePoints.Events.TryUpdateRepositoryInformation, nameof (RepositoryInformationHelper), "Unable to retrieve {0} repository '{1}' from projectId {2} with service endpoint {3}", (object) definition.Repository.Type, (object) definition.Repository.Id, (object) definition.ProjectId, (object) serviceEndpointId);
          return false;
        }
        new CustomerIntelligenceData().AddSourceRepositoryData(requestContext, userRepository).PublishCI(requestContext, "Repository");
        return true;
      }
      catch (MissingEndpointInformationException ex)
      {
        requestContext.TraceInfo(TracePoints.Events.TryUpdateRepositoryInformation, nameof (RepositoryInformationHelper), ex.Message);
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TracePoints.Events.TryUpdateRepositoryInformation, TracePoints.Area, nameof (RepositoryInformationHelper), ex);
        return false;
      }
    }
  }
}
