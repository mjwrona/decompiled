// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.GetMruRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class GetMruRequest : IOperationRequest
  {
    private OperationScopeEnum operationScope;
    private static IdentityProvider mruProvider = (IdentityProvider) new MruServiceAdapter();

    internal string RequestIdentityId { get; set; }

    internal string FeatureId { get; set; }

    internal IList<string> OperationScopes { get; set; }

    internal IList<string> Properties { get; set; }

    internal IList<string> FilterByAncestorEntityIds { get; set; }

    internal IList<string> FilterByEntityIds { get; set; }

    internal int? MaxItemsCount { get; set; }

    public void Validate(IVssRequestContext requestContext)
    {
      IdentityOperationHelper.ValidateRequestContext(requestContext);
      IdentityOperationHelper.CheckRequestByAuthorizedMember(requestContext);
      this.operationScope = IdentityOperationHelper.ParseOperationScopes(this.OperationScopes);
      this.RequestIdentityId = !string.IsNullOrEmpty(this.RequestIdentityId) && !(this.RequestIdentityId.Trim().ToLower() != "me") ? this.RequestIdentityId.Trim().ToLower() : throw new IdentityPickerArgumentException("RequestIdentityId (required parameter) is null or empty or is not 'me' (only supported value)");
      this.FeatureId = !string.IsNullOrEmpty(this.FeatureId) && !(this.FeatureId.Trim().ToLower() != "common") ? this.FeatureId.Trim().ToLower() : throw new IdentityPickerArgumentException("FeatureId (required parameter) is null or empty or is is not 'common' (only supported value)");
    }

    public IOperationResponse Process(IVssRequestContext requestContext)
    {
      IList<Guid> identities1 = GetMruRequest.mruProvider.GetIdentities(requestContext);
      if (identities1.Count == 0)
        return (IOperationResponse) new GetMruResponse()
        {
          MruIdentities = (IList<Identity>) new List<Identity>()
        };
      IVssRequestContext requestContext1 = requestContext;
      IList<Guid> vsIds = identities1;
      IList<string> properties = this.Properties;
      Dictionary<string, object> presetProperties = new Dictionary<string, object>();
      presetProperties.Add("IsMru", (object) true);
      IList<string> ancestorEntityIds = this.FilterByAncestorEntityIds;
      IList<string> filterByEntityIds = this.FilterByEntityIds;
      int? maxItemsCount = this.MaxItemsCount;
      IList<Identity> identities2 = DirectoryDiscoveryServiceAdapter.ResolveVsidsOrSubjectDescriptorsToIdentities(requestContext1, vsIds, (List<SubjectDescriptor>) null, (IEnumerable<string>) properties, (IDictionary<string, object>) presetProperties, (IEnumerable<string>) ancestorEntityIds, (IEnumerable<string>) filterByEntityIds, maxItemsCount);
      return (IOperationResponse) new GetMruResponse()
      {
        MruIdentities = identities2
      };
    }
  }
}
