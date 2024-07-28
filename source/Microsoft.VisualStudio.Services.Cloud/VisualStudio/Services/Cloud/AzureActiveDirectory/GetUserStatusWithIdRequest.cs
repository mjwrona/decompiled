// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.GetUserStatusWithIdRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory
{
  public class GetUserStatusWithIdRequest : AadServiceRequest
  {
    public GetUserStatusWithIdRequest()
    {
    }

    internal GetUserStatusWithIdRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public Guid ObjectId { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;

    internal override void Validate() => AadServiceUtils.ValidateId<Guid>(this.ObjectId, AadServiceUtils.IdentifierType.User);

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.GetUserStatusWithIdRequest request = new Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.GetUserStatusWithIdRequest();
      request.AccessToken = context.GetAccessToken(true);
      request.ObjectId = this.ObjectId;
      return (AadServiceResponse) new GetUserStatusWithIdResponse(msGraphClient.GetUserStatusWithId(vssRequestContext, request).User);
    }
  }
}
