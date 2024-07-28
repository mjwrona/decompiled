// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Constants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct Constants
  {
    internal const string workItemChangedEvent = "WorkItemChangedEvent";
    internal const string bisBackendDatabaseName = "WIT DB";
    internal const string bisAttachmentDatabaseName = "WITAttachments DB";
    internal const string bisInstanceId = "InstanceId";
    internal const string bisEntryType = "vstfs";
    internal const string requestIdParameter = "requestId";
    internal const string contextParameter = "context";
    internal const string userParameter = "user";
    internal const string fileName = "FileName";
    internal const string projectURI = "ProjectURI";
    internal const string artifactURI = "ArtifactURI";
    internal const string artifactUris = "ArtifactUris";
    internal const string uuidScheme = "uuid:";
    internal const int EventProcessTimerInterval = 2000;
    internal const int ProcessRetryCountMax = 3;
    internal const string ExceptionHeader = "X-WorkItemTracking-Exception";
  }
}
