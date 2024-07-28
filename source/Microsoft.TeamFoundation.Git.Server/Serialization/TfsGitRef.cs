// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Serialization
{
  [XmlType("TfsGitRef")]
  [ClientType("TfsGitRef")]
  [XmlInclude(typeof (TfsGitBranchRef))]
  [XmlInclude(typeof (TfsGitTagRef))]
  [ClassVisibility(ClientVisibility.Internal)]
  public class TfsGitRef
  {
    public TfsGitRef()
    {
    }

    public TfsGitRef(IVssRequestContext requestContext, Microsoft.TeamFoundation.Git.Server.TfsGitRef gitRef)
    {
      this.Name = gitRef.Name;
      this.ObjectId = gitRef.ObjectId.ToString();
      this.IsLockedBy = gitRef.GetIdentityRef(requestContext);
    }

    public string Name { get; private set; }

    public string ObjectId { get; private set; }

    public IdentityRef IsLockedBy { get; private set; }
  }
}
