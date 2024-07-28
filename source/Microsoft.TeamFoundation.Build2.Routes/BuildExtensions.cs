// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.BuildExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  public static class BuildExtensions
  {
    public static string GetWebUrl(this Microsoft.TeamFoundation.Build.WebApi.Build build, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      if (build.Project == null)
        return (string) null;
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid id1 = build.Project.Id;
      int id2 = build.Id;
      DefinitionReference definition = build.Definition;
      int num = definition != null ? (definition.Type == DefinitionType.Xaml ? 1 : 0) : 0;
      return service.GetBuildWebUrl(requestContext1, id1, id2, num != 0);
    }

    public static string GetRestUrl(this Microsoft.TeamFoundation.Build.WebApi.Build build, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      return build.Project != null ? requestContext.GetService<IBuildRouteService>().GetBuildRestUrl(requestContext, build.Project.Id, build.Id) : (string) null;
    }
  }
}
