// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionPolicyFlags
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  [Flags]
  public enum ExtensionPolicyFlags
  {
    [EnumMember] None = 0,
    [EnumMember] Private = 1,
    [EnumMember] Public = 2,
    [EnumMember] Preview = 4,
    [EnumMember] Released = 8,
    [EnumMember] FirstParty = 16, // 0x00000010
    [EnumMember] All = FirstParty | Released | Preview | Public | Private, // 0x0000001F
  }
}
