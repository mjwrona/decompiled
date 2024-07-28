// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class BuildDefinitionExtensions
  {
    public static void ConvertObjectsToProjectGuid(
      this IList<BuildDefinition> definitions,
      IVssRequestContext requestContext)
    {
      List<BuildDefinitionSourceProvider> sourceProviders = new List<BuildDefinitionSourceProvider>();
      foreach (BuildDefinition definition in (IEnumerable<BuildDefinition>) definitions)
      {
        sourceProviders.AddRange((IEnumerable<BuildDefinitionSourceProvider>) definition.SourceProviders);
        definition.ConvertProcessParametersToProjectGuid(requestContext);
      }
      sourceProviders.ConvertToProjectGuid(requestContext);
    }

    public static void ConvertObjectsToProjectName(
      this IList<BuildDefinition> definitions,
      IVssRequestContext requestContext)
    {
      List<BuildDefinitionSourceProvider> sourceProviders = new List<BuildDefinitionSourceProvider>();
      foreach (BuildDefinition definition in (IEnumerable<BuildDefinition>) definitions)
      {
        sourceProviders.AddRange((IEnumerable<BuildDefinitionSourceProvider>) definition.SourceProviders);
        definition.ConvertProcessParametersToProjectName(requestContext);
      }
      sourceProviders.ConvertToProjectName(requestContext);
    }
  }
}
