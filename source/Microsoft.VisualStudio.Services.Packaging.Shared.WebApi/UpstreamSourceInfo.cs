// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamSourceInfo
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  [ClientIncludeModel]
  public class UpstreamSourceInfo : PackagingSecuredObject
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Location { get; set; }

    [DataMember]
    public string DisplayLocation { get; set; }

    [DataMember]
    public PackagingSourceType SourceType { get; set; }

    public override bool Equals(object obj)
    {
      UpstreamSourceInfo upstreamSourceInfo = obj as UpstreamSourceInfo;
      return this.Id.Equals((object) upstreamSourceInfo?.Id) && this.SourceType.Equals((object) upstreamSourceInfo?.SourceType);
    }

    public override int GetHashCode() => this.Id.GetHashCode() ^ this.SourceType.GetHashCode();
  }
}
