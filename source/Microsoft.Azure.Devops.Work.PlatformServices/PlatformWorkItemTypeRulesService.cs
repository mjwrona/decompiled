// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemTypeRulesService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemTypeRulesService : 
    IWorkItemTypeRulesRemotableService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ProcessRule> GetProcessRulesForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      IEnumerable<WorkItemFieldRule> rulesForWorkItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetAllRulesForWorkItemType(requestContext, processId, workItemTypeReferenceName, true);
      return (IEnumerable<ProcessRule>) ProcessRulesModelFactory.CreateIdToProcessRuleDictionary(requestContext, processId, workItemTypeReferenceName, rulesForWorkItemType).Values;
    }
  }
}
