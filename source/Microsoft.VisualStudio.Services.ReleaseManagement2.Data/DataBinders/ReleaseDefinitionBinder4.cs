// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionBinder4
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseDefinitionBinder4 : ReleaseDefinitionBinder3
  {
    public ReleaseDefinitionBinder4(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinition Bind()
    {
      Guid guid = this.DataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      ReleaseDefinition releaseDefinition = new ReleaseDefinition()
      {
        Id = this.Id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        Name = this.Name.GetString((IDataReader) this.Reader, false),
        ReleaseNameFormat = this.ReleaseNameFormat.GetString((IDataReader) this.Reader, string.Empty),
        CreatedBy = this.CreatedBy.GetGuid((IDataReader) this.Reader, true),
        CreatedOn = new DateTime?(this.CreatedOn.GetDateTime((IDataReader) this.Reader)),
        ModifiedBy = this.ModifiedBy.GetGuid((IDataReader) this.Reader, true),
        ModifiedOn = new DateTime?(this.ModifiedOn.GetDateTime((IDataReader) this.Reader)),
        Revision = this.Revision.GetInt32((IDataReader) this.Reader, 1),
        Path = PathHelper.DBPathToServerPath(this.Path.GetString((IDataReader) this.Reader, false))
      };
      releaseDefinition.Source = (ReleaseDefinitionSource) this.Source.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
      string str1 = this.Variables.GetString((IDataReader) this.Reader, (string) null);
      if (str1 != null)
        VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(str1), releaseDefinition.Variables);
      string str2 = this.VariableGroups.GetString((IDataReader) this.Reader, (string) null);
      if (!string.IsNullOrWhiteSpace(str2))
      {
        List<int> values = ServerModelUtility.FromString<List<int>>(str2);
        if (values.Count > 0)
          releaseDefinition.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) values);
      }
      return releaseDefinition;
    }
  }
}
