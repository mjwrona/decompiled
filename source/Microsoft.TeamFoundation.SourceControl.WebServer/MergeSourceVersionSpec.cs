// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.MergeSourceVersionSpec
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class MergeSourceVersionSpec : VersionSpec
  {
    public static readonly char Identifier = 'M';
    public static readonly char UseRenameSourceIdentifier = 'R';

    public MergeSourceVersionSpec(int changesetId, bool useRenameSource)
    {
      this.ChangesetId = changesetId;
      this.UseRenameSource = useRenameSource;
    }

    public int ChangesetId { get; set; }

    public bool UseRenameSource { get; set; }

    public override string ToDBString(IVssRequestContext requestContext) => this.ToString();

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
    }

    public override string ToString() => this.UseRenameSource ? ((int) MergeSourceVersionSpec.Identifier + (int) MergeSourceVersionSpec.UseRenameSourceIdentifier).ToString() + this.ChangesetId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) : MergeSourceVersionSpec.Identifier.ToString() + this.ChangesetId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
  }
}
