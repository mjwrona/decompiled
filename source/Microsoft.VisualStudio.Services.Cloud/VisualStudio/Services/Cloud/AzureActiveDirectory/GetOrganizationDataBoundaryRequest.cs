// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.GetOrganizationDataBoundaryRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory
{
  public class GetOrganizationDataBoundaryRequest : AadServiceRequest
  {
    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetOrganizationDataBoundaryRequest dataBoundaryRequest = new MsGraphGetOrganizationDataBoundaryRequest();
      dataBoundaryRequest.AccessToken = context.GetAccessToken(true);
      MsGraphGetOrganizationDataBoundaryRequest request = dataBoundaryRequest;
      MsGraphGetOrganizationDataBoundaryResponse organizationDataBoundary = context.GetMsGraphClient().GetOrganizationDataBoundary(context.VssRequestContext, request);
      return (AadServiceResponse) new GetOrganizationDataBoundaryResponse()
      {
        DataBoundary = organizationDataBoundary.DataBoundary
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;
  }
}
