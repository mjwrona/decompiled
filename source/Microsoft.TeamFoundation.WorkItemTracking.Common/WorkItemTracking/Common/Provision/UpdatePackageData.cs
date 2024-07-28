// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackageData
  {
    private IMetadataProvisioningHelper m_metadata;
    private int m_projectId;
    private string m_projectGuid;
    private GlobalListsCollection m_globalLists;
    private string m_methodologyName;
    private MetaIDFactory m_metaIdFactory;
    public MetaID CurrentTypeId;

    public UpdatePackageData(
      IMetadataProvisioningHelper metadata,
      MetaIDFactory metaIdFactory,
      int projectId,
      string methodologyName)
    {
      this.m_metadata = metadata;
      this.m_projectId = projectId;
      this.m_metaIdFactory = metaIdFactory;
      this.m_methodologyName = UpdatePackageData.GetNormalizedMethodologyName(methodologyName);
      this.m_globalLists = new GlobalListsCollection(this.m_metadata);
    }

    public static string GetNormalizedMethodologyName(string name)
    {
      if (!string.IsNullOrEmpty(name))
        name = Regex.Replace(name, "[\\.\\[\\]]", "_");
      return name;
    }

    public IMetadataProvisioningHelper Metadata => this.m_metadata;

    public int ProjectId => this.m_projectId;

    public GlobalListsCollection GlobalLists => this.m_globalLists;

    public string ProjectGuid
    {
      get
      {
        if (this.m_projectGuid == null)
          this.m_projectGuid = this.m_projectId != 0 ? (this.m_projectId != -1 ? this.Metadata.GetNodeGuid(this.m_projectId) + "\\" : ProvisionValues.ConstScopeProject) : string.Empty;
        return this.m_projectGuid;
      }
    }

    public string MethodologyName => this.m_methodologyName;

    public MetaIDFactory MetaIDFactory => this.m_metaIdFactory;
  }
}
