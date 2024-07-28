// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionDataBinder3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionDataBinder3 : ObjectBinder<TaskDefinitionData>
  {
    private SqlColumnBinder TaskIdColumn = new SqlColumnBinder("TaskId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder DisplayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder MajorVersionColumn = new SqlColumnBinder("MajorVersion");
    private SqlColumnBinder MinorVersionColumn = new SqlColumnBinder("MinorVersion");
    private SqlColumnBinder PatchVersionColumn = new SqlColumnBinder("PatchVersion");
    private SqlColumnBinder IsTestColumn = new SqlColumnBinder("IsTest");
    private SqlColumnBinder ContainerId = new SqlColumnBinder(nameof (ContainerId));
    private SqlColumnBinder FilePath = new SqlColumnBinder(nameof (FilePath));
    private SqlColumnBinder MetadataDocumentColumn = new SqlColumnBinder("MetadataDocument");
    private SqlColumnBinder IconPathColumn = new SqlColumnBinder("IconPath");
    private SqlColumnBinder ContributionIdentifierColumn = new SqlColumnBinder("ContributionIdentifier");
    private SqlColumnBinder ContributionVersionColumn = new SqlColumnBinder("ContributionVersion");
    private SqlColumnBinder ContributionInstallComplete = new SqlColumnBinder(nameof (ContributionInstallComplete));
    private SqlColumnBinder ContributionUpdatedOn = new SqlColumnBinder(nameof (ContributionUpdatedOn));

    internal TaskDefinitionDataBinder3()
    {
    }

    protected override TaskDefinitionData Bind() => new TaskDefinitionData()
    {
      Id = this.TaskIdColumn.GetGuid((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      DisplayName = this.DisplayNameColumn.GetString((IDataReader) this.Reader, false),
      Version = new TaskVersion()
      {
        Major = this.MajorVersionColumn.GetInt32((IDataReader) this.Reader),
        Minor = this.MinorVersionColumn.GetInt32((IDataReader) this.Reader),
        Patch = this.PatchVersionColumn.GetInt32((IDataReader) this.Reader),
        IsTest = this.IsTestColumn.GetBoolean((IDataReader) this.Reader)
      },
      ContainerId = this.ContainerId.GetNullableInt64((IDataReader) this.Reader),
      FilePath = this.FilePath.GetString((IDataReader) this.Reader, true),
      MetadataDocument = this.MetadataDocumentColumn.GetBytes((IDataReader) this.Reader, false),
      IconPath = this.IconPathColumn.GetString((IDataReader) this.Reader, true),
      ContributionIdentifier = this.ContributionIdentifierColumn.GetString((IDataReader) this.Reader, true),
      ContributionVersion = this.ContributionVersionColumn.GetString((IDataReader) this.Reader, true),
      ContributionInstallComplete = this.ContributionInstallComplete.GetBoolean((IDataReader) this.Reader, true),
      ContributionUpdatedOn = this.ContributionUpdatedOn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
