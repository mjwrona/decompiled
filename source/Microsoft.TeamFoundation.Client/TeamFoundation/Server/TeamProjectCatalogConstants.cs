// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.TeamProjectCatalogConstants
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  public static class TeamProjectCatalogConstants
  {
    public static readonly Guid ResourceType = new Guid("48577A4A-801E-412c-B8AE-CF7EF3529616");
    public const string ProjectNameProperty = "ProjectName";
    public const string ProjectUriProperty = "ProjectUri";
    public const string ProjectIdProperty = "ProjectId";
    public const string ProjectStateProperty = "ProjectState";
    public const string SourceControlCapabilityFlags = "SourceControlCapabilityFlags";
    public const string SourceControlGitEnabled = "SourceControlGitEnabled";
    public const string SourceControlTfvcEnabled = "SourceControlTfvcEnabled";
  }
}
