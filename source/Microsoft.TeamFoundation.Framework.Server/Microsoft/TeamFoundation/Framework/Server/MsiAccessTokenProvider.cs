// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MsiAccessTokenProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MsiAccessTokenProvider : IMsiAccessTokenProvider
  {
    private const string c_tokenPath = "/identity/oauth2/token";
    private readonly IAzureInstanceMetadataProvider m_imdsClient;
    private readonly MsiTokenCache m_cache;

    public MsiAccessTokenProvider(MsiTokenCache cache, IAzureInstanceMetadataProvider imdsClient)
    {
      this.m_imdsClient = imdsClient;
      this.m_cache = cache;
    }

    public void ClearCache() => this.m_cache?.Clear();

    public string GetAccessToken(string resource) => this.GetMsiAccessToken(resource).AccessToken;

    public MsiToken GetMsiAccessToken(string resource)
    {
      MsiToken token = this.m_cache?.GetToken(resource);
      if (token == null)
      {
        token = this.RetrieveAccessToken(resource);
        this.m_cache?.SetToken(resource, token);
      }
      return token;
    }

    internal MsiToken RetrieveAccessToken(string resource)
    {
      string str = (string) null;
      using (SafeHandle registryKey = RegistryHelper.OpenSubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\TeamFoundationServer", RegistryAccessMask.Execute | RegistryAccessMask.Wow6464Key))
      {
        if (registryKey != null)
          str = RegistryHelper.GetValue(registryKey, "ManagedIdentityClientId", (object) null) as string;
      }
      try
      {
        Dictionary<string, string> parameters = new Dictionary<string, string>()
        {
          [nameof (resource)] = resource
        };
        if (!string.IsNullOrEmpty(str))
          parameters["client_id"] = str;
        return MsiToken.Parse(this.m_imdsClient.GetMetadata("/identity/oauth2/token", parameters));
      }
      catch (Exception ex) when (!(ex is ManagedIdentityException))
      {
        throw new ApplicationException("Error retrieving the Managed Service Identity", ex);
      }
    }
  }
}
