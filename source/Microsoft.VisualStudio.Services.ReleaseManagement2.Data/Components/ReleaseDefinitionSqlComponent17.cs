// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent17
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle onprem host servicing")]
  public class ReleaseDefinitionSqlComponent17 : ReleaseDefinitionSqlComponent16
  {
    protected override ReleaseDefinitionBinder GetReleaseDefinitionBinder() => (ReleaseDefinitionBinder) new ReleaseDefinitionBinder2((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This input has already been validated at above layer")]
    protected override void BindToReleaseDefinitionTable(
      ReleaseDefinition releaseDefinition,
      bool bindIds)
    {
      if (bindIds)
      {
        this.BindInt("id", releaseDefinition.Id);
        this.BindInt("revision", releaseDefinition.Revision);
      }
      this.BindString("Name", string.IsNullOrEmpty(releaseDefinition.Name) ? string.Empty : releaseDefinition.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("CreatedBy", releaseDefinition.CreatedBy);
      this.BindNullableDateTime("CreatedOn", releaseDefinition.CreatedOn);
      this.BindGuid("ModifiedBy", releaseDefinition.ModifiedBy);
      this.BindNullableDateTime("ModifiedOn", releaseDefinition.ModifiedOn);
      this.BindString("Variables", ServerModelUtility.ToString((object) VariablesUtility.ReplaceSecretVariablesWithNull(releaseDefinition.Variables)), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("VariableGroups", ServerModelUtility.ToString((object) releaseDefinition.VariableGroups), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("ReleaseNameFormat", string.IsNullOrEmpty(releaseDefinition.ReleaseNameFormat) ? string.Empty : releaseDefinition.ReleaseNameFormat, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }
  }
}
