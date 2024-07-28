// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BaseLocalVersionUpdate
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public abstract class BaseLocalVersionUpdate : IValidatable
  {
    private string m_targetLocalItem;
    private int m_localVersion;

    [XmlAttribute("tlocal")]
    public string TargetLocalItem
    {
      get => this.m_targetLocalItem;
      set => this.m_targetLocalItem = value;
    }

    [XmlAttribute("lver")]
    public int LocalVersion
    {
      get => this.m_localVersion;
      set => this.m_localVersion = value;
    }

    internal static void UpdateLocalVersion(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      BaseLocalVersionUpdate[] updates)
    {
      versionControlRequestContext.Validation.check((IValidatable[]) updates, nameof (updates), false);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        versionedItemComponent.UpdateLocalVersion(workspace, updates, versionControlRequestContext.MaxSupportedServerPathLength);
    }

    internal abstract void SetRecord(SqlDataRecord record);

    internal abstract void SetRecord3(SqlDataRecord record);

    internal abstract void SetRecord4(SqlDataRecord record);

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      this.ValidateInternal(versionControlRequestContext, parameterName);
    }

    internal virtual void ValidateInternal(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkLocalItem(this.m_targetLocalItem, parameterName + ".TargetLocalItem", true, false, true, false);
    }
  }
}
