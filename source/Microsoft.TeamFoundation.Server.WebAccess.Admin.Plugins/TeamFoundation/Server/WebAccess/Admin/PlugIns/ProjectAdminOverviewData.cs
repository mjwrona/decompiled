// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProjectAdminOverviewData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  [DataContract]
  public class ProjectAdminOverviewData
  {
    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ProcessTemplateName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ProjectVisibility { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProjectEditOptionsData EditProjectOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProjectDeleteOptionsData DeleteProjectOptions { get; set; }
  }
}
