// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.DefinitionReferenceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  public static class DefinitionReferenceExtensions
  {
    public static string GetWebUrl(
      this DefinitionReference definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<DefinitionReference>(definition, nameof (definition));
      return definition.Project != null ? requestContext.GetService<IBuildRouteService>().GetDefinitionWebUrl(requestContext, definition.Project.Id, definition.Id) : (string) null;
    }

    public static string GetRestUrl(
      this DefinitionReference definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<DefinitionReference>(definition, nameof (definition));
      return definition.Project != null ? requestContext.GetService<IBuildRouteService>().GetDefinitionRestUrl(requestContext, definition.Project.Id, definition.Id, definition.Revision) : (string) null;
    }
  }
}
