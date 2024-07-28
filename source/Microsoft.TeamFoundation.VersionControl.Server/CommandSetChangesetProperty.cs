// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandSetChangesetProperty
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandSetChangesetProperty : VersionControlCommand
  {
    public CommandSetChangesetProperty(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(int changesetId, PropertyValue[] propertyValues)
    {
      this.ValidateChangeset(changesetId);
      this.PropertyService.SetProperties(this.RequestContext, new ArtifactSpec(VersionControlPropertyKinds.Changeset, changesetId, 0), (IEnumerable<PropertyValue>) propertyValues);
    }

    public override void ContinueExecution()
    {
    }

    private void ValidateChangeset(int changesetId)
    {
      using (CommandQueryChangeset commandQueryChangeset = new CommandQueryChangeset(this.m_versionControlRequestContext))
      {
        commandQueryChangeset.Execute(changesetId, true, false, int.MaxValue, (string[]) null, VersionedItemPermissions.Checkin);
        if (commandQueryChangeset.Changeset == null)
          throw new ChangesetNotFoundException(changesetId);
        if (!commandQueryChangeset.Changeset.Changes.IsComplete)
        {
          foreach (Change change in commandQueryChangeset.Changeset.Changes)
            ;
        }
        if (commandQueryChangeset.CanAccessAllChanges)
          return;
        if (commandQueryChangeset.CanReadAtLeastOneChange)
          throw new ResourceAccessException(this.RequestContext.GetUserId().ToString(), "Checkin", Resources.Format("InsufficientAccessToSetPropertiesOnChangeset", (object) changesetId));
        throw new ChangesetNotFoundException(changesetId);
      }
    }

    private ITeamFoundationPropertyService PropertyService => this.m_versionControlRequestContext.VersionControlService.GetPropertyService(this.m_versionControlRequestContext);

    protected override void Dispose(bool disposing)
    {
    }
  }
}
