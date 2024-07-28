// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ArtifactLinkIds
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation
{
  public static class ArtifactLinkIds
  {
    public static readonly string Changeset = "Fixed in Changeset";
    public static readonly string Commit = "Fixed in Commit";
    public static readonly string VersionedItem = "Source Code File";
    public static readonly string NoOutboundLink = "No Outbound Link";
    public static readonly string PullRequest = "Pull Request";
    public static readonly string Branch = nameof (Branch);
    public static readonly string FoundInBuild = "Found in build";
    public static readonly string IntegratedInBuild = "Integrated in build";
  }
}
