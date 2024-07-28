// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ConfigurationConstants
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class ConfigurationConstants
  {
    public static readonly string ReleaseManifestFileName = "ReleaseManifest.xml";
    public static readonly string ServicingDataAssemblyName = "Microsoft.VisualStudio.Services.Sdk.Servicing.dll";

    public static class ReturnCodes
    {
      public const int SUCCESS = 0;
      public const int FAIL = 1;
      public const int ERROR_RETRY = 1237;
      public const int ERROR_SUCCESS_REBOOT_REQUIRED = 3010;
      public const int ERROR_SUCCESS_RESTART_REQUIRED = 3011;
      public const int ERROR_FAIL_REBOOT_REQUIRED = 3017;
      public const int ERROR_FAIL_REBOOT_INITIATED = 3018;
      public const int ERROR_FAIL_AND_RETRIABLE = -2146498302;
    }
  }
}
