// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskDefinitionRevisionData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class MetaTaskDefinitionRevisionData
  {
    internal Guid DefinitionId { get; set; }

    internal int Revision { get; set; }

    internal int MajorVersion { get; set; }

    internal Guid ChangedBy { get; set; }

    internal DateTime ChangedDate { get; set; }

    internal AuditAction ChangeType { get; set; }

    internal int FileId { get; set; }

    internal string Comment { get; set; }

    internal TaskGroupRevision GetRevision() => new TaskGroupRevision()
    {
      TaskGroupId = this.DefinitionId,
      Revision = this.Revision,
      MajorVersion = this.MajorVersion,
      ChangedBy = new IdentityRef()
      {
        Id = this.ChangedBy.ToString()
      },
      ChangedDate = this.ChangedDate,
      ChangeType = this.ChangeType,
      FileId = this.FileId,
      Comment = this.Comment
    };
  }
}
