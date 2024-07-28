// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWiqlConverterService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWiqlConverterService : IWiqlConverterRemotableService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public string WiqlConvertNameToIds(
      IVssRequestContext requestContext,
      Guid projectId,
      string wiql)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Parser.ParseSyntax(wiql);
      requestContext.GetService<IWorkItemQueryService>().ValidateWiql(requestContext, wiql, projectId);
      return WiqlTransformUtils.TransformNamesToIds(requestContext, wiql, true);
    }

    public string WiqlConvertIdsToName(
      IVssRequestContext requestContext,
      Guid projectId,
      string wiql)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Parser.ParseSyntax(wiql);
      requestContext.GetService<IWorkItemQueryService>().ValidateWiql(requestContext, wiql, projectId);
      Query query = new Query() { Wiql = wiql };
      WiqlIdToNameTransformer.Transform(requestContext, (Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) query);
      return query.Wiql;
    }
  }
}
