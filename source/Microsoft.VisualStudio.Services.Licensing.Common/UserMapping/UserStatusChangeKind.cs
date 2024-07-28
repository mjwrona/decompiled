// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.UserStatusChangeKind
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  [DataContract]
  public enum UserStatusChangeKind
  {
    Enabled = 10, // 0x0000000A
    Disabled = 20, // 0x00000014
  }
}
