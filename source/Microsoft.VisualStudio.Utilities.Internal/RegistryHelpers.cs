// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.RegistryHelpers
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using Microsoft.Win32;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  internal class RegistryHelpers
  {
    public static bool TryGetRegistryValueKindForSet(
      object value,
      out RegistryValueKind registryValueKind)
    {
      registryValueKind = RegistryValueKind.Unknown;
      switch (value)
      {
        case bool _:
          registryValueKind = RegistryValueKind.DWord;
          return true;
        case int _:
          registryValueKind = RegistryValueKind.DWord;
          return true;
        case long _:
        case ulong _:
          registryValueKind = RegistryValueKind.QWord;
          return true;
        default:
          return false;
      }
    }
  }
}
