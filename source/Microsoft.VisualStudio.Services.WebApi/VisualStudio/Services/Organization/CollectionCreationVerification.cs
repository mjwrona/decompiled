// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionCreationVerification
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DataContract]
  [Flags]
  public enum CollectionCreationVerification
  {
    [EnumMember] Unknown = 0,
    [EnumMember] None = 1,
    [EnumMember] AllowedList = 2,
    [EnumMember] Capcha = 4,
    [EnumMember] RiskEngine = 8,
    [EnumMember] TrustedTenant = 16, // 0x00000010
  }
}
