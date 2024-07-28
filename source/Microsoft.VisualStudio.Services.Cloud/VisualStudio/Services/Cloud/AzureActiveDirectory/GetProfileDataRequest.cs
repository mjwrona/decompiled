// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.GetProfileDataRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory
{
  public class GetProfileDataRequest : AadServiceRequest
  {
    public IdentityDescriptor UserDescriptor { get; set; }

    internal override void Validate() => ArgumentUtility.CheckGenericForNull((object) this.UserDescriptor, "UserDescriptor");

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetProfileDataRequest profileDataRequest = new MsGraphGetProfileDataRequest();
      profileDataRequest.AccessToken = context.GetAccessToken(true, true);
      profileDataRequest.ObjectId = AadServiceUtils.MapIds<IdentityDescriptor>(context.VssRequestContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        this.UserDescriptor
      }, AadServiceUtils.IdentifierType.User).First<KeyValuePair<IdentityDescriptor, Guid>>().Value;
      MsGraphGetProfileDataRequest request = profileDataRequest;
      MsGraphGetProfileDataResponse profileData = context.GetMsGraphClient().GetProfileData(context.VssRequestContext, request);
      return (AadServiceResponse) new GetProfileDataResponse()
      {
        DisplayName = profileData.DisplayName,
        Mail = profileData.Mail
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;

    internal override bool UseBetaGraphVersion => true;
  }
}
