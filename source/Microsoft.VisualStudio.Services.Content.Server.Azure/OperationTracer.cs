// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.OperationTracer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class OperationTracer
  {
    protected VssRequestPump.Processor processor;
    protected MigrationTracer tracer;
    private string header1;

    internal OperationTracer(
      VssRequestPump.Processor processor,
      MigrationTracer tracer,
      string header,
      string sas,
      string prefix)
    {
      this.processor = processor;
      this.tracer = tracer;
      if (sas == null)
        sas = "All";
      this.header1 = "(SAGroup=" + sas + "|Prefix='" + prefix + "') " + header;
    }

    internal virtual void Info(string msg)
    {
      MigrationTracer tracer = this.tracer.Enter(nameof (OperationTracer), nameof (Info));
      this.processor.ExecuteWorkAsync((Action<IVssRequestContext>) (req => tracer.Info(req, 197500, this.header1 + msg)));
    }
  }
}
