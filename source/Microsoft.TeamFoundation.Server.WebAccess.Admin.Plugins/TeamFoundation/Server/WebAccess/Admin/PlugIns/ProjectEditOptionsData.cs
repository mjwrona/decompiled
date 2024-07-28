// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProjectEditOptionsData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  [DataContract]
  public class ProjectEditOptionsData
  {
    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasGenericWritePermission { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasRenamePermission { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasUpdateVisibilityPermission { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ShowOrgVisibilityOption { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ShowPublicVisibilityOption { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsPublicVisibilityOptionEnabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsOrgVisibilityOptionEnabled { get; set; }
  }
}
