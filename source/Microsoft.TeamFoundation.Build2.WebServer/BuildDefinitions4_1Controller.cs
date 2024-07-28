// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitions4_1Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "definitions", ResourceVersion = 6)]
  public class BuildDefinitions4_1Controller : BuildDefinitions4Controller
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public override BuildDefinition GetDefinition(
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      bool includeLatestBuilds = false)
    {
      return base.GetDefinition(definitionId, revision, minMetricsTime, propertyFilters, includeLatestBuilds);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<BuildDefinitionReference>), null, null)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.Protected)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetDefinitions(
      string name = "*",
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      [FromUri(Name = "$top")] int top = 10000,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool includeAllProperties = false,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null)
    {
      return base.GetDefinitions(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTime, definitionIds, path, builtAfter, notBuiltAfter, includeAllProperties, includeLatestBuilds, taskIdFilter);
    }
  }
}
