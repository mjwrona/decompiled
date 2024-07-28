// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class ProcessViewModel
  {
    public ProcessViewModel(TfsWebContext webContext)
      : this(webContext, webContext.TfsRequestContext.ServiceHost)
    {
    }

    public ProcessViewModel(TfsWebContext webContext, IVssServiceHost collectionServiceHost)
    {
      this.CollectionId = collectionServiceHost.InstanceId;
      this.DisplayName = collectionServiceHost.Name;
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      this.CanCreateProcesses = tfsRequestContext.GetService<ITeamFoundationProcessService>().HasRootCreatePermission(tfsRequestContext);
      this.ControlContributionInputLimit = tfsRequestContext.WitContext().ServerSettings.ControlContributionInputLimit;
      this.MaxPicklistItemsPerList = tfsRequestContext.WitContext().TemplateValidatorConfiguration.MaxPickListItemsPerList;
    }

    public Guid CollectionId { get; private set; }

    public string DisplayName { get; private set; }

    public bool CanCreateProjects { get; internal set; }

    public bool CanCreateProcesses { get; internal set; }

    public int ControlContributionInputLimit { get; private set; }

    public int MaxPicklistItemsPerList { get; private set; }
  }
}
