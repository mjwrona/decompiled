// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.UserCredentialsManager
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  public class UserCredentialsManager : IUserCredentialsManager
  {
    private readonly string drawerName;
    private const int c_tracepoint = 1063120;

    public string Area => typeof (UserCredentialsManager).Namespace;

    public string Layer => nameof (UserCredentialsManager);

    public UserCredentialsManager(string drawerName) => this.drawerName = drawerName;

    public string GetStoredCredentials(IVssRequestContext requestContext, bool throwIfNotFound = false)
    {
      this.ValidateRequest(requestContext);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      string userLookupKey = UserCredentialsManager.GetUserLookupKey(vssRequestContext);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, this.drawerName, userLookupKey, throwIfNotFound);
      if (itemInfo != null)
        return service.GetString(vssRequestContext, itemInfo);
      vssRequestContext.Trace(1063120, TraceLevel.Error, this.Area, this.Layer, "Failed to retrieve StrongBox item from drawer '{0}' with lookup key '{1}'", (object) this.drawerName, (object) userLookupKey);
      return (string) null;
    }

    public void SaveCredentials(IVssRequestContext requestContext, string secret)
    {
      this.ValidateRequest(requestContext);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, this.drawerName, false);
      if (drawerId == Guid.Empty)
      {
        drawerId = service.CreateDrawer(vssRequestContext, this.drawerName);
        vssRequestContext.Trace(1063120, TraceLevel.Info, this.Area, this.Layer, "Created StrongBox drawer '{0}'", (object) this.drawerName);
      }
      string userLookupKey = UserCredentialsManager.GetUserLookupKey(vssRequestContext);
      service.AddString(vssRequestContext, drawerId, userLookupKey, secret);
      vssRequestContext.Trace(1063120, TraceLevel.Info, this.Area, this.Layer, "Added item with lookup key '{0}' to StrongBox", (object) userLookupKey);
    }

    public void ClearCredentials(IVssRequestContext requestContext)
    {
      this.ValidateRequest(requestContext);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, this.drawerName, false);
      if (drawerId == Guid.Empty)
      {
        vssRequestContext.Trace(1063120, TraceLevel.Info, this.Area, this.Layer, "StrongBox drawer '{0}' does not exist", (object) this.drawerName);
      }
      else
      {
        string userLookupKey = UserCredentialsManager.GetUserLookupKey(vssRequestContext);
        service.DeleteItem(vssRequestContext, drawerId, userLookupKey);
        vssRequestContext.Trace(1063120, TraceLevel.Info, this.Area, this.Layer, "Deleted item with lookup key '{0}' from StrongBox", (object) userLookupKey);
      }
    }

    private void ValidateRequest(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.CheckPermissionToReadPersonalIdentityInfo();
    }

    private static string GetUserLookupKey(IVssRequestContext requestContext) => requestContext.GetUserId().ToString("D");
  }
}
