// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioAndroidEmulatorLicenseAdapter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioAndroidEmulatorLicenseAdapter : ILicensingAdapter
  {
    private readonly ILicensingConfigurationManager m_settingsManager;
    protected LicensingServiceConfiguration m_serviceSettings;
    protected VisualStudioEmulatorAndroidAdapterConfiguration m_adapterSettings;
    private IVssDateTimeProvider m_dateTimeProvider;
    private const string s_area = "VisualStudio.Services.LicensingService.VisualStudioEmulatorAndroidAdapter";
    private const string s_layer = "BusinessLogic";

    public VisualStudioAndroidEmulatorLicenseAdapter()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), VssDateTimeProvider.DefaultProvider)
    {
    }

    internal VisualStudioAndroidEmulatorLicenseAdapter(
      ILicensingConfigurationManager settingsManager,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.m_settingsManager = settingsManager;
      this.m_dateTimeProvider = dateTimeProvider;
    }

    public void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<LicensingServiceConfiguration>(serviceSettings, nameof (serviceSettings));
      this.m_dateTimeProvider = dateTimeProvider;
      this.m_serviceSettings = serviceSettings;
      this.PopulateSettings(requestContext);
    }

    public void Stop(IVssRequestContext requestContext) => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));

    public IList<IUsageRight> GetRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IRightsQueryContext>(queryContext, nameof (queryContext));
      requestContext.TraceEnter(1053600, "VisualStudio.Services.LicensingService.VisualStudioEmulatorAndroidAdapter", "BusinessLogic", nameof (GetRights));
      List<IUsageRight> rights = new List<IUsageRight>();
      if (VisualStudioAndroidEmulatorLicenseAdapter.ValidateQueryContext(requestContext, queryContext))
        rights.Add((IUsageRight) this.CreateVisualStudioRight(requestContext, queryContext));
      requestContext.TraceProperties<List<IUsageRight>>(1053608, "VisualStudio.Services.LicensingService.VisualStudioEmulatorAndroidAdapter", "BusinessLogic", rights, (string) null);
      requestContext.TraceLeave(1053609, "VisualStudio.Services.LicensingService.VisualStudioEmulatorAndroidAdapter", "BusinessLogic", nameof (GetRights));
      return (IList<IUsageRight>) rights;
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_adapterSettings = this.m_settingsManager.GetVisualStudioEmulatorAndroidAdapterConfiguration(requestContext);

    private VisualStudioEmulatorAndroidRight CreateVisualStudioRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      return VisualStudioEmulatorAndroidRight.Create("VisualStudioEmulatorAndroid", queryContext.ProductVersion, queryContext.VisualStudioEdition, this.m_adapterSettings.LicenseDurationSeconds > 0 ? DateTimeOffset.Now.AddSeconds((double) this.m_adapterSettings.LicenseDurationSeconds) : DateTimeOffset.MaxValue, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), this.m_serviceSettings.LicenseUrl, "VSEmulatorAndroid", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSEmulatorAndroid"));
    }

    private static bool ValidateQueryContext(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      if (queryContext.VisualStudioEdition != VisualStudioEdition.Unspecified)
      {
        requestContext.Trace(1051402, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioEmulatorAndroidAdapter", "BusinessLogic", "Invalid product edition '{0}'.", (object) queryContext.VisualStudioEdition);
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductEdition());
      }
      if (queryContext.ProductVersion == (Version) null)
      {
        requestContext.Trace(1053603, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioEmulatorAndroidAdapter", "BusinessLogic", "Invalid product version.");
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
      return queryContext.VisualStudioFamily == VisualStudioFamily.VisualStudioEmulatorAndroid;
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);
  }
}
