// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.FolderCoverageData
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class FolderCoverageData : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<FolderCoverageData> Folders { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<FileCoverageData> Files { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CoverageStatistics CoverageStatistics { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.CoverageStatistics != null)
        this.CoverageStatistics.InitializeSecureObject(securedObject);
      if (this.Folders != null && this.Folders.Count > 0)
      {
        foreach (TestManagementBaseSecuredObject folder in this.Folders)
          folder.InitializeSecureObject(securedObject);
      }
      if (this.Files == null || this.Files.Count <= 0)
        return;
      foreach (TestManagementBaseSecuredObject file in this.Files)
        file.InitializeSecureObject(securedObject);
    }
  }
}
