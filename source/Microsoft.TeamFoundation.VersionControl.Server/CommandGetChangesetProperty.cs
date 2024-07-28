// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandGetChangesetProperty
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandGetChangesetProperty : VersionControlCommand
  {
    private StreamingCollection<ArtifactPropertyValue> m_result;
    private TeamFoundationDataReader m_dataReader;

    internal CommandGetChangesetProperty(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(int changesetId, string[] propertyNameFilters) => this.Execute(changesetId, propertyNameFilters, true);

    public void Execute(int changesetId, string[] propertyNameFilters, bool validate)
    {
      if (validate)
        this.ValidateChangeset(changesetId);
      this.m_dataReader = this.PropertyService.GetProperties(this.RequestContext, new ArtifactSpec(VersionControlPropertyKinds.Changeset, changesetId, 0), (IEnumerable<string>) propertyNameFilters);
      this.m_result = this.m_dataReader.Current<StreamingCollection<ArtifactPropertyValue>>();
    }

    public override void ContinueExecution()
    {
    }

    private void ValidateChangeset(int changesetId)
    {
      using (CommandQueryChangeset commandQueryChangeset = new CommandQueryChangeset(this.m_versionControlRequestContext))
      {
        commandQueryChangeset.Execute(changesetId, true, false, 1, (string[]) null, VersionedItemPermissions.Read);
        if (commandQueryChangeset.Changeset == null)
          throw new ChangesetNotFoundException(changesetId);
        commandQueryChangeset.Changeset.Changes.MoveNext();
      }
    }

    internal StreamingCollection<ArtifactPropertyValue> Result => this.m_result;

    private ITeamFoundationPropertyService PropertyService => this.m_versionControlRequestContext.VersionControlService.GetPropertyService(this.m_versionControlRequestContext);

    internal ArtifactPropertyValue First => this.Result == null || !this.Result.MoveNext() ? (ArtifactPropertyValue) null : this.Result.Current;

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.m_dataReader == null)
        return;
      this.m_dataReader.Dispose();
      this.m_dataReader = (TeamFoundationDataReader) null;
    }
  }
}
