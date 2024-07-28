// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryBranchObjectsByPath
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryBranchObjectsByPath : CommandQueryBranchObjects
  {
    private VersionSpec m_version;

    public CommandQueryBranchObjectsByPath(
      VersionControlRequestContext context,
      ItemIdentifier item,
      VersionSpec version)
      : base(context)
    {
      this.Item = item;
      this.m_version = version;
    }

    protected override ResultCollection ExecuteInternal(VersionedItemComponent db) => db.QueryBranchObjectsByPath(this.ItemPathPair, this.m_version);

    protected override void SetItemValidationOptionsInternal() => this.Item.SetValidationOptions(ItemValidationOptions.Allow8Dot3Paths | ItemValidationOptions.DisallowLocalItem, BranchObject.ValidVersionSpecOptions);

    protected override void RecordClientTraceData()
    {
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("serverPath", (object) this.Item?.ItemPathPair.ProjectNamePath);
      ctData.Add("version", (object) this.m_version.ToString());
      ClientTrace.Publish(this.RequestContext, "QueryBranchObjectsByPath", ctData);
    }
  }
}
