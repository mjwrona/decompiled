// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.DeleteServicePrincipalRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class DeleteServicePrincipalRequest : AadServiceRequest
  {
    public Guid ServicePrincipalObjectId { get; set; }

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.ServicePrincipalObjectId, "ServicePrincipalObjectId");

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphDeleteServicePrincipalRequest principalRequest = new MsGraphDeleteServicePrincipalRequest();
      principalRequest.AccessToken = context.GetAccessToken(true);
      principalRequest.ServicePrincipalObjectId = this.ServicePrincipalObjectId;
      MsGraphDeleteServicePrincipalRequest request = principalRequest;
      MsGraphDeleteServicePrincipalResponse principalResponse = context.GetMsGraphClient().DeleteServicePrincipal(context.VssRequestContext, request);
      return (AadServiceResponse) new DeleteServicePrincipalResponse()
      {
        Success = principalResponse.Success
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;
  }
}
