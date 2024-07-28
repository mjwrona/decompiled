// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetGroupsWithIdsRequest`1
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
  public class GetGroupsWithIdsRequest<TIdentifier> : AadServiceRequest
  {
    public GetGroupsWithIdsRequest()
    {
    }

    internal GetGroupsWithIdsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public IEnumerable<TIdentifier> Identifiers { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override void Validate() => AadServiceUtils.ValidateIds<TIdentifier>(this.Identifiers, AadServiceUtils.IdentifierType.Group, "Identifiers");

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      Dictionary<TIdentifier, Guid> dictionary1 = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, this.Identifiers, AadServiceUtils.IdentifierType.Group).ToDictionary<KeyValuePair<TIdentifier, Guid>, TIdentifier, Guid>((Func<KeyValuePair<TIdentifier, Guid>, TIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<TIdentifier, Guid>, Guid>) (kvp => kvp.Value));
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      GetGroupsWithIdsRequest request = new GetGroupsWithIdsRequest();
      request.AccessToken = context.GetAccessToken();
      request.ObjectIds = (IEnumerable<Guid>) dictionary1.Values;
      GetGroupsWithIdsResponse groupsWithIds = graphClient.GetGroupsWithIds(vssRequestContext, request);
      IDictionary<TIdentifier, AadGroup> dictionary2 = AadServiceUtils.ConvertValues<TIdentifier, AadGroup>(context.VssRequestContext, (IDictionary<TIdentifier, Guid>) dictionary1, groupsWithIds.Groups);
      return (AadServiceResponse) new GetGroupsWithIdsResponse<TIdentifier>()
      {
        Groups = dictionary2
      };
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      Dictionary<TIdentifier, Guid> dictionary1 = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, this.Identifiers, AadServiceUtils.IdentifierType.Group).ToDictionary<KeyValuePair<TIdentifier, Guid>, TIdentifier, Guid>((Func<KeyValuePair<TIdentifier, Guid>, TIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<TIdentifier, Guid>, Guid>) (kvp => kvp.Value));
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      MsGraphGetGroupsWithIdsRequest request = new MsGraphGetGroupsWithIdsRequest();
      request.AccessToken = context.GetAccessToken(true);
      request.ObjectIds = (IEnumerable<Guid>) dictionary1.Values;
      MsGraphGetGroupsWithIdsResponse groupsWithIds = msGraphClient.GetGroupsWithIds(vssRequestContext, request);
      IDictionary<TIdentifier, AadGroup> dictionary2 = AadServiceUtils.ConvertValues<TIdentifier, AadGroup>(context.VssRequestContext, (IDictionary<TIdentifier, Guid>) dictionary1, groupsWithIds.Groups);
      return (AadServiceResponse) new GetGroupsWithIdsResponse<TIdentifier>()
      {
        Groups = dictionary2
      };
    }
  }
}
