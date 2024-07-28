// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitLfsLockCollection
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [ClientIgnore]
  public class GitLfsLockCollection
  {
    [DataMember(Name = "locks", EmitDefaultValue = false)]
    public IEnumerable<GitLfsLock> Locks { get; set; }

    [DataMember(Name = "ours", EmitDefaultValue = false)]
    public IEnumerable<GitLfsLock> Ours { get; set; }

    [DataMember(Name = "theirs", EmitDefaultValue = false)]
    public IEnumerable<GitLfsLock> Theirs { get; set; }

    [DataMember(Name = "next_cursor", EmitDefaultValue = false)]
    public string NextCursor { get; set; }
  }
}
