// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcShelvesetsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "tfvc", ResourceName = "shelvesets", ResourceVersion = 1)]
  public class TfvcShelvesetsController : TfvcApiController
  {
    [HttpGet]
    [ClientExample("GET__tfvc_shelvesets_list.json", "GET a list of shelveset refs.", null, null)]
    [ClientExample("GET__tfvc_shelvesets_list_top-_top___skip-_skip_.json", "GET a list of shelveset refs with top and skip.", null, null)]
    [ClientExample("GET__tfvc_shelvesets_list_maxCommentLength-_maxCommentLength_.json", "GET a list of shelveset refs with a max comment length.", null, null)]
    [ClientExample("GET__tfvc_shelvesets_list_owner-_owner_.json", "GET a list of shelveset refs for a specific Owner.", null, null)]
    [ClientLocationId("E36D44FB-E907-4B0A-B194-F83F1ED32AD3")]
    public IEnumerable<TfvcShelvesetRef> GetShelvesets(
      [ModelBinder] TfvcShelvesetRequestData requestData = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      if (requestData == null)
        requestData = new TfvcShelvesetRequestData();
      TfvcShelvesetRequestData shelvesetRequestData = requestData;
      int? nullable1 = requestData.MaxCommentLength;
      int? nullable2 = new int?(Math.Max(0, nullable1 ?? 80));
      shelvesetRequestData.MaxCommentLength = nullable2;
      ref int? local = ref top;
      nullable1 = top;
      int num = Math.Max(0, nullable1 ?? 100);
      local = new int?(num);
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      if (requestData.Owner != null)
        requestData.Owner = TfsModelExtensions.ParseOwnerId(this.TfsRequestContext, requestData.Owner);
      return TfvcShelvesetUtility.GetShelvesets(this.TfsRequestContext, this.Url, requestData, top.Value, skip.Value);
    }

    [HttpGet]
    [ClientExample("GET__tfvc_shelvesets__shelvesetId_.json", "GET a specific Shelveset.", null, null)]
    [ClientExample("GET__tfvc_shelvesets__shelvesetId__maxChangeCount-100.json", "GET a specific Shelveset with associated changes.", null, null)]
    [ClientExample("GET__tfvc_shelvesets__shelvesetId__includeWorkItems-true.json", "GET a specific Shelveset with associated workitems.", null, null)]
    [ClientExample("GET__tfvc_shelvesets__shelvesetId__includeDetails-true.json", "GET a specific Shelveset with details.", null, null)]
    [ClientExample("GET__tfvc_shelvesets__shelvesetId__maxCommentLength-_maxCommentLength_.json", "GET a specific Shelveset with a comment lenght limit.", null, null)]
    [ClientLocationId("E36D44FB-E907-4B0A-B194-F83F1ED32AD3")]
    public TfvcShelveset GetShelveset(string shelvesetId, [ModelBinder] TfvcShelvesetRequestData requestData = null)
    {
      if (shelvesetId == null)
        throw new InvalidArgumentValueException(nameof (shelvesetId));
      if (requestData == null)
        requestData = new TfvcShelvesetRequestData();
      string name = (string) null;
      string owner = (string) null;
      TfvcShelvesetUtility.ParseNameId(this.TfsRequestContext, shelvesetId, out name, out owner);
      requestData.Name = name;
      requestData.Owner = owner;
      return TfvcShelvesetUtility.GetShelveset(this.TfsRequestContext, this.Url, requestData);
    }
  }
}
