// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetConnectionsRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations
{
  internal sealed class GetConnectionsRequest : OperationRequest
  {
    internal IList<string> ConnectionTypes { get; set; }

    internal IList<string> IdentityTypes { get; set; }

    internal IList<string> OperationScopes { get; set; }

    internal string ObjectId { get; set; }

    internal HashSet<string> Properties { get; set; }

    internal string PagingToken { get; set; }

    internal int Depth { get; set; }

    internal Dictionary<string, object> Options { get; set; }

    protected override OperationResponse Process(IVssRequestContext requestContext) => (OperationResponse) new GetConnectionsResponse(IdentityOperation.GetConnections(requestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.GetConnectionsRequest()
    {
      IdentityTypes = this.IdentityTypes,
      OperationScopes = this.OperationScopes,
      ConnectionTypes = this.ConnectionTypes,
      ObjectId = this.ObjectId,
      PagingToken = this.PagingToken,
      RequestProperties = this.Properties,
      Depth = this.Depth
    }));
  }
}
