// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  [ClientIncludeModel]
  public class UpstreamingBehavior : PackagingSecuredObject, IEquatable<UpstreamingBehavior>
  {
    [DataMember]
    public UpstreamVersionVisibility VersionsFromExternalUpstreams { get; set; }

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
  }
}
