// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.RegionControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class RegionControllerBase : TfsApiController
  {
    private const string IbizaDisableAllRegionsFeatureFlag = "VisualStudio.Services.Commerce.IbizaDisableAllregions";

    [HttpGet]
    [TraceFilter(5104101, 5104200)]
    public IList<AzureRegion> GetAccountRegions()
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      try
      {
        List<AzureRegion> azureRegionList = new List<AzureRegion>();
        return !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.IbizaDisableAllregions") ? this.TfsRequestContext.GetExtension<ICommerceAccountHandler>().GetSupportedAzureRegions(this.TfsRequestContext) : (IList<AzureRegion>) azureRegionList;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5104102, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }
  }
}
