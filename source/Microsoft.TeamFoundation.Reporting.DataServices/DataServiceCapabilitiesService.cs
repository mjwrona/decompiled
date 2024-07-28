// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataServiceCapabilitiesService
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class DataServiceCapabilitiesService : IDataServiceCapabilitiesService, IVssFrameworkService
  {
    public DataServiceCapabilities GetDataServiceCapabilities(
      IVssRequestContext requestContext,
      string scope)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(scope, nameof (scope));
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        IDataServiceCapabilityProvider capabilityProvider = requestContext.GetService<FeatureProviderRegistryService>().SelectProvider(requestContext, scope).GetCapabilityProvider(requestContext);
        return new DataServiceCapabilities()
        {
          Scope = capabilityProvider.GetScopeName(),
          HistoryRanges = capabilityProvider.GetHistoryRanges(requestContext).Select<FixedIntervalDateRange, NameLabelPair>((Func<FixedIntervalDateRange, NameLabelPair>) (o => o.GetInfo())),
          PluralArtifactName = capabilityProvider.GetArtifactPluralName(requestContext),
          Fields = (IEnumerable<FieldInfo>) capabilityProvider.GetFields(requestContext).OrderBy<FieldInfo, string>((Func<FieldInfo, string>) (o => o.LabelText)),
          NumericalAggregationFunctions = AggregationMediator.GetNumericalAggregationFunctions(requestContext)
        };
      }
      catch (Exception ex)
      {
        TelemetryHelper.PublishChartingRequestFailure(requestContext, scope, nameof (GetDataServiceCapabilities), ex);
        throw;
      }
      finally
      {
        stopwatch.Stop();
        TelemetryHelper.PublishCapabilitiesRequest(requestContext, scope, stopwatch.Elapsed);
      }
    }

    public IDataServiceCapabilityProvider GetCapabilityProvider(
      IVssRequestContext requestContext,
      string scope)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, scope).GetCapabilityProvider(requestContext);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
