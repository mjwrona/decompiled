// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.ConnectedServerContextKeyService
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  public class ConnectedServerContextKeyService : 
    IConnectedServerContextKeyService,
    IVssFrameworkService
  {
    private readonly string m_serverVersion;
    private static readonly TimeSpan ExpiryIterval = TimeSpan.FromHours(24.0);
    private static readonly Guid GalleryServiceId = new Guid("00000029-0000-8888-8000-000000000000");

    public ConnectedServerContextKeyService()
    {
      Version version = Version.Parse(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
      this.m_serverVersion = string.Format("{0}.{1}.{2}.{3}", (object) "19", (object) 2, (object) version.Build, (object) version.Revision);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckOnPremisesDeployment();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string ServerVersion => this.m_serverVersion;

    public string GetToken(IVssRequestContext requestContext, Dictionary<string, string> properties)
    {
      Dictionary<string, string> dictionary = properties ?? new Dictionary<string, string>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationInternetConnectivityService service1 = vssRequestContext.GetService<TeamFoundationInternetConnectivityService>();
      ILocationService service2 = vssRequestContext.GetService<ILocationService>();
      string str = new Uri(service2.GetLocationServiceUrl(vssRequestContext, ConnectedServerContextKeyService.GalleryServiceId, AccessMappingConstants.ClientAccessMappingMoniker)).AbsoluteUri + "_gallery/";
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.ServerVersion, this.m_serverVersion);
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.ServerId, vssRequestContext.ServiceHost.InstanceId.ToString());
      Uri uri = new Uri(service2.GetPublicAccessMapping(vssRequestContext).AccessPoint);
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.ServerName, uri.Host);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.CollectionId, requestContext.ServiceHost.InstanceId.ToString());
        dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.CollectionName, requestContext.ServiceHost.Name);
        ICloudConnectedService service3 = requestContext.GetService<ICloudConnectedService>();
        Uri baseUri = new Uri(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ConnectedServerContextKeyService.GalleryServiceId, AccessMappingConstants.ClientAccessMappingMoniker));
        dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.CollectionUrl, baseUri.AbsoluteUri);
        Guid connectedAccountId = service3.GetConnectedAccountId(requestContext);
        if (!connectedAccountId.Equals(Guid.Empty))
        {
          dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.BillingAccountName, service3.GetConnectedAccountName(requestContext));
          dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.BillingAccountId, connectedAccountId.ToString("D"));
        }
        else
        {
          string absoluteUri = new Uri(baseUri, "_gallery/server/connect").AbsoluteUri;
          dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.ConnectUrl, absoluteUri);
          dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.RegistrationKey, service3.GetRegistrationKey(requestContext));
          dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.RegistrationId, service3.GetRegistrationId(requestContext).ToString("D"));
        }
        string absoluteUri1 = new Uri(baseUri, "_admin/_userhub?synchronizeCommerceData=true").AbsoluteUri;
        dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.UserHubUrl, absoluteUri1);
      }
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.IsAdmin, this.IsAdmin(requestContext).ToString());
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.GalleryUrl, str);
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.HasInternetAccess, service1.IsConnectedToInternet.ToString());
      OnPremFeatures onPremFeatures = OnPremFeatures.ValidateConnectServer;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableNewAcquisitionOnPremExperience"))
        onPremFeatures |= OnPremFeatures.NewAcqExperience;
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.EnabledFeatures, onPremFeatures.ToString("D"));
      Guid guid = Guid.NewGuid();
      dictionary.TryAdd<string, string>(CloudConnectedServerShortNameConstants.AuthenticationToken, guid.ToString());
      return CloudConnectedUtilities.EncodeToken(dictionary);
    }

    public bool IsValidAuthToken(IVssRequestContext requestContext, string token)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(token, nameof (token));
      string tokenRegistryPath = this.GetAuthTokenRegistryPath(token);
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) tokenRegistryPath, string.Empty);
      return !string.IsNullOrWhiteSpace(str) && !this.hasAuthTokenExpired(JsonConvert.DeserializeObject<ConnectedAuthTokenData>(str).creationTime);
    }

    public void SaveAuthToken(IVssRequestContext requestContext, string token)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(token, nameof (token));
      this.cleanExpiredEntries(requestContext);
      string tokenRegistryPath = this.GetAuthTokenRegistryPath(token);
      ConnectedAuthTokenData connectedAuthTokenData = new ConnectedAuthTokenData()
      {
        creationTime = DateTime.UtcNow
      };
      requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, tokenRegistryPath, connectedAuthTokenData.ToString());
    }

    private void cleanExpiredEntries(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(requestContext, (RegistryQuery) (CloudConnectedConstants.ConnectedServerAuthToken + "/**"));
      List<string> stringList = new List<string>();
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        if (this.hasAuthTokenExpired(JsonConvert.DeserializeObject<ConnectedAuthTokenData>(registryEntry.Value).creationTime))
          stringList.Add(registryEntry.Path);
      }
      if (stringList.Count <= 0)
        return;
      service.DeleteEntries(requestContext, stringList.ToArray());
    }

    private bool hasAuthTokenExpired(DateTime creationTime) => DateTime.UtcNow.Subtract(creationTime) > ConnectedServerContextKeyService.ExpiryIterval;

    private string GetAuthTokenRegistryPath(string token)
    {
      string shA256Hash = this.GetSHA256Hash(token);
      return string.Format("{0}/{1}", (object) CloudConnectedConstants.ConnectedServerAuthToken, (object) shA256Hash);
    }

    private string GetSHA256Hash(string token) => string.Join("", ((IEnumerable<byte>) new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(token))).Select<byte, string>((Func<byte, string>) (h => h.ToString("X2"))));

    private bool IsAdmin(IVssRequestContext requestContext) => requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext.UserContext);
  }
}
