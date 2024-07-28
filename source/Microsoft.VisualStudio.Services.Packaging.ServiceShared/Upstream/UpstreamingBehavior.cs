// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class UpstreamingBehavior : IEquatable<UpstreamingBehavior>
  {
    public static readonly UpstreamingBehavior AllowExternalVersions = new UpstreamingBehavior(UpstreamVersionVisibility.AllowExternalVersions);
    public static readonly UpstreamingBehavior Auto = new UpstreamingBehavior(UpstreamVersionVisibility.Auto);

    public UpstreamingBehavior(
      UpstreamVersionVisibility versionsFromExternalUpstreams)
    {
      this.VersionsFromExternalUpstreams = versionsFromExternalUpstreams;
    }

    public UpstreamVersionVisibility VersionsFromExternalUpstreams { get; }

    public bool Equals(UpstreamingBehavior other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.VersionsFromExternalUpstreams == other.VersionsFromExternalUpstreams;
    }

    public override bool Equals(object obj) => this.Equals(obj as UpstreamingBehavior);

    public static bool operator ==(UpstreamingBehavior e1, UpstreamingBehavior e2) => (object) e1 == null ? (object) e2 == null : e1.Equals(e2);

    public static bool operator !=(UpstreamingBehavior e1, UpstreamingBehavior e2) => !(e1 == e2);

    public override int GetHashCode() => this.VersionsFromExternalUpstreams.GetHashCode();

    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior ToWebApi() => new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior()
    {
      VersionsFromExternalUpstreams = this.VersionsFromExternalUpstreams
    };
  }
}
