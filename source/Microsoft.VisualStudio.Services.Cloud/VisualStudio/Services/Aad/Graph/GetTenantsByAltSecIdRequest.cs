// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetTenantsByAltSecIdRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetTenantsByAltSecIdRequest : AadGraphClientRequest<GetTenantsByAltSecIdResponse>
  {
    public IdentifierType IdentifierType { get; set; }

    public string Identifier { get; set; }

    internal override void Validate()
    {
      if (this.IdentifierType != IdentifierType.LiveId && this.IdentifierType != IdentifierType.OrgId)
        throw new ArgumentException("Unsupported identifier type: " + this.IdentifierType.ToString());
      ArgumentUtility.CheckStringForNullOrWhiteSpace(this.Identifier, "Identifier");
    }

    internal override GetTenantsByAltSecIdResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<AadTenant> aadTenants = (connection.GetTenantsByAlternativeSecurityId(this.IdentifierType == IdentifierType.OrgId ? AlternativeSecurityIdentifierType.OrgId : AlternativeSecurityIdentifierType.LiveId, this.Identifier) ?? throw new AadException("Failed to get tenants by alternative security ID: connection returned an invalid response.")).Select<GuestTenantDetail, AadTenant>(GetTenantsByAltSecIdRequest.\u003C\u003EO.\u003C0\u003E__ConvertGuestTenant ?? (GetTenantsByAltSecIdRequest.\u003C\u003EO.\u003C0\u003E__ConvertGuestTenant = new Func<GuestTenantDetail, AadTenant>(AadGraphClient.ConvertGuestTenant)));
      return new GetTenantsByAltSecIdResponse()
      {
        Tenants = aadTenants
      };
    }

    public override string ToString() => string.Format("GetTenantsByAltSecIdRequest{0}IdentifierType={1},Identifier={2}{3}", (object) "{", (object) this.IdentifierType, (object) this.Identifier, (object) "}");
  }
}
