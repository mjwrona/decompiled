// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentStepBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DefinitionEnvironmentStepBinder : ObjectBinder<DefinitionEnvironmentStep>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder isAutomated = new SqlColumnBinder("IsAutomated");
    private SqlColumnBinder isNotificationOn = new SqlColumnBinder("IsNotificationOn");
    private SqlColumnBinder approverId = new SqlColumnBinder("ApproverId");
    private SqlColumnBinder stepType = new SqlColumnBinder("StepType");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");

    protected override DefinitionEnvironmentStep Bind() => new DefinitionEnvironmentStep()
    {
      Id = this.id.GetInt32((IDataReader) this.Reader),
      Rank = this.rank.GetInt32((IDataReader) this.Reader),
      IsAutomated = this.isAutomated.GetBoolean((IDataReader) this.Reader, false),
      IsNotificationOn = this.isNotificationOn.GetBoolean((IDataReader) this.Reader, false),
      ApproverId = this.approverId.GetGuid((IDataReader) this.Reader, true),
      StepType = (EnvironmentStepType) this.stepType.GetByte((IDataReader) this.Reader),
      DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader)
    };
  }
}
