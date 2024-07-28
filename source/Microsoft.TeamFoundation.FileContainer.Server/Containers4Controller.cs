// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Containers4Controller
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [VersionedApiControllerCustomName(Area = "Container", ResourceName = "Containers", ResourceVersion = 4)]
  public class Containers4Controller : ContainersController
  {
    [HttpGet]
    [TraceFilter(1008000, 1008010)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public IQueryable<Microsoft.VisualStudio.Services.FileContainer.FileContainer> GetContainers(
      Guid scope,
      string artifactUris = null)
    {
      return this.GetContainers(new Guid?(scope), artifactUris);
    }

    [HttpGet]
    [TraceFilter(1008021, 1008030)]
    [ClientResponseType(typeof (List<FileContainerItem>), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public HttpResponseMessage GetItems(
      long containerId,
      Guid scope,
      [ClientQueryParameter] string itemPath = null,
      bool metadata = false,
      [FromUri(Name = "$format")] string format = null,
      string downloadFileName = null,
      bool includeDownloadTickets = false,
      bool isShallow = false,
      bool ignoreRequestedMediaType = false,
      bool includeBlobMetadata = false,
      bool saveAbsolutePath = true,
      bool preferRedirect = false)
    {
      return this.GetItems(containerId, new Guid?(scope), itemPath, metadata, downloadFileName, includeDownloadTickets, includeBlobMetadata, isShallow, ignoreRequestedMediaType: ignoreRequestedMediaType, saveAbsolutePath: saveAbsolutePath, preferRedirect: preferRedirect);
    }

    [HttpPut]
    [TraceFilter(1008011, 1008020)]
    [ClientRequestBodyIsStream]
    [ClientResponseType(typeof (FileContainerItem), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Java | RestClientLanguages.TypeScript)]
    public HttpResponseMessage CreateItem(
      int containerId,
      [ClientQueryParameter] string itemPath,
      Guid scope,
      [ClientQueryParameter] string artifactHash = null,
      [ClientQueryParameter] long? fileLength = null)
    {
      return artifactHash != null ? this.CreateItemFromArtifactUpload(containerId, itemPath, new Guid?(scope), artifactHash, fileLength) : this.CreateItem(containerId, itemPath, new Guid?(scope));
    }

    [HttpDelete]
    [TraceFilter(1008131, 1008140)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public HttpResponseMessage DeleteItem(long containerId, [ClientQueryParameter] string itemPath, Guid scope) => this.DeleteItem(containerId, itemPath, new Guid?(scope));

    [HttpPost]
    [TraceFilter(1008141, 1008150)]
    [ClientResponseType(typeof (List<FileContainerItem>), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public HttpResponseMessage CreateItems(
      int containerId,
      VssJsonCollectionWrapper<List<FileContainerItem>> items,
      Guid scope)
    {
      return this.CreateItems(containerId, items, new Guid?(scope));
    }
  }
}
