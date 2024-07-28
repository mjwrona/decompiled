// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildStatusCallbackService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildStatusCallbackService : IBuildStatusCallbackService, IVssFrameworkService
  {
    private const string c_drawerKeyFormat = "/Service/Pipelines/Runs/{0}/StatusCallback";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Delete(IVssRequestContext requestContext, IReadOnlyBuildData buildData)
    {
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(buildData, nameof (buildData));
      this.Delete(requestContext, (IEnumerable<IReadOnlyBuildData>) new IReadOnlyBuildData[1]
      {
        buildData
      });
    }

    public void Delete(IVssRequestContext requestContext, IEnumerable<IReadOnlyBuildData> builds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<IReadOnlyBuildData>>(builds, nameof (builds));
      this.Delete(requestContext, builds.Select<IReadOnlyBuildData, int>((Func<IReadOnlyBuildData, int>) (b => b.Id)));
    }

    public void Delete(IVssRequestContext requestContext, IEnumerable<int> buildIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<int>>(buildIds, nameof (buildIds));
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      foreach (int buildId in buildIds)
      {
        Guid drawerId = service.UnlockDrawer(requestContext, BuildStatusCallbackService.GetDrawerName(buildId), false);
        if (drawerId != Guid.Empty)
          service.DeleteDrawer(requestContext, drawerId);
      }
    }

    public void Store(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      StatusCallbackInfo statusCallbackInfo)
    {
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(buildData, nameof (buildData));
      this.Store(requestContext, buildData.Id, statusCallbackInfo);
    }

    public void Store(
      IVssRequestContext requestContext,
      int buildId,
      StatusCallbackInfo statusCallbackInfo)
    {
      ArgumentUtility.CheckForNonPositiveInt(buildId, nameof (buildId));
      ArgumentUtility.CheckForNull<StatusCallbackInfo>(statusCallbackInfo, nameof (statusCallbackInfo));
      ArgumentUtility.CheckStringForNullOrEmpty(statusCallbackInfo.SignatureKey, "SignatureKey");
      ArgumentUtility.CheckForNull<Uri>(statusCallbackInfo.JobStatusUrl, "JobStatusUrl");
      ArgumentUtility.CheckForNull<Uri>(statusCallbackInfo.RunStatusUrl, "RunStatusUrl");
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawer = service.CreateDrawer(requestContext, BuildStatusCallbackService.GetDrawerName(buildId));
      List<Tuple<StrongBoxItemInfo, string>> items = new List<Tuple<StrongBoxItemInfo, string>>()
      {
        Tuple.Create<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
        {
          DrawerId = drawer,
          ItemKind = StrongBoxItemKind.String,
          LookupKey = BuildStatusCallbackService.Keys.SignatureKey
        }, statusCallbackInfo.SignatureKey),
        Tuple.Create<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
        {
          DrawerId = drawer,
          ItemKind = StrongBoxItemKind.String,
          LookupKey = BuildStatusCallbackService.Keys.JobStatusUrl
        }, statusCallbackInfo.JobStatusUrl.IsAbsoluteUri ? statusCallbackInfo.JobStatusUrl.AbsoluteUri : statusCallbackInfo.JobStatusUrl.ToString()),
        Tuple.Create<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
        {
          DrawerId = drawer,
          ItemKind = StrongBoxItemKind.String,
          LookupKey = BuildStatusCallbackService.Keys.RunStatusUrl
        }, statusCallbackInfo.RunStatusUrl.IsAbsoluteUri ? statusCallbackInfo.RunStatusUrl.AbsoluteUri : statusCallbackInfo.RunStatusUrl.ToString())
      };
      service.AddStrings(requestContext, items);
    }

    public bool TryGet(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      out StatusCallbackInfo statusCallbackInfo)
    {
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(buildData, nameof (buildData));
      return this.TryGet(requestContext, buildData.Id, out statusCallbackInfo);
    }

    public bool TryGet(
      IVssRequestContext requestContext,
      int buildId,
      out StatusCallbackInfo statusCallbackInfo)
    {
      ArgumentUtility.CheckForNonPositiveInt(buildId, nameof (buildId));
      statusCallbackInfo = (StatusCallbackInfo) null;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, BuildStatusCallbackService.GetDrawerName(buildId), false);
      string str;
      string uriString1;
      string uriString2;
      if (drawerId == Guid.Empty || !service.TryGetStrongBoxValue(requestContext, drawerId, BuildStatusCallbackService.Keys.SignatureKey, out str) || !service.TryGetStrongBoxValue(requestContext, drawerId, BuildStatusCallbackService.Keys.JobStatusUrl, out uriString1) || !service.TryGetStrongBoxValue(requestContext, drawerId, BuildStatusCallbackService.Keys.RunStatusUrl, out uriString2))
        return false;
      statusCallbackInfo = new StatusCallbackInfo()
      {
        SignatureKey = str,
        JobStatusUrl = new Uri(uriString1),
        RunStatusUrl = new Uri(uriString2)
      };
      return true;
    }

    private static string GetDrawerName(int buildId) => string.Format("/Service/Pipelines/Runs/{0}/StatusCallback", (object) buildId);

    private static class Keys
    {
      public static readonly string SignatureKey = nameof (SignatureKey);
      public static readonly string JobStatusUrl = nameof (JobStatusUrl);
      public static readonly string RunStatusUrl = nameof (RunStatusUrl);
    }
  }
}
