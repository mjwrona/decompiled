// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlertConfigurationBinder
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class AlertConfigurationBinder : ObjectBinder<AlertConfiguration>
  {
    protected SqlColumnBinder EventSource = new SqlColumnBinder(nameof (EventSource));
    protected SqlColumnBinder EventId = new SqlColumnBinder(nameof (EventId));
    protected SqlColumnBinder RuntimeEntry = new SqlColumnBinder(nameof (RuntimeEntry));
    protected SqlColumnBinder Version = new SqlColumnBinder(nameof (Version));
    protected SqlColumnBinder Enabled = new SqlColumnBinder(nameof (Enabled));
    protected SqlColumnBinder Area = new SqlColumnBinder(nameof (Area));
    protected SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));

    internal AlertConfigurationBinder()
    {
    }

    internal void Bind(out AlertConfiguration result) => result = this.Bind();

    protected override AlertConfiguration Bind() => new AlertConfiguration()
    {
      EventSource = this.EventSource.GetString((IDataReader) this.Reader, false),
      EventId = this.EventId.GetInt32((IDataReader) this.Reader),
      RuntimeEntry = this.RuntimeEntry.GetBoolean((IDataReader) this.Reader),
      Version = this.Version.GetInt32((IDataReader) this.Reader),
      Enabled = this.Enabled.GetBoolean((IDataReader) this.Reader),
      Area = this.Area.GetString((IDataReader) this.Reader, false),
      Description = this.Description.GetString((IDataReader) this.Reader, true)
    };
  }
}
