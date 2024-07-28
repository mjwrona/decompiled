// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.VSProvider.AadProviderConfiguration
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Client.Keychain.VSProvider
{
  internal class AadProviderConfiguration : IAadProviderConfiguration
  {
    private const string ValidateAadAuthorityName = "ValidateAadAuthority";
    private const string EnableAzureRMIdentityName = "EnableAzureRMIdentity";
    private const string AzureRMEndpointName = "AzureRMEndpoint";
    private const string GraphEndpointName = "GraphUrl";
    private const string AzureRMAudienceEndpointName = "AzureRMAudienceEndpoint";
    private const string ScopesName = "Scopes";
    private const string GraphScopesName = "GraphScopes";
    private const string AzureRMAudienceScopesName = "AzureRMAudienceScopes";
    private static readonly Uri DefaultAzureResourceManagementEndpoint = new Uri("https://management.azure.com/");
    private static readonly Uri DefaultGraphEndpoint = new Uri("https://graph.windows.net/");
    private static readonly Uri DefaultResourceEndpoint = new Uri("https://management.core.windows.net/");
    private static readonly string[] DefaultScopes = new string[1]
    {
      AadProviderConfiguration.DefaultResourceEndpoint.AbsoluteUri + "/.default"
    };
    private static readonly string[] DefaultGraphScopes = new string[1]
    {
      AadProviderConfiguration.DefaultGraphEndpoint.AbsoluteUri + "/.default"
    };
    private static readonly string[] DefaultAzureRMAudienceScopes = new string[1]
    {
      AadProviderConfiguration.DefaultAzureResourceManagementEndpoint.AbsoluteUri + "/.default"
    };
    private Uri resourceEndpoint;

    public string AadAuthorityBase => VssAadSettings.AadInstance;

    public bool ValidateAadAuthority
    {
      get
      {
        bool result = true;
        string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>(nameof (ValidateAadAuthority));
        if (!string.IsNullOrEmpty(connectedUserValue) && !bool.TryParse(connectedUserValue, out result))
          result = true;
        return result;
      }
    }

    public bool AzureRMIdentityEnabled => VssClientEnvironment.GetSharedConnectedUserValue<bool>("EnableAzureRMIdentity");

    public string ClientIdentifier => VssAadSettings.ClientId;

    public Uri NativeClientRedirect => VssAadSettings.NativeClientRedirectUri;

    public Uri ResourceEndpoint
    {
      get
      {
        if (this.resourceEndpoint == (Uri) null)
          this.resourceEndpoint = AadProviderConfiguration.DefaultResourceEndpoint;
        Uri registryOverride = AccountManagementUtilities.GetEndpointRegistryOverride();
        return (object) registryOverride != null ? registryOverride : this.resourceEndpoint;
      }
      internal set => this.resourceEndpoint = value;
    }

    public Uri AzureResourceManagementEndpoint
    {
      get
      {
        Uri result = (Uri) null;
        string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>("AzureRMEndpoint");
        try
        {
          Uri.TryCreate(connectedUserValue, UriKind.Absolute, out result);
        }
        catch
        {
        }
        Uri uri = result;
        return (object) uri != null ? uri : AadProviderConfiguration.DefaultAzureResourceManagementEndpoint;
      }
    }

    public Uri GraphEndpoint
    {
      get
      {
        Uri result = (Uri) null;
        string uriString = VssClientEnvironment.GetSharedConnectedUserValue<string>("GraphUrl");
        try
        {
          if (!string.IsNullOrEmpty(uriString) && !uriString.StartsWith("https://"))
            uriString = "https://" + uriString;
          Uri.TryCreate(uriString, UriKind.Absolute, out result);
        }
        catch
        {
        }
        Uri uri = result;
        return (object) uri != null ? uri : AadProviderConfiguration.DefaultGraphEndpoint;
      }
    }

    public Uri AzureRMAudienceEndpoint
    {
      get
      {
        Uri result = (Uri) null;
        string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>(nameof (AzureRMAudienceEndpoint));
        try
        {
          Uri.TryCreate(connectedUserValue, UriKind.Absolute, out result);
        }
        catch
        {
        }
        Uri uri = result;
        return (object) uri != null ? uri : this.AzureResourceManagementEndpoint;
      }
    }

    public string[] Scopes => VssClientEnvironment.GetSharedConnectedUserValue<string[]>(nameof (Scopes)) ?? AadProviderConfiguration.DefaultScopes;

    public string[] GraphScopes => VssClientEnvironment.GetSharedConnectedUserValue<string[]>(nameof (GraphScopes)) ?? AadProviderConfiguration.DefaultGraphScopes;

    public string[] AzureRMAudienceScopes => VssClientEnvironment.GetSharedConnectedUserValue<string[]>(nameof (AzureRMAudienceScopes)) ?? AadProviderConfiguration.DefaultAzureRMAudienceScopes;
  }
}
