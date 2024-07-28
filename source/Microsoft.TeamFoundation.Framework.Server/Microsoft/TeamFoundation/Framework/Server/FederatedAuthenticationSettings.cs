// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FederatedAuthenticationSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FederatedAuthenticationSettings
  {
    internal FederatedAuthenticationSettings(
      RegistryEntryCollection settings,
      IEnumerable<string> serviceIdentitySigningKeys)
    {
      this.PassiveRedirectEnabled = settings[nameof (PassiveRedirectEnabled)].GetValue<bool>(true);
      this.RequireHttps = settings[nameof (RequireHttps)].GetValue<bool>(false);
      this.Issuer = settings[nameof (Issuer)].GetValue<string>((string) null);
      this.DefaultRealm = settings["Realm"].GetValue<string>((string) null);
      this.ServiceIdentitySigningKeys = serviceIdentitySigningKeys.ToArray<string>();
      this.SignOutLocations = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry setting in settings)
      {
        if (setting.Path.StartsWith(FederatedAuthRegistryConstants.SignOutLocations, StringComparison.OrdinalIgnoreCase))
          this.SignOutLocations.Add(setting.Name, setting.Value);
      }
      this.Realms = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry setting in settings)
      {
        if (setting.Path.StartsWith(FederatedAuthRegistryConstants.AlternateRealms, StringComparison.OrdinalIgnoreCase))
          this.Realms.Add(setting.Name, setting.Value);
      }
      this.Realms[AccessMappingConstants.PublicAccessMappingMoniker] = this.DefaultRealm;
      Dictionary<string, FederatedAuthenticationSettings.CustomClientSignInOptions> dictionary = new Dictionary<string, FederatedAuthenticationSettings.CustomClientSignInOptions>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry setting in settings)
      {
        if (setting.Path.StartsWith(FederatedAuthRegistryConstants.ClientSignInOptions, StringComparison.OrdinalIgnoreCase))
        {
          string fileName = Path.GetFileName(Path.GetDirectoryName(setting.Path));
          FederatedAuthenticationSettings.CustomClientSignInOptions clientSignInOptions;
          if (!dictionary.TryGetValue(fileName, out clientSignInOptions))
          {
            clientSignInOptions = new FederatedAuthenticationSettings.CustomClientSignInOptions();
            dictionary.Add(fileName, clientSignInOptions);
          }
          if (StringComparer.OrdinalIgnoreCase.Equals(setting.Name, "UserAgent"))
            clientSignInOptions.UserAgent = setting.Value.ToUpperInvariant();
          if (StringComparer.OrdinalIgnoreCase.Equals(setting.Name, "Options"))
            clientSignInOptions.Options = setting.Value;
        }
      }
      this.ClientSignInOptions = dictionary.Values.ToList<FederatedAuthenticationSettings.CustomClientSignInOptions>();
    }

    public bool PassiveRedirectEnabled { get; set; }

    public bool RequireHttps { get; set; }

    public string Issuer { get; set; }

    public string DefaultRealm { get; set; }

    public Dictionary<string, string> Realms { get; set; }

    public string[] ServiceIdentitySigningKeys { get; set; }

    public Dictionary<string, string> SignOutLocations { get; set; }

    public List<FederatedAuthenticationSettings.CustomClientSignInOptions> ClientSignInOptions { get; set; }

    internal class CustomClientSignInOptions
    {
      public string UserAgent { get; set; }

      public string Options { get; set; }
    }
  }
}
