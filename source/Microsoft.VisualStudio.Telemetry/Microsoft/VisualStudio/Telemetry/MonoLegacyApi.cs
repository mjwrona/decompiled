// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MonoLegacyApi
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MonoLegacyApi : LegacyApi
  {
    public MonoLegacyApi(IRegistryTools registryTools)
      : base(registryTools)
    {
    }

    protected override string ReadMachineIdFromRegistry(
      IRegistryTools registry,
      string regPath,
      string regKey)
    {
      return (string) registry.GetRegistryValueFromCurrentUserRoot(regPath, regKey);
    }

    protected override bool SaveMachineIdToRegistry(
      IRegistryTools registry,
      string regPath,
      string regKey,
      string machineId)
    {
      return registry.SetRegistryFromCurrentUserRoot(regPath, regKey, (object) machineId);
    }
  }
}
