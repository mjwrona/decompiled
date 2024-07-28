// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcShelvesetChangesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("shelvesets")]
  public class TfvcShelvesetChangesController : TfvcApiController
  {
    private static readonly string[] s_defaultPropertyFilters = new string[1]
    {
      "Microsoft.TeamFoundation.VersionControl.SymbolicLink"
    };

    [HttpGet]
    [ClientExample("GET__tfvc_shelvesetChanges__shelvesetId.json", "GET a list of changesets for a ShelvesetId.", null, null)]
    [ClientExample("GET__tfvc_shelvesetChanges__shelvesetId__top-_top___skip-_skip.json", "GET a list of changesets for a ShelvesetId with top and skip.", null, null)]
    [ClientLocationId("DBAF075B-0445-4C34-9E5B-82292F856522")]
    public IEnumerable<TfvcChange> GetShelvesetChanges(string shelvesetId, [FromUri(Name = "$top")] int? top = null, [FromUri(Name = "$skip")] int? skip = null)
    {
      if (shelvesetId == null)
        throw new InvalidArgumentValueException(nameof (shelvesetId));
      ref int? local = ref top;
      int? nullable = top;
      int num1 = Math.Max(0, nullable ?? 100);
      local = new int?(num1);
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      IEnumerable<TfvcChange> shelvesetChanges = (IEnumerable<TfvcChange>) new List<TfvcChange>();
      nullable = top;
      int num2 = 0;
      if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
      {
        ShelvesetVersionSpec versionSpecFromId = TfvcShelvesetUtility.GetShelvesetVersionSpecFromId(this.TfsRequestContext, shelvesetId);
        bool allChangesIncluded = false;
        shelvesetChanges = TfsModelExtensions.GetShelvedChanges(this.TfsRequestContext, "$/", RecursionType.Full, 2, versionSpecFromId, top.Value, skip.Value, TfvcShelvesetChangesController.s_defaultPropertyFilters, out allChangesIncluded, true, this.Url);
      }
      return shelvesetChanges;
    }
  }
}
