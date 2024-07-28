// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DefinitionIdsBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These classes will be deleted post M91 deployment. So, better be together.")]
  public class DefinitionIdsBinder : ObjectBinder<ProjectAndDefinitionId>
  {
    private SqlColumnBinder projectId = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");

    protected override ProjectAndDefinitionId Bind() => new ProjectAndDefinitionId()
    {
      ProjectId = this.projectId.GetGuid((IDataReader) this.Reader),
      DefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader)
    };
  }
}
