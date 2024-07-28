// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LatestVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LatestVersionSpec : VersionSpec
  {
    public override int ToChangeset(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = requestContext.GetService<TeamFoundationVersionControlService>().GetVersionControlRequestContext(requestContext);
      return controlRequestContext.VersionControlService.GetLatestChangeset(controlRequestContext);
    }

    public override string ToDBString(IVssRequestContext requestContext) => "T";

    public override string ToString() => "T";

    public static string DisplayString => "T";

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
    }
  }
}
