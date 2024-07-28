// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetServicePrincipalsByIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetServicePrincipalsByIdsRequest : AadServiceRequest
  {
    public IEnumerable<Guid> Identifiers { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;

    internal override void Validate() => AadServiceUtils.ValidateIds<Guid>(this.Identifiers, AadServiceUtils.IdentifierType.AadServicePrincipal, "Identifiers");

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      Dictionary<Guid, Guid> dictionary1 = AadServiceUtils.MapIds<Guid>(context.VssRequestContext, this.Identifiers, AadServiceUtils.IdentifierType.AadServicePrincipal).ToDictionary<KeyValuePair<Guid, Guid>, Guid, Guid>((Func<KeyValuePair<Guid, Guid>, Guid>) (kvp => kvp.Key), (Func<KeyValuePair<Guid, Guid>, Guid>) (kvp => kvp.Value));
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsRequest();
      request.AccessToken = context.GetAccessToken(true);
      request.ObjectIds = (IEnumerable<Guid>) dictionary1.Values;
      Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsResponse servicePrincipalsByIds = msGraphClient.GetServicePrincipalsByIds(vssRequestContext, request);
      IDictionary<Guid, AadServicePrincipal> dictionary2 = AadServiceUtils.ConvertValues<Guid, AadServicePrincipal>(context.VssRequestContext, (IDictionary<Guid, Guid>) dictionary1, servicePrincipalsByIds.ServicePrincipals);
      return (AadServiceResponse) new GetServicePrincipalsByIdsResponse()
      {
        ServicePrincipals = dictionary2
      };
    }
  }
}
