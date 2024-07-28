// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.NullUserCredentialsManager
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  public class NullUserCredentialsManager : IUserCredentialsManager
  {
    public string GetStoredCredentials(IVssRequestContext requestContext, bool throwIfNotFound = false) => (string) null;

    public void SaveCredentials(IVssRequestContext requestContext, string secret)
    {
    }

    public void ClearCredentials(IVssRequestContext requestContext)
    {
    }
  }
}
