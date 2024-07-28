// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract(Name = "VersionControlType")]
  public enum VersionControlType
  {
    [EnumMember] Git,
    [EnumMember] Tfvc,
    [EnumMember] Custom,
  }
}
