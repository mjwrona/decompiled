// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcShelvesetWorkItemsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("shelvesets")]
  public class TfvcShelvesetWorkItemsController : TfvcApiController
  {
    [HttpGet]
    [ClientExample("GET__tfvc_shelvesetWorkItems__shelvesetId.json", "GET a list of workitems for a ShelvesetId.", null, null)]
    [ClientLocationId("A7A0C1C1-373E-425A-B031-A519474D743D")]
    public IEnumerable<AssociatedWorkItem> GetShelvesetWorkItems(string shelvesetId)
    {
      if (shelvesetId == null)
        throw new InvalidArgumentValueException(nameof (shelvesetId));
      string name = (string) null;
      string owner = (string) null;
      TfvcShelvesetUtility.ParseNameId(this.TfsRequestContext, shelvesetId, out name, out owner);
      return TfvcShelvesetUtility.GetShelveset(this.TfsRequestContext, name, owner).GetWorkItems(this.TfsRequestContext);
    }
  }
}
