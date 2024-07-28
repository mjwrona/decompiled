// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform.PlatformSingleton
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform
{
  internal static class PlatformSingleton
  {
    private static IPlatform current;

    public static IPlatform Current
    {
      get => PlatformSingleton.current ?? (PlatformSingleton.current = (IPlatform) new PlatformImplementation());
      set => PlatformSingleton.current = value;
    }
  }
}
