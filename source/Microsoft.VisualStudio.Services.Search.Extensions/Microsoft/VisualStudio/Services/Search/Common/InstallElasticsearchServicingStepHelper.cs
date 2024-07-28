// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.InstallElasticsearchServicingStepHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class InstallElasticsearchServicingStepHelper : InstallElasticsearchHelperBase
  {
    private ServicingContext m_servicingContext;

    public InstallElasticsearchServicingStepHelper(ServicingContext servicingContext) => this.m_servicingContext = servicingContext;

    protected override void WriteError(string messageFormat, params object[] args) => this.WriteError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, messageFormat, args));

    protected override void WriteError(string text) => this.m_servicingContext.Error(text);

    protected override void WriteException(Exception exception) => this.WriteError(exception.ToString());

    protected override void WriteLine(string messageFormat, params object[] args) => this.m_servicingContext.LogInfo(string.Format((IFormatProvider) CultureInfo.InvariantCulture, messageFormat, args));

    protected override void WriteLine(string text) => this.m_servicingContext.LogInfo(text);
  }
}
