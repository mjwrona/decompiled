// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationTrialService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.Win32;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationTrialService : IVssFrameworkService
  {
    private TeamFoundationIdentity m_admins;
    private static TfsEdition s_tfsEdition;
    private const string c_installFlavor = "InstalledFlavor";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckOnPremisesDeployment();
      this.m_admins = systemRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(systemRequestContext, new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      })[0];
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public static TfsEdition TfsEdition
    {
      get
      {
        if (TeamFoundationTrialService.s_tfsEdition == TfsEdition.None)
        {
          using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\TeamFoundationServer\\19.0", false))
            TeamFoundationTrialService.s_tfsEdition = registryKey == null ? TfsEdition.None : (!(registryKey.GetValue("InstalledFlavor", (object) 0) is int num) ? TfsEdition.None : (TfsEdition) num);
        }
        return TeamFoundationTrialService.s_tfsEdition;
      }
    }

    public ProductTrialState GetTrialState(
      IVssRequestContext requestContext,
      out DateTime trialStartDate,
      out int trialExtensionCount)
    {
      requestContext.CheckDeploymentRequestContext();
      if (TeamFoundationTrialService.TfsEdition != TfsEdition.Trial)
      {
        trialStartDate = new DateTime();
        trialExtensionCount = 0;
        return ProductTrialState.Completed;
      }
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      trialStartDate = service.GetValue<DateTime>(vssRequestContext, (RegistryQuery) LicensingRegistryConstants.TrialStartDate, false, new DateTime());
      trialExtensionCount = service.GetValue<int>(vssRequestContext, (RegistryQuery) LicensingRegistryConstants.TrialExtended, false, 0);
      return TeamFoundationTrialService.GetTrialState(trialStartDate, trialExtensionCount);
    }

    public string GetTrialBannerMessage(IVssRequestContext requestContext)
    {
      DateTime trialStartDate;
      int trialExtensionCount;
      return TeamFoundationTrialService.GetTrialBannerMessage(this.GetTrialState(requestContext, out trialStartDate, out trialExtensionCount), trialStartDate, trialExtensionCount);
    }

    public bool CompleteTrial(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      if (!TeamFoundationTrialService.IsDeploymentAdmin(requestContext))
        return false;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<IVssRegistryService>().DeleteEntries(vssRequestContext, LicensingRegistryConstants.TrialStartDate, LicensingRegistryConstants.TrialExtended);
      return true;
    }

    public bool ExtendTrial(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      if (!TeamFoundationTrialService.IsDeploymentAdmin(requestContext))
        return false;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      DateTime dateTime = service.GetValue<DateTime>(vssRequestContext, (RegistryQuery) LicensingRegistryConstants.TrialStartDate, false, new DateTime());
      if (dateTime == new DateTime())
        return true;
      if ((DateTime.UtcNow - dateTime).TotalHours >= 2160.0)
        return false;
      service.SetValue<int>(vssRequestContext, LicensingRegistryConstants.TrialExtended, 1);
      return true;
    }

    public static string GetTrialBannerMessage(
      ProductTrialState trialState,
      DateTime trialStartDate,
      int trialExtensionCount)
    {
      if (trialState == ProductTrialState.Completed || trialState == ProductTrialState.InTrial)
        return (string) null;
      string shortDateString = trialStartDate.Add(TimeSpan.FromDays((double) (60 + trialExtensionCount * 30))).ToShortDateString();
      return trialState == ProductTrialState.Expired ? Resources.TrialExpiredBanner((object) shortDateString) : Resources.TrialExpiredCanExtendBanner((object) shortDateString);
    }

    public static bool IsDeploymentAdmin(IVssRequestContext requestContext)
    {
      requestContext.CheckOnPremisesDeployment();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationTrialService>().IAdmin(vssRequestContext);
    }

    private bool IAdmin(IVssRequestContext requestContext)
    {
      requestContext.CheckOnPremisesDeployment();
      if (requestContext.IsSystemContext)
        return true;
      IVssRequestContext requestContext1 = requestContext.Elevate();
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      return requestContext.UserContext != (IdentityDescriptor) null && service.IsMember(requestContext1, this.m_admins.Descriptor, requestContext.UserContext);
    }

    internal static ProductTrialState GetTrialState(
      DateTime trialStartDate,
      int trialExtensionCount)
    {
      if (trialStartDate == new DateTime())
        return ProductTrialState.Completed;
      DateTime utcNow = DateTime.UtcNow;
      if (trialExtensionCount < 0)
        trialExtensionCount = 0;
      else if (trialExtensionCount > 1)
        trialExtensionCount = 1;
      if ((trialStartDate.AddDays((double) (60 + 30 * trialExtensionCount)) - utcNow).TotalHours >= 0.0)
        return ProductTrialState.InTrial;
      return utcNow > trialStartDate.AddDays(90.0) ? ProductTrialState.Expired : ProductTrialState.ExpiredCanExtend;
    }

    public static string GetTrialAdminMessage(
      ProductTrialState trialState,
      DateTime trialStartDate,
      int trialExtensionCount)
    {
      if (trialState == ProductTrialState.Completed)
        return (string) null;
      DateTime dateTime = trialStartDate.AddDays((double) (60 + 30 * trialExtensionCount));
      string trialAdminMessage = (string) null;
      switch (trialState)
      {
        case ProductTrialState.InTrial:
          trialAdminMessage = Resources.InTrialMessage((object) dateTime.ToShortDateString());
          break;
        case ProductTrialState.ExpiredCanExtend:
          trialAdminMessage = Resources.TrialExpiredCanExtendMessage((object) trialStartDate.AddDays(90.0).ToShortDateString());
          break;
        case ProductTrialState.Expired:
          trialAdminMessage = Resources.TrialExpiredMessage();
          break;
      }
      return trialAdminMessage;
    }
  }
}
