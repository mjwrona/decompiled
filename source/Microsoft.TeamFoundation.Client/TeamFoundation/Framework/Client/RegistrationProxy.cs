// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistrationProxy
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class RegistrationProxy
  {
    private Registration _proxy;
    private string _tfsServerUrl;
    private const string _UrlSeparator = "/";

    internal RegistrationProxy(TfsTeamProjectCollection tfsObject, string url)
    {
      if (url == null)
        throw new ArgumentNullException(nameof (url));
      this._tfsServerUrl = tfsObject == null ? url : tfsObject.Uri.AbsoluteUri;
      this._tfsServerUrl = ProxyUtilities.GetServerUrl(this._tfsServerUrl);
      this._proxy = new Registration(tfsObject, url);
    }

    public FrameworkRegistrationEntry[] GetRegistrationEntries(string toolId)
    {
      try
      {
        FrameworkRegistrationEntry[] registrationEntries = this._proxy.GetRegistrationEntries(toolId);
        RegistrationProxy.ExpandRelativeUrls(registrationEntries, this._tfsServerUrl);
        return registrationEntries;
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
      catch (WebException ex)
      {
        throw;
      }
      catch (InvalidOperationException ex)
      {
        throw new TeamFoundationServiceUnavailableException(ClientResources.ConnectToTfs_AddServer_UnableToConnect((object) this._tfsServerUrl, (object) this._tfsServerUrl));
      }
    }

    internal static void ExpandRelativeUrls(
      FrameworkRegistrationEntry[] regEntries,
      string tfsServerUrl)
    {
      if (regEntries == null)
        return;
      foreach (FrameworkRegistrationEntry regEntry in regEntries)
      {
        if (!VssStringComparer.RegistrationEntryName.Equals(regEntry.Type, "TeamProjects"))
        {
          for (int index = 0; index < regEntry.ServiceInterfaces.Length; ++index)
            regEntry.ServiceInterfaces[index].Url = RegistrationProxy.GetAbsoluteUrl(regEntry.ServiceInterfaces[index].Url, tfsServerUrl);
        }
      }
    }

    private static string GetAbsoluteUrl(string relativeUrl, string tfsServerUrl)
    {
      if (string.IsNullOrEmpty(relativeUrl))
        relativeUrl = "/";
      else if (!relativeUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        return relativeUrl;
      return tfsServerUrl + relativeUrl;
    }
  }
}
