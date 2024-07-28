// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Enums.RejectionCode
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Enums
{
  [DataContract]
  public enum RejectionCode
  {
    [EnumMember] DEFAULT = 0,
    [EnumMember] PERSISTENT = 1,
    [EnumMember] LANGUAGEPARSERCRASHED = 2,
    [EnumMember] TEXTPARSERCRASHED = 3,
    [EnumMember] PARSEROTHER = 255, // 0x000000FF
  }
}
