// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetTenantRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetTenantRequest : AadGraphClientRequest<GetTenantResponse>
  {
    internal override GetTenantResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      return new GetTenantResponse()
      {
        Tenant = AadGraphClient.ConvertTenant(connection.GetTenantDetails() ?? throw new AadException("Failed to get tenant: connection returned an invalid response."))
      };
    }

    public override string ToString() => string.Format("GetTenantRequest{0}{1}", (object) "{", (object) "}");
  }
}
