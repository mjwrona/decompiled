// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRefUpdate
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitRefUpdate : VersionControlSecuredObject
  {
    [DataMember(Name = "repositoryId", EmitDefaultValue = false)]
    public Guid RepositoryId { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "oldObjectId", EmitDefaultValue = false)]
    public string OldObjectId { get; set; }

    [DataMember(Name = "newObjectId", EmitDefaultValue = false)]
    public string NewObjectId { get; set; }

    [DataMember(Name = "isLocked", EmitDefaultValue = false)]
    public bool? IsLocked { get; set; }
  }
}
