// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.ForksExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class ForksExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Forks ToWebApiForks(
      this Microsoft.TeamFoundation.Build2.Server.Forks srvForks,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvForks == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Forks) null;
      return new Microsoft.TeamFoundation.Build.WebApi.Forks(securedObject)
      {
        Enabled = srvForks.Enabled,
        AllowSecrets = srvForks.AllowSecrets,
        AllowFullAccessToken = srvForks.AllowFullAccessToken
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.Forks ToServerForks(this Microsoft.TeamFoundation.Build.WebApi.Forks webApiForks)
    {
      if (webApiForks == null)
        return (Microsoft.TeamFoundation.Build2.Server.Forks) null;
      return new Microsoft.TeamFoundation.Build2.Server.Forks()
      {
        Enabled = webApiForks.Enabled,
        AllowSecrets = webApiForks.AllowSecrets,
        AllowFullAccessToken = webApiForks.AllowFullAccessToken
      };
    }
  }
}
