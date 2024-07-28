// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ConnectOptions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  [Flags]
  public enum ConnectOptions
  {
    [EnumMember] None = 0,
    [EnumMember] IncludeServices = 1,
    [EnumMember] IncludeLastUserAccess = 2,
    [EnumMember, EditorBrowsable(EditorBrowsableState.Never)] IncludeInheritedDefinitionsOnly = 4,
    [EnumMember, EditorBrowsable(EditorBrowsableState.Never)] IncludeNonInheritedDefinitionsOnly = 8,
  }
}
