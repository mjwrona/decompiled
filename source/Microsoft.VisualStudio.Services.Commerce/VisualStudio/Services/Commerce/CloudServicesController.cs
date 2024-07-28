// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CloudServicesController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [XmlFormatter]
  public class CloudServicesController : CommerceControllerBase
  {
    private IResourceOutputBuilder resourceOutputBuilder;

    internal override string Layer => nameof (CloudServicesController);

    internal IResourceOutputBuilder ResourceOutputBuilder
    {
      get
      {
        if (this.resourceOutputBuilder == null)
          this.resourceOutputBuilder = (IResourceOutputBuilder) new Microsoft.VisualStudio.Services.Commerce.ResourceOutputBuilder();
        return this.resourceOutputBuilder;
      }
      set => this.resourceOutputBuilder = value;
    }

    internal virtual bool RequestIsCsmPassthrough()
    {
      this.TfsRequestContext.TraceEnter(5106371, this.Area, this.Layer, nameof (RequestIsCsmPassthrough));
      try
      {
        IEnumerable<string> values;
        return this.Request.Headers.TryGetValues("x-ms-csmpassthrough", out values) && bool.TryParse(values.FirstOrDefault<string>(), out bool _);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(5106380, this.Area, this.Layer, nameof (RequestIsCsmPassthrough));
      }
    }

    [HttpGet]
    [CaptureRdfeOperationId]
    [TraceFilter(5103000, 5103050)]
    [TraceExceptions(5103049)]
    [TraceRequest(5103002)]
    [TraceResponse(5103048)]
    public CloudServiceOutput GetAllResourcesInCloudService(
      string subscriptionId,
      string cloudServiceName)
    {
      Guid result;
      if (!Guid.TryParse(subscriptionId, out result))
        throw new ArgumentException("subscriptionId with value \"" + subscriptionId + "\" does not contain valid Guid");
      ArgumentUtility.CheckStringForNullOrEmpty(cloudServiceName, nameof (cloudServiceName));
      ArgumentUtility.CheckForEmptyGuid(result, nameof (subscriptionId));
      if (this.RequestIsCsmPassthrough())
      {
        this.TfsRequestContext.Trace(5103003, TraceLevel.Info, this.Area, this.Layer, "Detected header x-ms-csmpassthrough, return empty result set to avoid unwanted resource deletion from RDFE legacy job.");
        return new CloudServiceOutput()
        {
          GeoRegion = "West US",
          Resources = new ResourceOutputCollection()
        };
      }
      IEnumerable<AzureResourceAccount> source = this.TfsRequestContext.GetService<PlatformSubscriptionService>().QueryForAzureResourceAccountsBySubscriptionId(this.TfsRequestContext, result, AccountProviderNamespace.VisualStudioOnline);
      this.TfsRequestContext.Trace(5103048, TraceLevel.Info, this.Area, this.Layer, string.Format("Retrieved {0} resources", (object) source.Count<AzureResourceAccount>()));
      List<ResourceOutput> resourceOutputList = new List<ResourceOutput>();
      resourceOutputList.AddRange(source.Where<AzureResourceAccount>((Func<AzureResourceAccount, bool>) (x => cloudServiceName.Equals(x.AzureCloudServiceName, StringComparison.Ordinal) || cloudServiceName.Equals(x.AlternateCloudServiceName, StringComparison.Ordinal))).Select<AzureResourceAccount, ResourceOutput>((Func<AzureResourceAccount, ResourceOutput>) (azureResourceAccount => this.ResourceOutputBuilder.CreateResourceOutput(azureResourceAccount, this.TfsRequestContext))));
      string str = "West US";
      if (resourceOutputList.Any<ResourceOutput>() && resourceOutputList[0].CloudServiceSettings != null)
        str = resourceOutputList[0].CloudServiceSettings.GeoRegion;
      return new CloudServiceOutput()
      {
        GeoRegion = str,
        Resources = new ResourceOutputCollection((IEnumerable<ResourceOutput>) resourceOutputList)
      };
    }

    [HttpDelete]
    [CaptureRdfeOperationId]
    [TraceFilter(5103051, 5103100)]
    [TraceExceptions(5103099)]
    public void DeleteCloudService(string subscriptionId, string cloudServiceName)
    {
      Guid result;
      if (!Guid.TryParse(subscriptionId, out result))
        throw new ArgumentException("subscriptionId with value \"" + subscriptionId + "\" does not contain valid Guid");
      ArgumentUtility.CheckStringForNullOrEmpty(cloudServiceName, nameof (cloudServiceName));
      ArgumentUtility.CheckForEmptyGuid(result, nameof (subscriptionId));
      this.TfsRequestContext.GetService<PlatformSubscriptionService>().DeleteAzureCloudService(this.TfsRequestContext, result, AccountProviderNamespace.VisualStudioOnline, cloudServiceName);
    }

    [HttpPut]
    [CaptureRdfeOperationId]
    [TraceFilter(5105751, 5105800)]
    [TraceExceptions(5105799)]
    public HttpResponseMessage PutCloudService(string subscriptionId, string cloudServiceName) => new HttpResponseMessage(HttpStatusCode.OK);
  }
}
