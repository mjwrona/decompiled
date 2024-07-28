// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImportHostUpgradingFilter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DataImportHostUpgradingFilter : ITeamFoundationRequestFilter
  {
    private static readonly string s_area = "DataImport";
    private static readonly string s_layer = nameof (DataImportHostUpgradingFilter);

    public Task PostAuthenticateRequest(IVssRequestContext requestContext) => Task.CompletedTask;

    public void BeginRequest(IVssRequestContext requestContext)
    {
    }

    public Task BeginRequestAsync(IVssRequestContext requestContext) => Task.CompletedTask;

    public void EndRequest(IVssRequestContext requestContext)
    {
    }

    public void EnterMethod(IVssRequestContext requestContext)
    {
    }

    public void LeaveMethod(IVssRequestContext requestContext)
    {
    }

    public Task PostLogRequestAsync(IVssRequestContext requestContext) => Task.CompletedTask;

    void ITeamFoundationRequestFilter.PostAuthorizeRequest(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, DataImportHostUpgradingFilter.s_area, DataImportHostUpgradingFilter.s_layer, "PostAuthorizeRequest");
      try
      {
        if (this.ShouldBlockRequest(requestContext))
          throw new HostUnavailableDuringDataImportHostUpgradeException(requestContext.GetService<IVssRegistryService>().GetValue<Guid>(requestContext, (RegistryQuery) "/Configuration/DataImport/DataImportId", false, new Guid()));
      }
      finally
      {
        requestContext.TraceLeave(0, DataImportHostUpgradingFilter.s_area, DataImportHostUpgradingFilter.s_layer, "PostAuthorizeRequest");
      }
    }

    private bool ShouldBlockRequest(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.ServiceHostInternal().SubStatus != ServiceHostSubStatus.UpgradeDuringImport)
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
      {
        requestContext.Trace(15080315, TraceLevel.Info, DataImportHostUpgradingFilter.s_area, DataImportHostUpgradingFilter.s_layer, "Requested while host is upgrading without an identity");
        return true;
      }
      bool flag = ServicePrincipals.IsServicePrincipal(requestContext, userIdentity.Descriptor);
      requestContext.Trace(15080316, TraceLevel.Info, DataImportHostUpgradingFilter.s_area, DataImportHostUpgradingFilter.s_layer, string.Format("Descriptor: {0}, Is Service Principal {1}", (object) userIdentity.Descriptor, (object) flag));
      return !flag;
    }
  }
}
