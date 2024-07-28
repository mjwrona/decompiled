// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.UpdateApplicationRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class UpdateApplicationRequest : AadServiceRequest
  {
    public AadApplication ApplicationToUpdate { get; set; }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphUpdateApplicationRequest applicationRequest = new MsGraphUpdateApplicationRequest();
      applicationRequest.AccessToken = context.GetAccessToken(true);
      applicationRequest.ApplicationToUpdate = this.ApplicationToUpdate;
      MsGraphUpdateApplicationRequest request = applicationRequest;
      MsGraphUpdateApplicationResponse applicationResponse = context.GetMsGraphClient().UpdateApplication(context.VssRequestContext, request);
      return (AadServiceResponse) new UpdateApplicationResponse()
      {
        UpdatedApplication = applicationResponse.ApplicationUpdated
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;
  }
}
