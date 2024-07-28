// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.UserUrlHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Users;
using System;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class UserUrlHelper
  {
    private const string Area = "User";
    private const string Layer = "UserUrlHelper";

    public static string GetAvatarUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return UserUrlHelper.GetLocationDataProvider(vssRequestContext).GetResourceUri(vssRequestContext, "User", UserResourceIds.Avatar, (object) new
        {
          descriptor = subjectDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008625, "User", nameof (UserUrlHelper), ex);
        return string.Empty;
      }
    }

    private static ILocationDataProvider GetLocationDataProvider(
      IVssRequestContext deploymentContext)
    {
      int num = deploymentContext.GetService<IVssRegistryService>().GetValue<int>(deploymentContext, (RegistryQuery) "/Configuration/User/UrlHelper/LocationDataTimeoutInMilliseconds", 3000);
      using (deploymentContext.CreateAsyncTimeOutScope(TimeSpan.FromMilliseconds((double) num)))
        return deploymentContext.GetService<ILocationService>().GetLocationData(deploymentContext, UserResourceIds.AreaIdGuid);
    }

    private static class RegistryKeys
    {
      private const string @base = "/Configuration/User/UrlHelper/";
      public const string LocationDataTimeoutInMilliseconds = "/Configuration/User/UrlHelper/LocationDataTimeoutInMilliseconds";
    }

    private static class Defaults
    {
      public const int LocationDataTimeoutInMilliseconds = 3000;
    }
  }
}
