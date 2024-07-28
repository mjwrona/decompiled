// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ShelvesetVersionSpec
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Server;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class ShelvesetVersionSpec : VersionSpec
  {
    public static readonly char Identifier = 'S';

    public ShelvesetVersionSpec(string shelvesetName, string shelvesetOwner)
    {
      this.Name = shelvesetName;
      this.Owner = shelvesetOwner;
    }

    public override string ToDBString(IVssRequestContext requestContext) => ShelvesetVersionSpec.Identifier.ToString() + this.Name + ";" + this.Owner;

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
    }

    public override string ToString() => ShelvesetVersionSpec.Identifier.ToString() + this.Name + ";" + this.Owner;

    public string ToShelvesetVersion() => this.Name + ";" + this.Owner;

    public string Owner { get; set; }

    public string Name { get; set; }
  }
}
