// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.UpstreamSource
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class UpstreamSource : FeedSecuredObject, IEquatable<UpstreamSource>
  {
    [DataMember]
    public virtual Guid Id { get; set; }

    [DataMember]
    public virtual string Name { get; set; }

    [DataMember]
    public virtual string Protocol { get; set; }

    [DataMember]
    public virtual string Location { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public virtual string DisplayLocation { get; set; }

    [DataMember]
    public virtual UpstreamSourceType UpstreamSourceType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? InternalUpstreamCollectionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? InternalUpstreamFeedId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? InternalUpstreamViewId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? InternalUpstreamProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ServiceEndpointId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ServiceEndpointProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeletedDate { get; set; }

    [DataMember]
    public UpstreamStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<UpstreamStatusDetail> StatusDetails { get; set; }

    public bool Equals(UpstreamSource other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return string.Equals(this.Name, other.Name) && StringComparer.OrdinalIgnoreCase.Equals(this.Protocol, other.Protocol) && UpstreamSource.LocationEquals(this.Location, other.Location) && this.UpstreamSourceType == other.UpstreamSourceType;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return obj is UpstreamSource other && this.Equals(other);
    }

    public override int GetHashCode() => (int) ((UpstreamSourceType) ((((this.Name != null ? this.Name.GetHashCode() : 0) * 397 ^ (this.Protocol != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Protocol) : 0)) * 397 ^ (this.Location != null ? UpstreamSource.GetLocationHashCode(this.Location) : 0)) * 397) ^ this.UpstreamSourceType);

    public static IEqualityComparer<UpstreamSource> ProtocolSourceTypeLocationComparer { get; } = (IEqualityComparer<UpstreamSource>) new UpstreamSource.UpstreamProtocolSourceTypeLocationComparer();

    public override string ToString() => string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}, {10}: {11}", (object) "Id", (object) this.Id, (object) "Name", (object) this.Name, (object) "Protocol", (object) this.Protocol, (object) "Location", (object) this.Location, (object) "UpstreamSourceType", (object) this.UpstreamSourceType, (object) "DeletedDate", (object) this.DeletedDate);

    private static bool LocationEquals(string x, string y) => Uri.IsWellFormedUriString(x, UriKind.Absolute) && Uri.IsWellFormedUriString(y, UriKind.Absolute) ? new Uri(x) == new Uri(y) : string.Equals(x, y);

    private static int GetLocationHashCode(string location) => Uri.IsWellFormedUriString(location, UriKind.Absolute) ? new Uri(location).GetHashCode() : location.GetHashCode();

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [IgnoreDataMember]
    public string FeatureFlag
    {
      get => (string) null;
      set => throw new NotSupportedException();
    }

    private sealed class UpstreamProtocolSourceTypeLocationComparer : 
      IEqualityComparer<UpstreamSource>
    {
      public bool Equals(UpstreamSource x, UpstreamSource y)
      {
        if (x == y)
          return true;
        return x != null && y != null && StringComparer.OrdinalIgnoreCase.Equals(x.Protocol, y.Protocol) && UpstreamSource.LocationEquals(x.Location, y.Location) && x.UpstreamSourceType == y.UpstreamSourceType;
      }

      public int GetHashCode(UpstreamSource obj) => (int) ((UpstreamSourceType) (((obj.Protocol != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Protocol) : 0) * 397 ^ (obj.Location != null ? UpstreamSource.GetLocationHashCode(obj.Location) : 0)) * 397) ^ obj.UpstreamSourceType);
    }
  }
}
