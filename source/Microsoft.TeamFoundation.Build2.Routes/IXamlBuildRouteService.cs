// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.IXamlBuildRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  [DefaultServiceImplementation(typeof (XamlBuildRouteService))]
  public interface IXamlBuildRouteService : IVssFrameworkService
  {
    string GetControllerRestUrl(IVssRequestContext requestContext, int controllerId);

    string GetInformationNodesRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId);
  }
}
