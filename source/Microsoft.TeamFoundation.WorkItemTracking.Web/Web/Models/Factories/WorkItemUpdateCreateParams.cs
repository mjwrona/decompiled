// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemUpdateCreateParams
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal class WorkItemUpdateCreateParams
  {
    private WorkItemTrackingRequestContext witRequestContext;
    private WorkItem workItem;
    private bool includeLinks;
    private bool returnIdentityRef;
    private bool returnProjectScopedUrl = true;
    private bool includeUrls = true;
    private bool returnAuthorizedAs;

    internal WorkItemTrackingRequestContext WitRequestContext
    {
      get => this.witRequestContext;
      set => this.witRequestContext = value;
    }

    internal WorkItem WorkItem
    {
      get => this.workItem;
      set => this.workItem = value;
    }

    internal bool IncludeLinks
    {
      get => this.includeLinks;
      set => this.includeLinks = value;
    }

    internal bool ReturnIdentityRef
    {
      get => this.returnIdentityRef;
      set => this.returnIdentityRef = value;
    }

    internal bool ReturnProjectScopedUrl
    {
      get => this.returnProjectScopedUrl;
      set => this.returnProjectScopedUrl = value;
    }

    internal bool IncludeUrls
    {
      get => this.includeUrls;
      set => this.includeUrls = value;
    }

    internal bool ReturnAuthorizedAs
    {
      get => this.returnAuthorizedAs;
      set => this.returnAuthorizedAs = value;
    }

    public ApiResourceVersion ApiResourceVersion { get; set; }
  }
}
