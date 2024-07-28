// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.IWorkItemSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [DefaultServiceImplementation(typeof (WorkItemSecurityChecksService))]
  public interface IWorkItemSecurityChecksService : ISecurityChecksService, IVssFrameworkService
  {
    IEnumerable<ClassificationNode> GetUserAccessibleAreas(
      IVssRequestContext requestContext,
      out bool allAreasAreAccessible);

    void ScrubEmailsFromIdentityFieldsForAnonymousPublicUsers(
      IVssRequestContext requestContext,
      WorkItemSearchResponse response);
  }
}
