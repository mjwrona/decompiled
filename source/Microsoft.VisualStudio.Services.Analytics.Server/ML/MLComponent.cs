// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ML.MLComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.ML
{
  public class MLComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<MLComponent>(1)
    }, "MLService");

    public MLComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public ICollection<Tag> PredictWorkItemTags(Guid projectId, int workItemId)
    {
      this.PrepareStoredProcedure("AnalyticsModel.prc_PredictWorkItemTags");
      this.BindGuid("@projectId", projectId);
      this.BindInt("@workItemId", workItemId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tag>((ObjectBinder<Tag>) new TagColumns());
        return (ICollection<Tag>) resultCollection.GetCurrent<Tag>().Items;
      }
    }
  }
}
