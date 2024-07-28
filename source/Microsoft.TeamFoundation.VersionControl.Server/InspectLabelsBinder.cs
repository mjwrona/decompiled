// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InspectLabelsBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class InspectLabelsBinder : VersionControlObjectBinder<InspectLabelsInfo>
  {
    private SqlColumnBinder labelName = new SqlColumnBinder("LabelName");
    private SqlColumnBinder rows = new SqlColumnBinder("Rows");

    protected override InspectLabelsInfo Bind() => new InspectLabelsInfo()
    {
      LabelName = this.labelName.GetString((IDataReader) this.Reader, false),
      Rows = this.rows.GetInt64((IDataReader) this.Reader)
    };
  }
}
