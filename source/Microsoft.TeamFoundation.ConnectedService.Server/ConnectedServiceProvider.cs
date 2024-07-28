// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.ConnectedServiceProvider
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.ConnectedService.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  public abstract class ConnectedServiceProvider
  {
    public abstract string Id { get; }

    public virtual IUserCredentialsManager UserCredentialsManager => (IUserCredentialsManager) new NullUserCredentialsManager();

    public virtual void Start(IVssRequestContext requestContext)
    {
    }

    public abstract AuthRequest CreateAuthRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      AuthRequest authRequest,
      Guid oauthConfigurationId = default (Guid),
      string endpointType = null,
      string scope = null,
      string callbackQueryParams = null);

    public abstract string CompleteCallback(
      IVssRequestContext requestContext,
      string code,
      string state,
      string nonce,
      out bool redirect);

    public virtual List<Installation> GetAppInstallations(
      IVssRequestContext requestContext,
      Guid projectId,
      string oauthTokenStrongBoxKey)
    {
      throw new NotSupportedException();
    }
  }
}
