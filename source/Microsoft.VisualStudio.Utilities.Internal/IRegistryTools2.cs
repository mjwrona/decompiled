// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.IRegistryTools2
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public interface IRegistryTools2 : IRegistryTools
  {
    string[] GetRegistryValueNamesFromCurrentUserRoot(string regKeyPath);

    string[] GetRegistryValueNamesFromLocalMachineRoot(string regKeyPath, bool use64Bit = false);

    string[] GetRegistrySubKeyNamesFromCurrentUserRoot(string regKeyPath);

    string[] GetRegistrySubKeyNamesFromLocalMachineRoot(string regKeyPath, bool use64Bit = false);

    bool DoesRegistryKeyExistInCurrentUserRoot(string regKeyPath);

    bool DoesRegistryKeyExistInLocalMachineRoot(string regKeyPath, bool use64Bit = false);

    bool DeleteRegistryKeyFromCurrentUserRoot(string regKeyPath);

    bool DeleteRegistryKeyFromLocalMachineRoot(string regKeyPath, bool use64Bit = false);

    bool DeleteRegistryValueFromCurrentUserRoot(string regKeyPath, string regKeyName);

    bool DeleteRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit = false);
  }
}
