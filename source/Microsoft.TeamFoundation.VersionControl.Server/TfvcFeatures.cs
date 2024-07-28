// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TfvcFeatures
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TfvcFeatures
  {
    public const string IgnoreInvalidShelvesetName = "SourceControl.IgnoreInvalidShelvesetName";
    public const string BlockQueryItemsExtendedFullRecursion = "SourceControl.BlockQueryItemsExtendedFullRecursion";
    public const string ServiceBusEventsPublishAsync = "SourceControl.ServiceBusEventsPublishAsync";
    public const string ProjectScopedPaths = "Tfvc.ProjectScopedPaths";
    public const string SelfThrottle = "Tfvc.SelfThrottle";
    public const string ItemUriUsePath = "Tfvc.ItemUriUsePath";
    public const string UsePendMergeWithRecompile = "Tfvc.UsePendMergeWithRecompile";
    public const string DisableOldTfvcCheckinPolicies = "SourceControl.DisableOldTfvcCheckinPolicies";
    public const string SourceControlNewToggleToSwitchOldCheckinPoliciesSaving = "SourceControl.NewToggleToSwitchOldCheckinPoliciesSaving";
  }
}
