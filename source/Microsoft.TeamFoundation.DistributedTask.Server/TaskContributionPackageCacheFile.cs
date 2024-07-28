// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskContributionPackageCacheFile
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class TaskContributionPackageCacheFile : VirtualFileInformation
  {
    public TaskContributionPackageCacheFile(
      IVssRequestContext requestContext,
      ExtensionIdentifier extensionId,
      string version)
      : base(requestContext.ServiceHost.DeploymentServiceHost.InstanceId, TaskContributionPackageCacheFile.GetFileName(extensionId, version), "DistributedTask")
    {
      this.ExtensionId = extensionId;
      this.Version = version;
      this.ContentType = "application/octet-stream";
    }

    public ExtensionIdentifier ExtensionId { get; }

    public string Version { get; }

    internal static string GetFileName(ExtensionIdentifier extensionId, string version) => TimelineRecordIdGenerator.GetId(string.Format("{0}-{1}", (object) extensionId, (object) version)).ToString("N").ToUpperInvariant() + ".vsixcache";
  }
}
