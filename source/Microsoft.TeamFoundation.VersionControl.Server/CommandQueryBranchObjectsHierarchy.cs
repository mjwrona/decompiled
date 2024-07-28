// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryBranchObjectsHierarchy
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryBranchObjectsHierarchy : CommandQueryBranchObjects
  {
    private RecursionType m_recursion;

    public CommandQueryBranchObjectsHierarchy(
      VersionControlRequestContext context,
      ItemIdentifier item,
      RecursionType recursion)
      : base(context)
    {
      this.Item = item;
      this.m_recursion = recursion;
    }

    protected override ResultCollection ExecuteInternal(VersionedItemComponent db) => db.QueryBranchObjects(this.Item, this.m_recursion);

    protected override void SetItemValidationOptionsInternal() => this.Item.SetValidationOptions(BranchObject.ValidItemOptions, BranchObject.ValidVersionSpecOptions);

    protected override void RecordClientTraceData()
    {
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("serverPath", (object) this.Item?.ItemPathPair.ProjectNamePath);
      ctData.Add("version", (object) this.Item?.Version?.ToString());
      ctData.Add("recursion", (object) this.m_recursion);
      ClientTrace.Publish(this.RequestContext, "QueryBranchObjectsHierarchy", ctData);
    }
  }
}
