// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentTemplateBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DefinitionEnvironmentTemplateBinder : ObjectBinder<DefinitionEnvironmentTemplate>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder environmentJson = new SqlColumnBinder("EnvironmentJson");

    protected override DefinitionEnvironmentTemplate Bind()
    {
      DefinitionEnvironmentTemplate environmentTemplate = new DefinitionEnvironmentTemplate()
      {
        Id = this.id.GetGuid((IDataReader) this.Reader, false),
        Name = this.name.GetString((IDataReader) this.Reader, false),
        Description = this.description.GetString((IDataReader) this.Reader, true)
      };
      if (this.environmentJson.ColumnExists((IDataReader) this.Reader))
        environmentTemplate.EnvironmentJson = this.environmentJson.GetString((IDataReader) this.Reader, false);
      return environmentTemplate;
    }
  }
}
