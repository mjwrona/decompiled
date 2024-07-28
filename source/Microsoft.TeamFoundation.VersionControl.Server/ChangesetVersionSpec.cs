// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ChangesetVersionSpec : VersionSpec
  {
    private int m_changesetId;

    public ChangesetVersionSpec()
    {
    }

    [XmlAttribute("cs")]
    public int ChangesetId
    {
      get => this.m_changesetId;
      set => this.m_changesetId = value;
    }

    public ChangesetVersionSpec(string changeset) => this.ChangesetId = VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, changeset);

    public ChangesetVersionSpec(int changesetId) => this.ChangesetId = changesetId;

    public override int GetHashCode() => this.ChangesetId;

    public override int CompareTo(VersionSpec other)
    {
      int num = base.CompareTo(other);
      if (num == 0)
        num = this.ChangesetId - ((ChangesetVersionSpec) other).ChangesetId;
      return num;
    }

    public override string ToString() => this.ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public override int ToChangeset(IVssRequestContext requestContext) => this.ChangesetId;

    public override string ToDBString(IVssRequestContext requestContext) => "C" + this.ChangesetId.ToString();

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameter)
    {
      ChangesetVersionSpec.validate(versionControlRequestContext, this.ChangesetId);
    }

    internal static void validate(
      VersionControlRequestContext versionControlRequestContext,
      int changeset)
    {
      if (changeset < 0)
        throw new IllegalVersionException(changeset);
      if (changeset > versionControlRequestContext.VersionControlService.GetLatestChangeset(versionControlRequestContext))
        throw new ChangesetNotFoundException(changeset);
    }
  }
}
