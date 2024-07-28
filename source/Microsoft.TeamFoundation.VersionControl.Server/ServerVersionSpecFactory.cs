// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ServerVersionSpecFactory
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ServerVersionSpecFactory : VersionSpecFactory
  {
    public override object CreateVersionSpec(
      VersionSpecType type,
      string originalInput,
      params object[] parameters)
    {
      switch (type)
      {
        case VersionSpecType.Changeset:
          return (object) new ChangesetVersionSpec((string) parameters[0]);
        case VersionSpecType.Date:
          return (object) new DateVersionSpec()
          {
            Date = (DateTime) parameters[0],
            OriginalText = (string) parameters[1]
          };
        case VersionSpecType.Label:
          return (object) new LabelVersionSpec((string) parameters[0], (string) parameters[1] ?? "$/");
        case VersionSpecType.Latest:
          return (object) new LatestVersionSpec();
        case VersionSpecType.Workspace:
          return parameters.Length != 2 ? (object) new WorkspaceVersionSpec((string) parameters[0], (string) parameters[1], (string) parameters[2]) : throw new InvalidVersionSpecException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("WorkspaceNameEmpty"));
        default:
          return (object) null;
      }
    }

    public override void ThrowInvalidVersionSpecException(string message) => throw new InvalidVersionSpecException(message);
  }
}
