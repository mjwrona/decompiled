// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.GetAvatarRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class GetAvatarRequest : IOperationRequest
  {
    private static IdentityProvider provider = (IdentityProvider) new DirectoryDiscoveryServiceAdapter();

    internal string ObjectId { get; set; }

    public void Validate(IVssRequestContext requestContext)
    {
      IdentityOperationHelper.ValidateRequestContext(requestContext);
      if (string.IsNullOrEmpty(this.ObjectId))
        throw new IdentityPickerArgumentException("ObjectId (required parameter) is null or empty");
    }

    public IOperationResponse Process(IVssRequestContext requestContext)
    {
      try
      {
        IdentityOperationHelper.ParsedTokens identifiers;
        if (!IdentityOperationHelper.TryParseTokens(requestContext, this.ObjectId, QueryTypeHintEnum.UID, out identifiers) || identifiers.EntityIds == null || identifiers.EntityIds.Count == 0)
          return (IOperationResponse) new GetAvatarResponse()
          {
            Avatar = Array.Empty<byte>()
          };
        byte[] identityImage = GetAvatarRequest.provider.GetIdentityImage(requestContext, this.ObjectId);
        return (IOperationResponse) new GetAvatarResponse()
        {
          Avatar = identityImage
        };
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case IdentityPickerAuthorizationException _:
            throw;
          case IdentityPickerImageNotAvailableException _:
            Tracing.TraceInfo(requestContext, 528, "Image is not available hence returning no content");
            return (IOperationResponse) new GetAvatarResponse()
            {
              Avatar = Array.Empty<byte>()
            };
          case IdentityPickerException _:
            throw;
          default:
            throw new IdentityPickerImageRetrievalException("Identity image could not be retrieved", ex);
        }
      }
    }
  }
}
