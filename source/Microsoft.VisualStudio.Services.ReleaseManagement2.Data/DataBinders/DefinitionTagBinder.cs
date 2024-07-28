// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionTagBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "As per naming convention")]
  public sealed class DefinitionTagBinder : ReleaseManagementObjectBinderBase<DefinitionTagData>
  {
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder tag = new SqlColumnBinder("Tag");

    public DefinitionTagBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DefinitionTagData Bind() => new DefinitionTagData()
    {
      DefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader),
      Tag = this.tag.GetString((IDataReader) this.Reader, false)
    };
  }
}
