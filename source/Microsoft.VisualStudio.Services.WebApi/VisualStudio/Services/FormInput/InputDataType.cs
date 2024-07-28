// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FormInput.InputDataType
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FormInput
{
  [DataContract]
  public enum InputDataType
  {
    [EnumMember] None = 0,
    [EnumMember] String = 10, // 0x0000000A
    [EnumMember] Number = 20, // 0x00000014
    [EnumMember] Boolean = 30, // 0x0000001E
    [EnumMember] Guid = 40, // 0x00000028
    [EnumMember] Uri = 50, // 0x00000032
  }
}
