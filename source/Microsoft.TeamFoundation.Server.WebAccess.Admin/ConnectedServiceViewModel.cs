// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ConnectedServiceViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class ConnectedServiceViewModel
  {
    public ConnectedServiceViewModel(
      IVssRequestContext requestContext,
      ConnectedServiceMetadata connectedService)
    {
      this.DisplayName = connectedService.FriendlyName;
      this.Description = connectedService.Description;
      this.Name = connectedService.Name;
      this.ServiceUri = connectedService.ServiceUri;
      this.AuthenticatedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
      {
        connectedService.AuthenticatedBy
      })[0];
      this.Kind = connectedService.Kind;
    }

    public string DisplayName { get; private set; }

    public string Description { get; private set; }

    public string Name { get; private set; }

    public string ServiceUri { get; private set; }

    public string ImageUri { get; set; }

    public ConnectedServiceKind Kind { get; private set; }

    public TeamFoundationIdentity AuthenticatedBy { get; private set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("displayName", (object) this.DisplayName);
      json.Add("description", (object) this.Description);
      json.Add("name", (object) this.Name);
      json.Add("serviceUri", (object) this.ServiceUri);
      json.Add("imageUri", (object) this.ImageUri);
      json.Add("authenticatedBy", (object) this.AuthenticatedBy.GetAttribute("Account", string.Empty));
      json.Add("kind", (object) this.Kind);
      return json;
    }
  }
}
