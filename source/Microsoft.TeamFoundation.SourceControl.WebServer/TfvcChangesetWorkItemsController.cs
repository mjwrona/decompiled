// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesetWorkItemsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("changesets")]
  public class TfvcChangesetWorkItemsController : TfvcApiController
  {
    [ClientExample("GET__tfvc_changesetWorkItems.json", "GET the work items associated with a changeset", null, null)]
    [HttpGet]
    public IEnumerable<AssociatedWorkItem> GetChangesetWorkItems([ClientParameterType(typeof (int), false)] string id = null) => TfvcChangesetUtility.GetChangesetWorkItems(this.TfsRequestContext, this.Url, VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, id));
  }
}
