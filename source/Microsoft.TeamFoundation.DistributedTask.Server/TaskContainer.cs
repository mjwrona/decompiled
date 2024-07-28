// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskContainer
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskContainer
  {
    public const string ToolType = "DistributedTask";
    public const string ArtifactType = "Container";
    public const string ToolSpecificId = "Definitions";
    public const string TaskDefinitionFolderFormat = "Tasks/{0:N}";
    public const string TaskDefinitionResourceFileFormat = "Tasks/{0:N}/{1}/{2}";
    public static readonly Uri Uri = new Uri("vstfs:///DistributedTask/Container/Definitions");

    internal static string GetTaskDefinitionFolder(Guid taskId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Tasks/{0:N}", (object) taskId);

    internal static string GetTaskDefinitionResourcePath(TaskDefinition task, string resourceName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Tasks/{0:N}/{1}/{2}", (object) task.Id, (object) task.Version, (object) resourceName);
  }
}
