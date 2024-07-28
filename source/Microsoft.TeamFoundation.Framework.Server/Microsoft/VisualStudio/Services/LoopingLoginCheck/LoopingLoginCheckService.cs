// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.LoopingLoginCheck.LoopingLoginCheckService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.LoopingLoginCheck
{
  public class LoopingLoginCheckService : ILoopingLoginCheckService, IVssFrameworkService
  {
    private const string s_area = "Authentication";
    private const string s_layer = "LoopingLoginCheckService";
    private static readonly RegistryQuery m_blockedVsidsRegistryQuery = new RegistryQuery("/Configuration/LoopingLoginCheck/BlockedVsids");
    private HashSet<Guid> m_blockedVsids;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      this.m_blockedVsids = ((IEnumerable<string>) service.GetValue<string>(vssRequestContext, in LoopingLoginCheckService.m_blockedVsidsRegistryQuery, string.Empty).Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, Guid>((Func<string, Guid>) (v => Guid.Parse(v))).ToHashSet<Guid>();
      service.RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), in LoopingLoginCheckService.m_blockedVsidsRegistryQuery);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public void KillLoopingLogin(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      Guid userId = requestContext.GetUserId();
      if (this.m_blockedVsids.Count == 0 || this.m_blockedVsids.Contains(userId))
      {
        requestContext.Trace(119200, TraceLevel.Error, "Authentication", nameof (LoopingLoginCheckService), string.Format("Looping login detected for user with [Id]: {0}", (object) userId));
        throw new IdentityLoopingLoginException("Unable to complete authentication for user due to looping logins");
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_blockedVsids = ((IEnumerable<string>) vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in LoopingLoginCheckService.m_blockedVsidsRegistryQuery, string.Empty).Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, Guid>((Func<string, Guid>) (v => Guid.Parse(v))).ToHashSet<Guid>();
      requestContext.TraceDataConditionally(119201, TraceLevel.Info, "Authentication", nameof (LoopingLoginCheckService), "Updating blocked vsids with update registry value", (Func<object>) (() => (object) new
      {
        m_blockedVsids = this.m_blockedVsids
      }), nameof (OnRegistryChanged));
    }
  }
}
