// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Folder
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class Folder : ISecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastChangedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef LastChangedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TeamProjectReference Project { get; set; }

    public Guid NamespaceId
    {
      get
      {
        ArgumentUtility.CheckForNull<TeamProjectReference>(this.Project, "Project");
        return ((ISecuredObject) this.Project).NamespaceId;
      }
    }

    public int RequiredPermissions
    {
      get
      {
        ArgumentUtility.CheckForNull<TeamProjectReference>(this.Project, "Project");
        return ((ISecuredObject) this.Project).RequiredPermissions;
      }
    }

    public string GetToken()
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(this.Project, "Project");
      return ((ISecuredObject) this.Project).GetToken();
    }
  }
}
