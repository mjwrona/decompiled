// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetApplicationByIdRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetApplicationByIdRequest : AadServiceRequest
  {
    public Guid AppObjectId { get; set; }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetApplicationByIdRequest applicationByIdRequest = new MsGraphGetApplicationByIdRequest();
      applicationByIdRequest.AccessToken = context.GetAccessToken(true);
      applicationByIdRequest.AppObjectId = this.AppObjectId;
      MsGraphGetApplicationByIdRequest request = applicationByIdRequest;
      MsGraphGetApplicationByIdResponse applicationById = context.GetMsGraphClient().GetApplicationById(context.VssRequestContext, request);
      return (AadServiceResponse) new GetApplicationByIdResponse()
      {
        Application = applicationById.Application
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;
  }
}
