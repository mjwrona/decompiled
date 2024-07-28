// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.AadSettings
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.ClientStorage;

namespace Microsoft.TeamFoundation.Client
{
  internal static class AadSettings
  {
    public const string DefaultAadInstance = "https://login.microsoftonline.com/";
    public const string CommonTenant = "common";
    public const string Resource = "499b84ac-1321-427f-aa17-267ca6975798";
    public const string Client = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";

    public static string AadInstance
    {
      get
      {
        string aadInstance = AadSettings.GetAadInstanceOverrideFromRegistry();
        if (string.IsNullOrWhiteSpace(aadInstance))
          aadInstance = "https://login.microsoftonline.com/";
        else if (!aadInstance.EndsWith("/"))
          aadInstance += "/";
        return aadInstance;
      }
    }

    private static string GetAadInstanceOverrideFromRegistry()
    {
      string overrideFromRegistry = (string) null;
      try
      {
        overrideFromRegistry = VssClientStorage.CurrentUserSettings.ReadEntry<string>(VssClientStorage.CurrentUserSettings.PathSeparator.ToString() + VssClientStorage.CurrentUserSettings.PathKeyCombine("ConnectedUser", "AadInstance"), (string) null);
      }
      catch
      {
      }
      return overrideFromRegistry;
    }
  }
}
