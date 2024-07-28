// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetSoftDeletedObjectsRequest`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetSoftDeletedObjectsRequest<TIdentifier, TType> : AadServiceRequest where TType : AadObject
  {
    private Lazy<AadServiceUtils.IdentifierType> IdentifierType = new Lazy<AadServiceUtils.IdentifierType>((Func<AadServiceUtils.IdentifierType>) (() =>
    {
      Type type = typeof (TType);
      if (type == typeof (AadGroup))
        return AadServiceUtils.IdentifierType.Group;
      if (type == typeof (AadUser))
        return AadServiceUtils.IdentifierType.User;
      throw new InvalidOperationException("Unsupported type used for fetching deleted objects.");
    }));

    public GetSoftDeletedObjectsRequest()
    {
    }

    public IEnumerable<TIdentifier> Identifiers { get; set; }

    internal GetSoftDeletedObjectsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    internal override void Validate() => AadServiceUtils.ValidateIds<TIdentifier>(this.Identifiers, this.IdentifierType.Value, "Identifiers");

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      GetSoftDeletedObjectsResponse<TIdentifier, TType> deletedObjectsResponse = new GetSoftDeletedObjectsResponse<TIdentifier, TType>();
      foreach (IEnumerable<KeyValuePair<TIdentifier, Guid>> source in AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, this.Identifiers, this.IdentifierType.Value).ToDictionary<KeyValuePair<TIdentifier, Guid>, TIdentifier, Guid>((Func<KeyValuePair<TIdentifier, Guid>, TIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<TIdentifier, Guid>, Guid>) (kvp => kvp.Value)).Batch<KeyValuePair<TIdentifier, Guid>>(this.GetAadMaxRequestsPerBatch(context)))
      {
        Dictionary<TIdentifier, Guid> dictionary = source.ToDictionary<KeyValuePair<TIdentifier, Guid>, TIdentifier, Guid>((Func<KeyValuePair<TIdentifier, Guid>, TIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<TIdentifier, Guid>, Guid>) (kvp => kvp.Value));
        IAadGraphClient graphClient = context.GetGraphClient();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        GetSoftDeletedObjectsRequest<TType> request = new GetSoftDeletedObjectsRequest<TType>();
        request.AccessToken = context.GetAccessToken();
        request.ObjectIds = (IEnumerable<Guid>) dictionary.Values;
        GetSoftDeletedObjectsResponse<TType> deletedObjectsWithIds = graphClient.GetSoftDeletedObjectsWithIds<TType>(vssRequestContext, request);
        foreach (KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>> convertValue in (IEnumerable<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>>) AadServiceUtils.ConvertValues<TIdentifier, GetSoftDeletedObjectResponse<TType>>(context.VssRequestContext, (IDictionary<TIdentifier, Guid>) dictionary, deletedObjectsWithIds.Objects))
          deletedObjectsResponse.Objects[convertValue.Key] = convertValue.Value;
      }
      return (AadServiceResponse) deletedObjectsResponse;
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      GetSoftDeletedObjectsResponse<TIdentifier, TType> deletedObjectsResponse = new GetSoftDeletedObjectsResponse<TIdentifier, TType>();
      foreach (IEnumerable<KeyValuePair<TIdentifier, Guid>> source in AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, this.Identifiers, this.IdentifierType.Value).ToDictionary<KeyValuePair<TIdentifier, Guid>, TIdentifier, Guid>((Func<KeyValuePair<TIdentifier, Guid>, TIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<TIdentifier, Guid>, Guid>) (kvp => kvp.Value)).Batch<KeyValuePair<TIdentifier, Guid>>(this.GetAadMaxRequestsPerBatch(context)))
      {
        Dictionary<TIdentifier, Guid> dictionary = source.ToDictionary<KeyValuePair<TIdentifier, Guid>, TIdentifier, Guid>((Func<KeyValuePair<TIdentifier, Guid>, TIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<TIdentifier, Guid>, Guid>) (kvp => kvp.Value));
        IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        MsGraphGetSoftDeletedObjectsRequest<TType> request = new MsGraphGetSoftDeletedObjectsRequest<TType>();
        request.AccessToken = context.GetAccessToken(true);
        request.ObjectIds = (IEnumerable<Guid>) dictionary.Values;
        MsGraphGetSoftDeletedObjectsReponse<TType> softDeletedObjects = msGraphClient.GetSoftDeletedObjects<TType>(vssRequestContext, request);
        foreach (KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>> convertValue in (IEnumerable<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>>) AadServiceUtils.ConvertValues<TIdentifier, GetSoftDeletedObjectResponse<TType>>(context.VssRequestContext, (IDictionary<TIdentifier, Guid>) dictionary, softDeletedObjects.DeletedObjects))
          deletedObjectsResponse.Objects[convertValue.Key] = convertValue.Value;
      }
      return (AadServiceResponse) deletedObjectsResponse;
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    private int GetAadMaxRequestsPerBatch(AadServiceRequestContext context)
    {
      int setting = context.GetSetting<int>("/Service/Aad/MaxRequestsPerBatch", 5);
      if (setting < 1)
        return 1;
      if (setting <= 5)
        return setting;
      context.VssRequestContext.Trace(4100005, TraceLevel.Warning, "VisualStudio.Services.Aad", "Service", string.Format("Configured AAD MaxRequestsPerBatch:{0} exceeded AadGraphClientConstants.MaxRequestsPerBatch:{1}. Applying the lower value", (object) setting, (object) 5));
      return 5;
    }
  }
}
