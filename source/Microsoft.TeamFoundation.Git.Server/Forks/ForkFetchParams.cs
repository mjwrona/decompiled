// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Forks.ForkFetchParams
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Forks
{
  [DataContract]
  public class ForkFetchParams
  {
    [JsonConstructor]
    internal ForkFetchParams(
      GlobalGitRepositoryKey source,
      Guid targetRepoId,
      List<SourceToTargetRef> sourceToTargetRefs,
      bool copySourceRepoDefaults)
    {
      ArgumentUtility.CheckForNull<GlobalGitRepositoryKey>(source, nameof (source));
      ArgumentUtility.CheckForEmptyGuid(targetRepoId, nameof (targetRepoId));
      this.Source = source;
      this.TargetRepoId = targetRepoId;
      this.SourceToTargetRefs = sourceToTargetRefs;
      this.CopySourceRepoDefaults = copySourceRepoDefaults;
    }

    [DataMember]
    public GlobalGitRepositoryKey Source { get; }

    [DataMember]
    public Guid TargetRepoId { get; }

    [DataMember(EmitDefaultValue = false)]
    public List<SourceToTargetRef> SourceToTargetRefs { get; }

    [DataMember(EmitDefaultValue = false)]
    public bool CopySourceRepoDefaults { get; }
  }
}
