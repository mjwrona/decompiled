// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.IRegistryTools
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public interface IRegistryTools
  {
    int? GetRegistryIntValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      int? defaultOnError = null);

    int? GetRegistryIntValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit,
      int? defaultOnError = null);

    object GetRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null);

    object GetRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit,
      object defaultOnError = null);

    object GetRegistryValueFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null);

    bool SetRegistryFromCurrentUserRoot(string regKeyPath, string regKeyName, object value);

    bool SetRegistryFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      object value,
      bool use64Bit = false);
  }
}
