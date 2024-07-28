// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetAvatarRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations
{
  internal sealed class GetAvatarRequest : OperationRequest
  {
    internal string ObjectId { get; set; }

    protected override OperationResponse Process(IVssRequestContext requestContext) => (OperationResponse) new GetAvatarResponse(IdentityOperation.GetAvatar(requestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.GetAvatarRequest()
    {
      ObjectId = this.ObjectId
    }));
  }
}
