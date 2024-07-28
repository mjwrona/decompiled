// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.PreviousVersionSpec
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class PreviousVersionSpec : VersionSpec
  {
    public static readonly char Identifier = 'P';

    public PreviousVersionSpec(VersionSpec version) => this.Version = version != null ? version : throw new ArgumentNullException(nameof (version));

    public PreviousVersionSpec(string version) => this.Version = TfsModelExtensions.ParseVersionSpecString(version, (VersionSpec) new LatestVersionSpec());

    public VersionSpec Version { get; set; }

    public override string ToDBString(IVssRequestContext requestContext) => PreviousVersionSpec.Identifier.ToString() + this.Version.ToDBString(requestContext);

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
    }

    public override string ToString() => PreviousVersionSpec.Identifier.ToString() + this.Version.ToString();
  }
}
