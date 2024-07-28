// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioCommunityLicenseAdapter
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
  public class VisualStudioCommunityLicenseAdapter : ILicensingAdapter
  {
    private readonly ILicensingConfigurationManager m_settingsManager;
    protected LicensingServiceConfiguration m_serviceSettings;
    protected VisualStudioCommunityAdapterConfiguration m_adapterSettings;
    private IVssDateTimeProvider m_dateTimeProvider;
    private const string s_area = "VisualStudio.Services.LicensingService.VisualStudioCommunityLicensingAdapter";
    private const string s_layer = "BusinessLogic";
    private const string s_CommunityProgram = "Free Program";

    public VisualStudioCommunityLicenseAdapter()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), VssDateTimeProvider.DefaultProvider)
    {
    }

    internal VisualStudioCommunityLicenseAdapter(
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
      requestContext.TraceEnter(1051400, "VisualStudio.Services.LicensingService.VisualStudioCommunityLicensingAdapter", "BusinessLogic", nameof (GetRights));
      List<IUsageRight> rights = new List<IUsageRight>();
      if (VisualStudioCommunityLicenseAdapter.ValidateQueryContext(requestContext, queryContext))
        rights.Add((IUsageRight) this.CreateVisualStudioRight(requestContext, queryContext));
      requestContext.TraceProperties<List<IUsageRight>>(1051408, "VisualStudio.Services.LicensingService.VisualStudioCommunityLicensingAdapter", "BusinessLogic", rights, (string) null);
      requestContext.TraceLeave(1051409, "VisualStudio.Services.LicensingService.VisualStudioCommunityLicensingAdapter", "BusinessLogic", nameof (GetRights));
      return (IList<IUsageRight>) rights;
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_adapterSettings = this.m_settingsManager.GetVisualStudioCommunityAdapterConfiguration(requestContext);

    private VisualStudioRight CreateVisualStudioRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      return VisualStudioRight.Create(VisualStudioRight.MapProductFamilyToRightName(queryContext.VisualStudioFamily), queryContext.ProductVersion, queryContext.VisualStudioEdition, this.m_adapterSettings.LicenseDurationSeconds > 0 ? DateTimeOffset.Now.AddSeconds((double) this.m_adapterSettings.LicenseDurationSeconds) : DateTimeOffset.MaxValue, string.Empty, string.Empty, "VSCommunity", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSCommunity"), new Dictionary<string, object>()
      {
        {
          "SubscriptionChannel",
          (object) "Free Program"
        },
        {
          "SubscriptionLevel",
          (object) "Community"
        }
      });
    }

    private static bool ValidateQueryContext(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      if (queryContext.VisualStudioFamily == VisualStudioFamily.Invalid)
      {
        requestContext.Trace(1051402, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioCommunityLicensingAdapter", "BusinessLogic", "Invalid product family '{0}'.", (object) queryContext.VisualStudioFamily);
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductFamily());
      }
      if (queryContext.ProductVersion == (Version) null)
      {
        requestContext.Trace(1051403, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioCommunityLicensingAdapter", "BusinessLogic", "Invalid product version.");
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
      return queryContext.VisualStudioEdition == VisualStudioEdition.Community;
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);
  }
}
