// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioExpressLicensingAdapter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioExpressLicensingAdapter : ILicensingAdapter
  {
    protected LicensingServiceConfiguration m_serviceSettings;
    internal static Dictionary<string, string> ExpressEditions = VisualStudioExpressLicensingAdapter.CreateExpressEditions();
    private IVssDateTimeProvider m_dateTimeProvider;
    private const string s_area = "VisualStudio.Services.LicensingService.VisualStudioExpressLicensingAdapter";
    private const string s_layer = "BusinessLogic";
    private const string s_ExpressProgram = "Free Program";

    public VisualStudioExpressLicensingAdapter()
      : this(VssDateTimeProvider.DefaultProvider)
    {
    }

    internal VisualStudioExpressLicensingAdapter(IVssDateTimeProvider dateTimeProvider) => this.m_dateTimeProvider = dateTimeProvider;

    public void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<LicensingServiceConfiguration>(serviceSettings, nameof (serviceSettings));
      this.m_dateTimeProvider = dateTimeProvider;
      this.m_serviceSettings = serviceSettings;
    }

    public void Stop(IVssRequestContext requestContext) => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));

    public IList<IUsageRight> GetRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IRightsQueryContext>(queryContext, nameof (queryContext));
      requestContext.TraceEnter(1031200, "VisualStudio.Services.LicensingService.VisualStudioExpressLicensingAdapter", "BusinessLogic", nameof (GetRights));
      IList<IUsageRight> rights = (IList<IUsageRight>) null;
      try
      {
        if (string.IsNullOrEmpty(queryContext.SingleRightName))
        {
          rights = this.CreateAllExpressRights(requestContext, queryContext);
          return rights;
        }
        string name;
        if (!VisualStudioExpressLicensingAdapter.ExpressEditions.TryGetValue(queryContext.SingleRightName, out name))
        {
          requestContext.Trace(1031200, TraceLevel.Warning, "VisualStudio.Services.LicensingService.VisualStudioExpressLicensingAdapter", "BusinessLogic", "{0}: Unexpected VisualStudioExpressRight name: '{1}.", (object) queryContext.SingleRightName);
          rights = this.CreateAllExpressRights(requestContext, queryContext);
          return rights;
        }
        rights = (IList<IUsageRight>) new List<IUsageRight>()
        {
          this.CreateExpressRight(requestContext, queryContext, name)
        };
        return rights;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1031208, "VisualStudio.Services.LicensingService.VisualStudioExpressLicensingAdapter", "BusinessLogic", ex);
        return this.CreateAllExpressRights(requestContext, queryContext);
      }
      finally
      {
        requestContext.TraceProperties<IList<IUsageRight>>(1031207, "VisualStudio.Services.LicensingService.VisualStudioExpressLicensingAdapter", "BusinessLogic", rights, (string) null);
        requestContext.TraceLeave(1031209, "VisualStudio.Services.LicensingService.VisualStudioExpressLicensingAdapter", "BusinessLogic", nameof (GetRights));
      }
    }

    private IUsageRight CreateExpressRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      string name)
    {
      return (IUsageRight) VisualStudioExpressRight.Create(name, queryContext.ProductVersion, VisualStudioEdition.Unspecified, DateTimeOffset.MaxValue, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), (string) null, "VSExpress", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSExpress"), new Dictionary<string, object>()
      {
        {
          "SubscriptionChannel",
          (object) "Free Program"
        }
      });
    }

    private IList<IUsageRight> CreateAllExpressRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      return (IList<IUsageRight>) VisualStudioExpressLicensingAdapter.ExpressEditions.Keys.Select<string, IUsageRight>((Func<string, IUsageRight>) (name => this.CreateExpressRight(requestContext, queryContext, name))).ToList<IUsageRight>();
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);

    private static Dictionary<string, string> CreateExpressEditions() => new Dictionary<string, string>((IEqualityComparer<string>) LicensingComparers.RightNameComparer)
    {
      {
        "VPDExpress",
        "VPDExpress"
      },
      {
        "VSWinExpress",
        "VSWinExpress"
      },
      {
        "VWDExpress",
        "VWDExpress"
      },
      {
        "WDExpress",
        "WDExpress"
      }
    };
  }
}
