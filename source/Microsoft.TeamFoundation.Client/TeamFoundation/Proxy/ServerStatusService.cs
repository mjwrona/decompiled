// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.ServerStatusService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Server;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Proxy
{
  internal class ServerStatusService : IServerStatusService
  {
    private ServerStatus _proxy;

    internal ServerStatusService(TfsTeamProjectCollection tfs, string url) => this._proxy = new ServerStatus(tfs, url);

    internal ServerStatusService(string url)
      : this((TfsTeamProjectCollection) null, url)
    {
    }

    public DataChanged[] GetServerStatus()
    {
      try
      {
        return this._proxy.GetServerStatus();
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string GetSupportedContractVersion()
    {
      try
      {
        return this._proxy.GetSupportedContractVersion();
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string CheckAuthentication()
    {
      try
      {
        return this._proxy.CheckAuthentication();
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }
  }
}
