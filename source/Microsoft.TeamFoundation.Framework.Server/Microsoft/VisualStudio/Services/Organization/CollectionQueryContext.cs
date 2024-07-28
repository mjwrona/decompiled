// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionQueryContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DataContract]
  public class CollectionQueryContext
  {
    public CollectionQueryContext(
      CollectionSearchKind searchKind,
      string searchValue,
      bool? includeDeletedCollections = null)
    {
      this.SearchKind = searchKind;
      this.SearchValue = searchValue;
      this.IncludeDeletedCollections = includeDeletedCollections;
    }

    [DataMember]
    public CollectionSearchKind SearchKind { get; }

    [DataMember]
    public string SearchValue { get; }

    [DataMember]
    public bool? IncludeDeletedCollections { get; }

    public override string ToString() => !this.IncludeDeletedCollections.HasValue ? string.Format("{0};{1};IncludeDeletedCollections={2}", (object) this.SearchKind, (object) this.SearchValue, (object) this.IncludeDeletedCollections) : string.Format("{0};{1}", (object) this.SearchKind, (object) this.SearchValue);
  }
}
