// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FormInput.InputMode
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FormInput
{
  [DataContract]
  public enum InputMode
  {
    [EnumMember] None = 0,
    [EnumMember] TextBox = 10, // 0x0000000A
    [EnumMember] PasswordBox = 20, // 0x00000014
    [EnumMember] Combo = 30, // 0x0000001E
    [EnumMember] RadioButtons = 40, // 0x00000028
    [EnumMember] CheckBox = 50, // 0x00000032
    [EnumMember] TextArea = 60, // 0x0000003C
  }
}
