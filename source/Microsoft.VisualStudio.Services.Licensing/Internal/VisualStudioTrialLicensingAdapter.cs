// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioTrialLicensingAdapter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioTrialLicensingAdapter : ILicensingAdapter
  {
    private readonly ILicensingConfigurationManager m_settingsManager;
    private readonly ServiceFactory<IVssRegistryService> m_registryServiceFactory;
    protected LicensingServiceConfiguration m_serviceSettings;
    protected VisualStudioTrialAdapterConfiguration m_adapterSettings;
    private IVssDateTimeProvider m_dateTimeProvider;
    private const string s_area = "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter";
    private const string s_layer = "BusinessLogic";
    private const string s_TrialProgram = "Evaluation";

    public VisualStudioTrialLicensingAdapter()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), (ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()), VssDateTimeProvider.DefaultProvider)
    {
    }

    internal VisualStudioTrialLicensingAdapter(
      ILicensingConfigurationManager settingsManager,
      ServiceFactory<IVssRegistryService> registryServiceFactory,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.m_settingsManager = settingsManager;
      this.m_registryServiceFactory = registryServiceFactory;
      this.m_dateTimeProvider = dateTimeProvider;
    }

    public void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<LicensingServiceConfiguration>(serviceSettings, nameof (serviceSettings));
      this.m_registryServiceFactory(requestContext).RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Licensing/Msdn/...");
      this.m_serviceSettings = serviceSettings;
      this.m_dateTimeProvider = dateTimeProvider;
      this.PopulateSettings(requestContext);
    }

    public void Stop(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_registryServiceFactory(requestContext).UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public IList<IUsageRight> GetRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IRightsQueryContext>(queryContext, nameof (queryContext));
      requestContext.TraceEnter(1031800, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", nameof (GetRights));
      VisualStudioTrialLicensingAdapter.ValidateQueryContext(requestContext, queryContext);
      DateTimeOffset trialExpirationDate = this.GetDefaultTrialExpirationDate();
      DateTimeOffset expirationDate;
      try
      {
        expirationDate = this.GetTrialExpirationDate(requestContext, queryContext, trialExpirationDate);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1031808, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", ex);
        expirationDate = trialExpirationDate;
      }
      List<IUsageRight> rights;
      if (queryContext.VisualStudioFamily == VisualStudioFamily.TestManager)
        rights = new List<IUsageRight>()
        {
          (IUsageRight) this.CreateTestManagerRight(requestContext, queryContext, expirationDate)
        };
      else
        rights = new List<IUsageRight>()
        {
          (IUsageRight) this.CreateVisualStudioRight(requestContext, queryContext, expirationDate)
        };
      requestContext.TraceProperties<List<IUsageRight>>(1031807, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", rights, (string) null);
      requestContext.TraceLeave(1031809, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", nameof (GetRights));
      return (IList<IUsageRight>) rights;
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1031820, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", nameof (OnRegistryChanged));
      try
      {
        if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
          return;
        this.PopulateSettings(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1031828, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", ex);
      }
      finally
      {
        requestContext.TraceLeave(1031829, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", nameof (OnRegistryChanged));
      }
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_adapterSettings = this.m_settingsManager.GetVisualStudioTrialAdapterConfiguration(requestContext);

    private DateTimeOffset GetTrialExpirationDate(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      DateTimeOffset defaultTrialExpirationDate)
    {
      using (LicensingComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<LicensingComponent>())
      {
        IList<DateTimeOffset> trialExpirations = component.GetVisualStudioTrialExpirations(queryContext.MachineId, queryContext.UserIdentity.MasterId, queryContext.ProductVersion.Major, (int) queryContext.VisualStudioFamily, (int) queryContext.VisualStudioEdition, defaultTrialExpirationDate);
        if (trialExpirations != null && trialExpirations.Count == 2)
          return trialExpirations.OrderBy<DateTimeOffset, DateTimeOffset>((Func<DateTimeOffset, DateTimeOffset>) (date => date)).First<DateTimeOffset>();
        requestContext.Trace(1031810, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", "Unexpected LicensingComponent.GetVisualStudioTrialExpirations did not return any expiration dates.");
        return defaultTrialExpirationDate;
      }
    }

    private DateTimeOffset GetDefaultTrialExpirationDate() => this.GetOffsetUtcNow().Add(this.m_adapterSettings.TrialExtensionDuration);

    private VisualStudioRight CreateVisualStudioRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      DateTimeOffset expirationDate)
    {
      return VisualStudioRight.Create(VisualStudioRight.MapProductFamilyToRightName(queryContext.VisualStudioFamily), queryContext.ProductVersion, queryContext.VisualStudioEdition, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), this.m_serviceSettings.LicenseUrl, "VSExtensionTrial", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSExtensionTrial"), new Dictionary<string, object>()
      {
        {
          "SubscriptionChannel",
          (object) "Evaluation"
        },
        {
          "IsTrialLicense",
          (object) true
        },
        {
          "SubscriptionLevel",
          (object) "Community"
        }
      });
    }

    private TestManagerRight CreateTestManagerRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      DateTimeOffset expirationDate)
    {
      return TestManagerRight.Create("TestManager", queryContext.ProductVersion, queryContext.VisualStudioEdition, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), this.m_serviceSettings.LicenseUrl, "VSExtensionTrial", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSExtensionTrial"), new Dictionary<string, object>()
      {
        {
          "SubscriptionChannel",
          (object) "Evaluation"
        },
        {
          "IsTrialLicense",
          (object) true
        },
        {
          "SubscriptionLevel",
          (object) "Community"
        }
      });
    }

    private static void ValidateQueryContext(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      if (string.IsNullOrWhiteSpace(queryContext.MachineId))
      {
        requestContext.Trace(1031840, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", "Invalid machine id: '{0}'.", (object) (queryContext.MachineId ?? string.Empty));
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextMachineId());
      }
      if (queryContext.VisualStudioFamily == VisualStudioFamily.Invalid)
      {
        requestContext.Trace(1031841, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", "Invalid product family '{0}'.", (object) queryContext.VisualStudioFamily);
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductFamily());
      }
      if (queryContext.VisualStudioFamily != VisualStudioFamily.TestManager && queryContext.VisualStudioEdition == VisualStudioEdition.Unspecified)
      {
        requestContext.Trace(1031842, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", "Invalid product edition'{0}'.", (object) queryContext.VisualStudioEdition);
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductEdition());
      }
      if (queryContext.ProductVersion == (Version) null)
      {
        requestContext.Trace(1031842, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioTrialLicensingAdapter", "BusinessLogic", "Invalid product version.");
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);
  }
}
