// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TfvcTelemetryInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal static class TfvcTelemetryInfo
  {
    public const string TelemetryEventBaseName = "TFS/Tfvc/";
    public const string CheckInEvent = "TFS/Tfvc/CheckIn";
    public const string VCFilesChangedEvent = "TFS/Tfvc/VCFilesChanged";
    internal const string TelemetryPropertyBaseName = "TFS.SourceControl.";
    public const string KeyCollectionId = "TFS.SourceControl.CollectionId";
    public const string KeyChangesetId = "TFS.SourceControl.ChangesetId";
    public const string KeyFileExtensionChangeCounts = "TFS.SourceControl.FileExtensionChangeCounts";
  }
}
