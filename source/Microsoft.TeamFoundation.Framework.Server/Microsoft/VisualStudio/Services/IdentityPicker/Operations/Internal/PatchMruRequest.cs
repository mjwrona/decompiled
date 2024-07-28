// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.PatchMruRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class PatchMruRequest : IOperationRequest
  {
    private OperationScopeEnum operationScope;
    private static IdentityProvider mruProvider = (IdentityProvider) new MruServiceAdapter();

    internal string RequestIdentityId { get; set; }

    internal string FeatureId { get; set; }

    internal IList<string> OperationScopes { get; set; }

    internal PatchOperationTypeEnum PatchOperationType { get; set; }

    internal IList<string> ObjectIds { get; set; }

    public void Validate(IVssRequestContext requestContext)
    {
      IdentityOperationHelper.ValidateRequestContext(requestContext);
      IdentityOperationHelper.CheckRequestByAuthorizedMember(requestContext);
      this.operationScope = IdentityOperationHelper.ParseOperationScopes(this.OperationScopes);
      this.RequestIdentityId = !string.IsNullOrEmpty(this.RequestIdentityId) && !(this.RequestIdentityId.Trim().ToLower() != "me") ? this.RequestIdentityId.Trim().ToLower() : throw new IdentityPickerArgumentException("RequestIdentityId (required parameter) is null or empty or is not 'me' (only supported value)");
      this.FeatureId = !string.IsNullOrEmpty(this.FeatureId) && !(this.FeatureId.Trim().ToLower() != "common") ? this.FeatureId.Trim().ToLower() : throw new IdentityPickerArgumentException("FeatureId (required parameter) is null or empty or is is not 'common' (only supported value)");
      if (this.ObjectIds == null || this.ObjectIds.Count == 0)
        throw new IdentityPickerArgumentException("ObjectIds (required parameter) is null or empty");
    }

    public IOperationResponse Process(IVssRequestContext requestContext)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (string objectId in (IEnumerable<string>) this.ObjectIds)
      {
        Guid result;
        if (Guid.TryParse(objectId, out result))
          source.Add(result);
      }
      if (source.Count == 0)
        return (IOperationResponse) new PatchMruResponse()
        {
          Result = false
        };
      switch (this.PatchOperationType)
      {
        case PatchOperationTypeEnum.Add:
          PatchMruRequest.mruProvider.AddIdentities(requestContext, (IList<string>) source.Select<Guid, string>((Func<Guid, string>) (x => x.ToString())).ToList<string>());
          break;
        case PatchOperationTypeEnum.Remove:
          PatchMruRequest.mruProvider.RemoveIdentities(requestContext, (IList<string>) source.Select<Guid, string>((Func<Guid, string>) (x => x.ToString())).ToList<string>());
          break;
      }
      bool flag = source.Count == this.ObjectIds.Count;
      return (IOperationResponse) new PatchMruResponse()
      {
        Result = flag
      };
    }
  }
}
