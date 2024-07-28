// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionBinder5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseDefinitionBinder5 : ReleaseDefinitionBinder4
  {
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder pipelineProcessType = new SqlColumnBinder("PipelineProcessType");
    private SqlColumnBinder pipelineProcess = new SqlColumnBinder("PipelineProcess");
    private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder isDisabled = new SqlColumnBinder("IsDisabled");

    public ReleaseDefinitionBinder5(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinition Bind()
    {
      Guid guid = this.DataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      string str1 = this.pipelineProcess.GetString((IDataReader) this.Reader, (string) null);
      ReleaseDefinition releaseDefinition = new ReleaseDefinition()
      {
        Id = this.Id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        Name = this.Name.GetString((IDataReader) this.Reader, false),
        Description = this.description.GetString((IDataReader) this.Reader, string.Empty),
        ReleaseNameFormat = this.ReleaseNameFormat.GetString((IDataReader) this.Reader, string.Empty),
        CreatedBy = this.CreatedBy.GetGuid((IDataReader) this.Reader, true),
        CreatedOn = new DateTime?(this.CreatedOn.GetDateTime((IDataReader) this.Reader)),
        ModifiedBy = this.ModifiedBy.GetGuid((IDataReader) this.Reader, true),
        ModifiedOn = new DateTime?(this.ModifiedOn.GetDateTime((IDataReader) this.Reader)),
        IsDeleted = this.isDeleted.GetBoolean((IDataReader) this.Reader, false, false),
        IsDisabled = this.isDisabled.GetBoolean((IDataReader) this.Reader, false, false),
        Revision = this.Revision.GetInt32((IDataReader) this.Reader, 1),
        Path = PathHelper.DBPathToServerPath(this.Path.GetString((IDataReader) this.Reader, false)),
        PipelineProcessType = (PipelineProcessTypes) this.pipelineProcessType.GetByte((IDataReader) this.Reader, (byte) 1, (byte) 1),
        Comment = this.comment.GetString((IDataReader) this.Reader, string.Empty)
      };
      releaseDefinition.Source = (ReleaseDefinitionSource) this.Source.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
      string str2 = this.Variables.GetString((IDataReader) this.Reader, (string) null);
      if (str2 != null)
        VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(str2), releaseDefinition.Variables);
      string str3 = this.VariableGroups.GetString((IDataReader) this.Reader, (string) null);
      if (!string.IsNullOrWhiteSpace(str3))
      {
        List<int> values = ServerModelUtility.FromString<List<int>>(str3);
        if (values.Count > 0)
          releaseDefinition.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) values);
      }
      releaseDefinition.PipelineProcess = !string.IsNullOrEmpty(str1) ? JsonConvert.DeserializeObject<PipelineProcess>(str1) : (PipelineProcess) null;
      return releaseDefinition;
    }
  }
}
