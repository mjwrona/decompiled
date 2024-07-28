// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.RecycleBin2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.ActionFilters;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "recyclebin", ResourceVersion = 2)]
  [ControllerApiVersion(4.1)]
  [WitRestCiFilter]
  public class RecycleBin2Controller : RecycleBinCommonController
  {
    [HttpGet]
    [ClientLocationId("B70D8D39-926C-465E-B927-B1BF0E5CA0E0")]
    [ClientExample("GET__wit_recyclebin.json", null, null, null)]
    public IEnumerable<WorkItemDeleteShallowReference> GetDeletedWorkItemShallowReferences()
    {
      IEnumerable<WorkItemDeleteShallowReference> source = this.GetWorkItemIdsFromRecycleBin().Select<int, WorkItemDeleteShallowReference>((Func<int, WorkItemDeleteShallowReference>) (id => WorkItemDeleteShallowReferenceFactory.Create(this.WitRequestContext, id, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext))));
      return source == null ? (IEnumerable<WorkItemDeleteShallowReference>) null : (IEnumerable<WorkItemDeleteShallowReference>) source.ToList<WorkItemDeleteShallowReference>();
    }
  }
}
