// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionEnvironmentsBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "As per naming convention")]
  public class ReleaseDefinitionEnvironmentsBinder : 
    ReleaseManagementObjectBinderBase<ReleaseDefinitionEnvironmentsData>
  {
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder releaseDefinitionName = new SqlColumnBinder("ReleaseDefinitionName");
    private SqlColumnBinder environmentDefinitionId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder environmentDefintionName = new SqlColumnBinder("DefinitionEnvironmentName");

    public ReleaseDefinitionEnvironmentsBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinitionEnvironmentsData Bind() => new ReleaseDefinitionEnvironmentsData()
    {
      ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
      ReleaseDefinitionName = this.releaseDefinitionName.GetString((IDataReader) this.Reader, false),
      DefinitionEnvironmentId = this.environmentDefinitionId.GetInt32((IDataReader) this.Reader),
      DefinitionEnvironmentName = this.environmentDefintionName.GetString((IDataReader) this.Reader, false)
    };
  }
}
