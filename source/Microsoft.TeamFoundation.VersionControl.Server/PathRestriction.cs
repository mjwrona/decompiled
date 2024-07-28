// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PathRestriction
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class PathRestriction
  {
    [Obsolete("Use the constructor that takes the Identity object instead")]
    [Browsable(false)]
    public PathRestriction(string user, List<ClientArtifact> items, bool allChangesIncluded)
    {
      this.User = user;
      this.Items = items != null ? new ReadOnlyCollection<ClientArtifact>((IList<ClientArtifact>) items) : new ReadOnlyCollection<ClientArtifact>((IList<ClientArtifact>) new List<ClientArtifact>());
      this.AllChangesIncluded = allChangesIncluded;
    }

    public PathRestriction(
      TeamFoundationIdentity identity,
      List<ClientArtifact> items,
      bool allChangesIncluded)
      : this(IdentityUtil.Convert(identity), items, allChangesIncluded)
    {
    }

    public PathRestriction(Microsoft.VisualStudio.Services.Identity.Identity identity, List<ClientArtifact> items, bool allChangesIncluded)
    {
      this.Identity = identity;
      this.User = IdentityHelper.GetDomainUserName(identity);
      this.Items = items != null ? new ReadOnlyCollection<ClientArtifact>((IList<ClientArtifact>) items) : new ReadOnlyCollection<ClientArtifact>((IList<ClientArtifact>) new List<ClientArtifact>());
      this.AllChangesIncluded = allChangesIncluded;
    }

    [Obsolete("Use the Identity property instead")]
    [Browsable(false)]
    public string User { get; private set; }

    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; private set; }

    public ReadOnlyCollection<ClientArtifact> Items { get; private set; }

    public bool AllChangesIncluded { get; private set; }
  }
}
