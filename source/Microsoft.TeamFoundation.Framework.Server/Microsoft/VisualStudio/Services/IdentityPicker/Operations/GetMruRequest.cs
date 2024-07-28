// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetMruRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations
{
  internal class GetMruRequest : OperationRequest
  {
    internal string RequestIdentityId { get; set; }

    internal string FeatureId { get; set; }

    internal IList<string> OperationScopes { get; set; }

    internal IList<string> Properties { get; set; }

    internal IList<string> FilterByAncestorEntityIds { get; set; }

    internal IList<string> FilterByEntityIds { get; set; }

    internal int? MaxItemsCount { get; set; }

    protected override OperationResponse Process(IVssRequestContext requestContext) => (OperationResponse) new GetMruResponse(IdentityOperation.GetMru(requestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.GetMruRequest()
    {
      RequestIdentityId = this.RequestIdentityId,
      FeatureId = this.FeatureId,
      OperationScopes = this.OperationScopes,
      Properties = this.Properties,
      FilterByAncestorEntityIds = this.FilterByAncestorEntityIds,
      FilterByEntityIds = this.FilterByEntityIds,
      MaxItemsCount = this.MaxItemsCount
    }));
  }
}
