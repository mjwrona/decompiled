// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.IWiqlRemotableService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  [DefaultServiceImplementation("Microsoft.Azure.Devops.Work.PlatformServices.PlatformWiqlService, Microsoft.Azure.Devops.Work.PlatformServices")]
  public interface IWiqlRemotableService : IVssFrameworkService
  {
    WorkItemQueryResult QueryByWiql(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextLimitValidation = false);

    WorkItemQueryResult QueryByWiql(
      IVssRequestContext requestContext,
      string wiql,
      string projectName,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextLimitValidation = false);
  }
}
