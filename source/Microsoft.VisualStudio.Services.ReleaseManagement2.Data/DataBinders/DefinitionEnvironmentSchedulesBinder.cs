// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentSchedulesBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DefinitionEnvironmentSchedulesBinder : ObjectBinder<DefinitionEnvironment>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder schedules = new SqlColumnBinder("Schedules");

    protected override DefinitionEnvironment Bind()
    {
      DefinitionEnvironment definitionEnvironment = new DefinitionEnvironment()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader)
      };
      string str = this.schedules.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrWhiteSpace(str))
        definitionEnvironment.Schedules = JsonConvert.DeserializeObject<IList<ReleaseSchedule>>(str);
      return definitionEnvironment;
    }
  }
}
