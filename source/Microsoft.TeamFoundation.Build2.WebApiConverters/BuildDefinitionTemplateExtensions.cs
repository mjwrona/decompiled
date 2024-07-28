// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildDefinitionTemplateExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildDefinitionTemplateExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate ToWebApiBuildDefinitionTemplate(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate srvDefinitionTemplate,
      IVssRequestContext requestContext,
      Version apiVersion,
      IdentityMap identityMap = null,
      AgentPoolQueueCache queueCache = null,
      BuildRepositoryCache repositoryCache = null)
    {
      if (srvDefinitionTemplate == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate definitionTemplate = new Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate()
      {
        Id = srvDefinitionTemplate.Id,
        Name = srvDefinitionTemplate.Name,
        CanDelete = srvDefinitionTemplate.CanDelete,
        Category = srvDefinitionTemplate.Category,
        DefaultHostedQueue = srvDefinitionTemplate.DefaultHostedQueue,
        IconTaskId = srvDefinitionTemplate.IconTaskId,
        Description = srvDefinitionTemplate.Description,
        Icons = srvDefinitionTemplate.Icons
      };
      if (srvDefinitionTemplate.Template != null)
        definitionTemplate.Template = srvDefinitionTemplate.Template.ToWebApiBuildDefinition(requestContext, apiVersion, identityMap, queueCache, repositoryCache);
      return definitionTemplate;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate ToBuild2ServerBuildDefinitionTemplate(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate webApiDefinitionTemplate)
    {
      if (webApiDefinitionTemplate == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate) null;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate definitionTemplate = new Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate()
      {
        Id = webApiDefinitionTemplate.Id,
        Name = webApiDefinitionTemplate.Name,
        CanDelete = webApiDefinitionTemplate.CanDelete,
        Category = webApiDefinitionTemplate.Category,
        DefaultHostedQueue = webApiDefinitionTemplate.DefaultHostedQueue,
        IconTaskId = webApiDefinitionTemplate.IconTaskId,
        Description = webApiDefinitionTemplate.Description,
        Icons = webApiDefinitionTemplate.Icons
      };
      if (webApiDefinitionTemplate.Template != null)
        definitionTemplate.Template = webApiDefinitionTemplate.Template.ToBuildServerBuildDefinition();
      return definitionTemplate;
    }

    public static BuildDefinitionTemplate3_2 ToBuildDefinitionTemplate3_2(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate source)
    {
      if (source == null)
        return (BuildDefinitionTemplate3_2) null;
      BuildDefinitionTemplate3_2 definitionTemplate32 = new BuildDefinitionTemplate3_2()
      {
        CanDelete = source.CanDelete,
        Category = source.Category,
        Description = source.Description,
        IconTaskId = source.IconTaskId,
        Id = source.Id,
        Name = source.Name,
        Template = source.Template.ToBuildDefinition3_2()
      };
      if (source.Icons.Count > 0)
      {
        foreach (KeyValuePair<string, string> icon in (IEnumerable<KeyValuePair<string, string>>) source.Icons)
          definitionTemplate32.Icons.Add(icon);
      }
      return definitionTemplate32;
    }
  }
}
