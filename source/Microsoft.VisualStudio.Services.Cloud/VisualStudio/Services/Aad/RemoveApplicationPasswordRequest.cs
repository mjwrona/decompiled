// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.RemoveApplicationPasswordRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class RemoveApplicationPasswordRequest : AadServiceRequest
  {
    public Guid AppObjectId { get; set; }

    public Guid PasswordKeyId { get; set; }

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");
      ArgumentUtility.CheckForEmptyGuid(this.PasswordKeyId, "PasswordKeyId");
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphRemoveApplicationPasswordRequest applicationPasswordRequest = new MsGraphRemoveApplicationPasswordRequest();
      applicationPasswordRequest.AccessToken = context.GetAccessToken(true);
      applicationPasswordRequest.AppObjectId = this.AppObjectId;
      applicationPasswordRequest.PasswordKeyId = this.PasswordKeyId;
      MsGraphRemoveApplicationPasswordRequest request = applicationPasswordRequest;
      MsGraphRemoveApplicationPasswordResponse passwordResponse = context.GetMsGraphClient().RemoveApplicationPassword(context.VssRequestContext, request);
      return (AadServiceResponse) new RemoveApplicationPasswordResponse()
      {
        Success = passwordResponse.Success
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;
  }
}
