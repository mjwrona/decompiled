// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ClassificationNodeComponentV4
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ClassificationNodeComponentV4 : ClassificationNodeComponentV3
  {
    public ClassificationNodeComponentV4()
    {
    }

    internal ClassificationNodeComponentV4(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override ClassificationNode GetClassificationNode(int nodeId)
    {
      this.PrepareStoredProcedure("Search.prc_QueryClassificationNodeById");
      this.BindInt("@nodeId", nodeId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNode>((ObjectBinder<ClassificationNode>) new ClassificationNodeComponent.ClassificationNodeColumns());
        ObjectBinder<ClassificationNode> current = resultCollection.GetCurrent<ClassificationNode>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items[0] : (ClassificationNode) null;
      }
    }
  }
}
