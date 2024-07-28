// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskAttachmentExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TaskAttachmentExtensions
  {
    public static bool IsDiagnosticLogMetadataFile(this TaskAttachment attachment) => !string.IsNullOrEmpty(attachment.Name) && attachment.Name.StartsWith("diagnostics-", StringComparison.OrdinalIgnoreCase) && attachment.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
  }
}
