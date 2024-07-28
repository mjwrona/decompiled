// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.DependencyExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class DependencyExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Dependency ToWebApiDependency(
      this Microsoft.TeamFoundation.Build2.Server.Dependency srvDependency,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvDependency == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Dependency) null;
      return new Microsoft.TeamFoundation.Build.WebApi.Dependency(securedObject)
      {
        Scope = srvDependency.Scope,
        Event = srvDependency.Event
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.Dependency ToBuildServerDependency(
      this Microsoft.TeamFoundation.Build.WebApi.Dependency webApiDependency)
    {
      if (webApiDependency == null)
        return (Microsoft.TeamFoundation.Build2.Server.Dependency) null;
      return new Microsoft.TeamFoundation.Build2.Server.Dependency()
      {
        Scope = webApiDependency.Scope,
        Event = webApiDependency.Event
      };
    }
  }
}
