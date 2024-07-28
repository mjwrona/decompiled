// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.TeamProjectPropertyConstants
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using System;

namespace Microsoft.Azure.Devops.Teams.Service
{
  public static class TeamProjectPropertyConstants
  {
    internal static readonly Guid ArtifactKindId = new Guid("E08B03C0-7EBB-440D-BDD5-52E028566E77");
    internal static readonly string ArtifactKindDescription = "Project Properties";
    internal static readonly byte[] ArtifactId = Array.Empty<byte>();
    public const string SourceControlCapabilityFlags = "System.SourceControlCapabilityFlags";
    public const string SourceControlGitEnabled = "System.SourceControlGitEnabled";
    public const string SourceControlTfvcEnabled = "System.SourceControlTfvcEnabled";
    public const string SourceControlGitPermissionsInitialized = "System.SourceControlGitPermissionsInitialized";
    public const string IsProjectPreCreated = "System.ProjectPreCreated";
    public const string SoftDeletedProjectName = "System.SoftDeletedProjectName";
    public const string SoftDeletedTimestamp = "System.SoftDeletedTimestamp";
  }
}
