// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlertConfigurationBinder3
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class AlertConfigurationBinder3 : AlertConfigurationBinder2
  {
    protected SqlColumnBinder MdmEventEnabledColumn = new SqlColumnBinder("MdmEventEnabled");
    protected SqlColumnBinder ScopeIndexColumn = new SqlColumnBinder("ScopeIndex");
    protected SqlColumnBinder EventIndexColumn = new SqlColumnBinder("EventIndex");

    internal AlertConfigurationBinder3()
    {
    }

    protected override AlertConfiguration Bind()
    {
      AlertConfiguration alertConfiguration = base.Bind();
      alertConfiguration.MdmEventEnabled = this.MdmEventEnabledColumn.GetBoolean((IDataReader) this.Reader);
      alertConfiguration.ScopeIndex = this.ScopeIndexColumn.GetInt32((IDataReader) this.Reader);
      alertConfiguration.EventIndex = this.EventIndexColumn.GetInt32((IDataReader) this.Reader);
      return alertConfiguration;
    }
  }
}
