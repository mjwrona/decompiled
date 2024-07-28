// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.NsBundleInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Native.Mac;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class NsBundleInformationProvider : INsBundleInformationProvider
  {
    public string GetVersion() => MacFoundation.NSBundle.GetVersion();

    public string GetName() => MacFoundation.NSBundle.GetBundleName();
  }
}
