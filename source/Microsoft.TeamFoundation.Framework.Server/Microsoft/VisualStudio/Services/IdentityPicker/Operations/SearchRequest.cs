// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations
{
  public sealed class SearchRequest : OperationRequest
  {
    public string Query { get; set; }

    public string QueryTypeHint { get; set; }

    public IList<string> IdentityTypes { get; set; }

    public IList<string> OperationScopes { get; set; }

    public IList<string> RequestedProperties { get; set; }

    public IList<string> FilterByAncestorEntityIds { get; set; }

    public IList<string> FilterByEntityIds { get; set; }

    public string PagingToken { get; set; }

    protected override OperationResponse Process(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse response = IdentityOperation.Search(requestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequest(this.Query, this.QueryTypeHint, this.IdentityTypes, this.OperationScopes, this.ExtensionData as SearchOptions, this.RequestedProperties, this.FilterByAncestorEntityIds, this.FilterByEntityIds, this.PagingToken));
      if (this.ExtensionData != null)
      {
        try
        {
          ((SearchOptions) this.ExtensionData).ParseOptions();
        }
        catch (Exception ex)
        {
          throw new IdentityPickerExtensionDataParsingException("ExtensionData could not be parsed", ex);
        }
      }
      return (OperationResponse) new SearchResponse(response);
    }
  }
}
