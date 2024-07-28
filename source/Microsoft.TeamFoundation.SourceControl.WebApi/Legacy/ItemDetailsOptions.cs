// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class ItemDetailsOptions : VersionControlSecuredObject
  {
    public ItemDetailsOptions(
      bool? includeChildren,
      bool? includeContentMetadata,
      bool? includeVersionDescription)
      : this()
    {
      if (includeChildren.GetValueOrDefault(false))
        this.RecursionLevel = VersionControlRecursionType.OneLevel;
      this.IncludeContentMetadata = includeContentMetadata.GetValueOrDefault(false);
      this.IncludeVersionDescription = includeVersionDescription.GetValueOrDefault(false);
    }

    public ItemDetailsOptions() => this.RecursionLevel = VersionControlRecursionType.None;

    public ItemDetailsOptions(ISecuredObject securedObject)
      : base(securedObject)
    {
      this.RecursionLevel = VersionControlRecursionType.None;
    }

    [DataMember(Name = "recursionLevel", EmitDefaultValue = false)]
    public VersionControlRecursionType RecursionLevel { get; set; }

    [DataMember(Name = "includeContentMetadata", EmitDefaultValue = false)]
    public bool IncludeContentMetadata { get; set; }

    [DataMember(Name = "includeVersionDescription", EmitDefaultValue = false)]
    public bool IncludeVersionDescription { get; set; }

    public long ScanBytesForEncoding { get; set; }
  }
}
