// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitBranchRef
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Serialization
{
  [XmlType("TfsGitBranchRef")]
  [ClientType("TfsGitBranchRef")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class TfsGitBranchRef : TfsGitRef
  {
    public TfsGitBranchRef()
    {
    }

    public TfsGitBranchRef(IVssRequestContext requestContext, Microsoft.TeamFoundation.Git.Server.TfsGitRef gitRef)
      : base(requestContext, gitRef)
    {
      this.IsDefaultBranch = gitRef.IsDefaultBranch;
    }

    public bool IsDefaultBranch { get; private set; }
  }
}
