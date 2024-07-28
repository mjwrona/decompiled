// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommercePackageSettings
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommercePackageSettings
  {
    public static string UpdateRegistrySpsUrl = "Upd-Reg-SpsUrl";
    public static string UpdateRegistryAuthorizationUrl = "Upd-Reg-AuthUrl";
    public static string UpdateRegistryAccountId = "Upd-Reg-AccountId";
    internal static IDictionary<string, string> UpdateRegistryMap = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        CommercePackageSettings.UpdateRegistrySpsUrl,
        "/Configuration/ConnectedServer/SpsUrl"
      },
      {
        CommercePackageSettings.UpdateRegistryAuthorizationUrl,
        "/Configuration/ConnectedServer/Authorization/Url"
      },
      {
        CommercePackageSettings.UpdateRegistryAccountId,
        "/Configuration/ConnectedServer/AccountId"
      }
    };
  }
}
