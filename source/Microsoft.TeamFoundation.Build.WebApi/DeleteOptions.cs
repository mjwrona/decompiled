// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DeleteOptions
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public enum DeleteOptions
  {
    [EnumMember] None = 0,
    [EnumMember] DropLocation = 1,
    [EnumMember] TestResults = 2,
    [EnumMember] Label = 4,
    [EnumMember] Details = 8,
    [EnumMember] Symbols = 16, // 0x00000010
    [EnumMember] All = 31, // 0x0000001F
  }
}
