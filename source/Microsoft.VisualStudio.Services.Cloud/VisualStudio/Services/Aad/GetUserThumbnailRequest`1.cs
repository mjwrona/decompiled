// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetUserThumbnailRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetUserThumbnailRequest<TIdentifier> : AadServiceRequest
  {
    public TIdentifier Identifier { get; set; }

    public GetUserThumbnailRequest()
    {
    }

    internal GetUserThumbnailRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    internal override void Validate() => AadServiceUtils.ValidateId<TIdentifier>(this.Identifier, name: "Identifier");

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      GetUserThumbnailRequest thumbnailRequest = new GetUserThumbnailRequest();
      thumbnailRequest.AccessToken = context.GetAccessToken();
      thumbnailRequest.ObjectId = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).First<KeyValuePair<TIdentifier, Guid>>().Value;
      GetUserThumbnailRequest request = thumbnailRequest;
      Microsoft.VisualStudio.Services.Aad.Graph.GetUserThumbnailResponse userThumbnail = graphClient.GetUserThumbnail(vssRequestContext, request);
      return (AadServiceResponse) new GetUserThumbnailResponse()
      {
        Thumbnail = userThumbnail.Thumbnail
      };
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetUserThumbnailRequest thumbnailRequest = new MsGraphGetUserThumbnailRequest();
      thumbnailRequest.AccessToken = context.GetAccessToken(true);
      thumbnailRequest.UserObjectId = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).First<KeyValuePair<TIdentifier, Guid>>().Value;
      MsGraphGetUserThumbnailRequest request = thumbnailRequest;
      MsGraphGetUserThumbnailReponse userThumbnail = context.GetMsGraphClient().GetUserThumbnail(context.VssRequestContext, request);
      return (AadServiceResponse) new GetUserThumbnailResponse()
      {
        Thumbnail = userThumbnail.Thumbnail
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;
  }
}
