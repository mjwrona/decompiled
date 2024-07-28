// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.GitRepositoryData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class GitRepositoryData
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "projectName")]
    public string ProjectName { get; set; }

    [DataMember(Name = "projectId")]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "isdisabled")]
    public bool isdisabled { get; set; }

    public override string ToString() => string.Format("Repository: {0} ({1},{2}), Project: {3} ({4})", (object) this.Name, (object) this.Id.ToString(), (object) this.isdisabled.ToString(), (object) this.ProjectName, (object) this.ProjectId.ToString());
  }
}
