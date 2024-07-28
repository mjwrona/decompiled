// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ClassificationNodeComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ClassificationNodeComponentV2 : ClassificationNodeComponent
  {
    public ClassificationNodeComponentV2()
    {
    }

    internal ClassificationNodeComponentV2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override int DeleteClassificationNodes(List<int> classificationNodeIds)
    {
      this.ValidateNotNullOrEmptyList<int>(nameof (classificationNodeIds), (IList<int>) classificationNodeIds);
      this.PrepareStoredProcedure("Search.prc_DeleteClassificationNodes");
      this.BindClassificationNodeIdTable("@nodeIdList", (IEnumerable<int>) classificationNodeIds);
      return this.ExecuteNonQuery();
    }
  }
}
