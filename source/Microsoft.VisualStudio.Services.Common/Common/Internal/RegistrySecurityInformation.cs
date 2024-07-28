// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.RegistrySecurityInformation
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [Flags]
  internal enum RegistrySecurityInformation : uint
  {
    Owner = 1,
    Group = 2,
    Dacl = 4,
    Sacl = 8,
    ProtectedDacl = 2147483648, // 0x80000000
    ProtectedSacl = 1073741824, // 0x40000000
    UnprotectedDacl = 536870912, // 0x20000000
    UnprotectedSacl = 268435456, // 0x10000000
  }
}
