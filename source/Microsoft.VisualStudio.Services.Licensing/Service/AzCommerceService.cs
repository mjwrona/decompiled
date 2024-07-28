// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.AzCommerceService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.Rest.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Enums;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Service
{
  public class AzCommerceService : IVssFrameworkService
  {
    private readonly string _area = "Licensing";
    private readonly string _layer = nameof (AzCommerceService);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<MeterUsage2GetResponse> GetLicenseMeterUsages(
      IVssRequestContext requestContext)
    {
      return this.GetAllMeterUsages(requestContext).Where<MeterUsage2GetResponse>((Func<MeterUsage2GetResponse, bool>) (i => i.MeterKind == MeterKind.Assignment));
    }

    public AccountLicenseType GetDefaultLicenseType(IVssRequestContext requestContext)
    {
      DefaultLicenseTypeGetResponse licenseTypeGetResponse = requestContext.To(TeamFoundationHostType.Deployment).GetClient<AzCommHttpClient>().GetDefaultLicenseTypeAsync(requestContext.ServiceHost.InstanceId).SyncResult<DefaultLicenseTypeGetResponse>();
      requestContext.Trace(2428345, TraceLevel.Info, this._area, this._layer, string.Format("AzComm default license returned => {0}", (object) licenseTypeGetResponse.DefaultLicenseType));
      return licenseTypeGetResponse.DefaultLicenseType;
    }

    private IEnumerable<MeterUsage2GetResponse> GetAllMeterUsages(IVssRequestContext requestContext)
    {
      List<MeterUsage2GetResponse> meterUsages = requestContext.To(TeamFoundationHostType.Deployment).GetClient<AzCommHttpClient>().GetAllMeterUsagesAsync(requestContext.ServiceHost.InstanceId).SyncResult<List<MeterUsage2GetResponse>>();
      requestContext.TraceDataConditionally(9171911, TraceLevel.Info, this._area, this._layer, "AzComm meter usages returned", (Func<object>) (() => (object) meterUsages), nameof (GetAllMeterUsages));
      return (IEnumerable<MeterUsage2GetResponse>) meterUsages;
    }
  }
}
