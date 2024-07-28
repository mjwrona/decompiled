// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeComponent6
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ClassificationNodeComponent6 : ClassificationNodeComponent5
  {
    public override void MarkRootNodeDeletionStatus(
      Guid projectId,
      Guid changerTfId,
      bool deleted,
      out int nodeSequenceId)
    {
      this.PrepareStoredProcedure("prc_MarkRootClassificationNodeDeletion");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindBoolean("@deleted", deleted);
      this.BindGuid("@changerTfId", changerTfId);
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@eventAuthor", this.Author);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ClassificationNodeComponent5.SequenceIdColumns());
        nodeSequenceId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
    }
  }
}
