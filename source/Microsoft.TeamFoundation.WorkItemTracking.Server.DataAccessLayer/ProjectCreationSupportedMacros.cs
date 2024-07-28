// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProjectCreationSupportedMacros
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ProjectCreationSupportedMacros
  {
    public static readonly string ProjectName = "$$PROJECTNAME$$";
    public static readonly string ProjectAdministratorsGroupName = "$$PROJECTADMINGROUP$$";
    public static readonly string ProjectCollectionAdministratorsGroupName = "$$PROJECTCOLLECTIONADMINGROUP$$";
    public static readonly string ProjectCollectionAdministratorsCompatibilityGroupName = "$$TEAMFOUNDATIONADMINGROUP$$";
    public static readonly string CreatorOwner = "$$CREATOR_OWNER$$";
  }
}
