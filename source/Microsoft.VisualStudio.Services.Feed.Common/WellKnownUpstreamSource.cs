// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.WellKnownUpstreamSource
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Common
{
  [DataContract]
  public sealed class WellKnownUpstreamSource : IEquatable<WellKnownUpstreamSource>
  {
    public WellKnownUpstreamSource(
      string protocol,
      WellKnownSources.Tag tag,
      string displayName,
      Uri location)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(protocol, nameof (protocol));
      ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (displayName));
      ArgumentUtility.CheckForNull<Uri>(location, nameof (location));
      if (location.Scheme != Uri.UriSchemeHttps)
        throw new ArgumentException("Public upstream sources must be specified with https://");
      this.Tag = tag;
      this.TagName = this.Tag.ToString();
      this.DisplayName = displayName;
      this.Location = location;
      this.Protocol = protocol;
      this.LocationUriString = this.Location.AbsoluteUri;
    }

    public WellKnownSources.Tag Tag { get; }

    public string TagName { get; }

    [DataMember]
    public string DisplayName { get; }

    [DataMember]
    public Uri Location { get; }

    [IgnoreDataMember]
    public string LocationUriString { get; }

    [DataMember]
    public string Protocol { get; }

    [IgnoreDataMember]
    public string? FeatureFlag { get; init; }

    public UpstreamSource ToUpstreamSource(Guid? id = null, string? name = null) => new UpstreamSource()
    {
      Id = !id.HasValue || !(id.Value != Guid.Empty) ? Guid.NewGuid() : id.Value,
      Protocol = this.Protocol,
      Name = string.IsNullOrWhiteSpace(name) ? this.DisplayName : name,
      Location = this.LocationUriString,
      DisplayLocation = this.LocationUriString,
      UpstreamSourceType = UpstreamSourceType.Public,
      InternalUpstreamCollectionId = new Guid?(),
      InternalUpstreamFeedId = new Guid?(),
      InternalUpstreamViewId = new Guid?(),
      InternalUpstreamProjectId = new Guid?(),
      ServiceEndpointId = new Guid?(),
      ServiceEndpointProjectId = new Guid?(),
      DeletedDate = new DateTime?(),
      Status = UpstreamStatus.Ok,
      StatusDetails = (IEnumerable<UpstreamStatusDetail>) null
    };

    public bool Equals(WellKnownUpstreamSource? other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || this.Location == other.Location;
    }

    public override bool Equals(object? obj)
    {
      if ((object) this == obj)
        return true;
      WellKnownUpstreamSource other = obj as WellKnownUpstreamSource;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode() => this.Location.GetHashCode();

    public static bool operator ==(WellKnownUpstreamSource? left, WellKnownUpstreamSource? right) => object.Equals((object) left, (object) right);

    public static bool operator !=(WellKnownUpstreamSource? left, WellKnownUpstreamSource? right) => !object.Equals((object) left, (object) right);

    public UpstreamMatchValidationResult ValidateUpstreamSourceMatches(UpstreamSource source)
    {
      UpstreamStatus Status = UpstreamStatus.Ok;
      List<UpstreamStatusDetail> StatusDetails = new List<UpstreamStatusDetail>();
      if (!StringComparer.OrdinalIgnoreCase.Equals(this.Protocol, source.Protocol))
      {
        Status = UpstreamStatus.Disabled;
        StatusDetails.Add(new UpstreamStatusDetail()
        {
          Reason = Resources.Error_WellKnownUpstreamWrongProtocol((object) (source.DisplayLocation ?? source.Location), (object) this.DisplayName, (object) this.Protocol, (object) source.Protocol)
        });
      }
      return new UpstreamMatchValidationResult(Status, (IEnumerable<UpstreamStatusDetail>) StatusDetails);
    }
  }
}
