// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GatedBuildDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class GatedBuildDefinitionExtensions
  {
    internal static void ConvertGatedTriggerPathFilters(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      if (definition == null)
        return;
      BuildRepository expandedRepository = requestContext.GetService<IBuildDefinitionService>().GetExpandedRepository(requestContext, definition);
      foreach (BuildTrigger trigger in definition.Triggers)
      {
        if (trigger is GatedCheckInTrigger gatedCheckInTrigger && gatedCheckInTrigger.UseWorkspaceMappings)
        {
          gatedCheckInTrigger.PathFilters.Clear();
          foreach (MappingDetails mapping in GatedBuildDefinitionExtensions.GetWorkspaceMapping(requestContext, expandedRepository).Mappings)
          {
            string empty = string.Empty;
            string str = (!string.Equals(mapping.MappingType, "map", StringComparison.InvariantCultureIgnoreCase) ? "-" : "+") + mapping.ServerPath;
            gatedCheckInTrigger.PathFilters.Add(str);
          }
        }
      }
    }

    private static BuildWorkspace GetWorkspaceMapping(
      IVssRequestContext requestContext,
      BuildRepository repository)
    {
      string toDeserialize;
      if (repository.Properties.TryGetValue("tfvcMapping", out toDeserialize))
      {
        if (!string.IsNullOrEmpty(toDeserialize))
        {
          try
          {
            return JsonUtility.FromString<BuildWorkspace>(toDeserialize);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, TraceLevel.Warning, "Build2", "GatedCheckInEvaluator", ex);
          }
        }
      }
      requestContext.Trace(0, TraceLevel.Warning, "Build2", "GatedCheckInEvaluator", "Workspace Mapping is not found. Create default mapping that map tfvc root.");
      return new BuildWorkspace()
      {
        Mappings = {
          new MappingDetails()
          {
            ServerPath = repository.RootFolder,
            MappingType = "map"
          }
        }
      };
    }
  }
}
