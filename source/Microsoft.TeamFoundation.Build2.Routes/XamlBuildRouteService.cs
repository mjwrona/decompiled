// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.XamlBuildRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  public sealed class XamlBuildRouteService : 
    BaseRouteService,
    IXamlBuildRouteService,
    IVssFrameworkService
  {
    public string GetControllerRestUrl(IVssRequestContext requestContext, int controllerId) => this.GetResourceUrl(requestContext, "build", BuildResourceIds.Queues, (object) new
    {
      controllerId = controllerId
    });

    public string GetInformationNodesRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.InformationNodes, projectId, (object) new
      {
        buildId = buildId
      }, (Func<Uri, string>) (uri => uri.AppendQuery("api-version", "1.0").AbsoluteUri));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
