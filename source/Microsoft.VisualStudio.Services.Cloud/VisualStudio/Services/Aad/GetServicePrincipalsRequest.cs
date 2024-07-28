// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetServicePrincipalsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetServicePrincipalsRequest : AadServicePagedRequest
  {
    public IEnumerable<string> AppIds { get; set; }

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.AppIds);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context) => this.ExecuteWithMicrosoftGraph(context, false);

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetServicePrincipalsRequest request;
      if (string.IsNullOrEmpty(this.PagingToken))
      {
        MsGraphGetServicePrincipalsRequest principalsRequest = new MsGraphGetServicePrincipalsRequest();
        principalsRequest.AccessToken = context.GetAccessToken(true);
        principalsRequest.DisplayNamePrefixes = this.DisplayNamePrefixes;
        principalsRequest.AppIds = this.AppIds;
        principalsRequest.PageSize = this.MaxResults;
        request = principalsRequest;
      }
      else
      {
        MsGraphGetServicePrincipalsRequest principalsRequest = new MsGraphGetServicePrincipalsRequest();
        principalsRequest.AccessToken = context.GetAccessToken(true);
        principalsRequest.PagingToken = this.PagingToken;
        request = principalsRequest;
      }
      MsGraphGetServicePrincipalsResponse servicePrincipals = context.GetMsGraphClient().GetServicePrincipals(context.VssRequestContext, request);
      GetServicePrincipalsResponse principalsResponse = new GetServicePrincipalsResponse();
      principalsResponse.ServicePrincipals = servicePrincipals.ServicePrincipals;
      principalsResponse.PagingToken = servicePrincipals.PagingToken;
      return (AadServiceResponse) principalsResponse;
    }
  }
}
