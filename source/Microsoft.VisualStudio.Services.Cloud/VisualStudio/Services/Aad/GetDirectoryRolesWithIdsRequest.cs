// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetDirectoryRolesWithIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetDirectoryRolesWithIdsRequest : AadServiceRequest
  {
    public GetDirectoryRolesWithIdsRequest()
    {
    }

    internal GetDirectoryRolesWithIdsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public IEnumerable<Guid> Identifiers { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override void Validate() => AadServiceUtils.ValidateIds<Guid>(this.Identifiers, name: "Identifiers");

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesWithIdsRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesWithIdsRequest();
      request.AccessToken = context.GetAccessToken();
      request.ObjectIds = this.Identifiers;
      Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesWithIdsResponse directoryRolesWithIds = graphClient.GetDirectoryRolesWithIds(vssRequestContext, request);
      return (AadServiceResponse) new GetDirectoryRolesWithIdsResponse()
      {
        DirectoryRoles = directoryRolesWithIds.DirectoryRoles
      };
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetDirectoryRolesWithIdsRequest rolesWithIdsRequest = new MsGraphGetDirectoryRolesWithIdsRequest();
      rolesWithIdsRequest.AccessToken = context.GetAccessToken(true);
      rolesWithIdsRequest.ObjectIds = this.Identifiers;
      MsGraphGetDirectoryRolesWithIdsRequest request = rolesWithIdsRequest;
      MsGraphGetDirectoryRolesWithIdsResponse directoryRolesWithIds = context.GetMsGraphClient().GetDirectoryRolesWithIds(context.VssRequestContext, request);
      return (AadServiceResponse) new GetDirectoryRolesWithIdsResponse()
      {
        DirectoryRoles = directoryRolesWithIds.DirectoryRoles
      };
    }
  }
}
