// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AccountPrefixTracer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class AccountPrefixTracer : OperationTracer
  {
    private string header2;
    private string header3;

    internal AccountPrefixTracer(
      VssRequestPump.Processor processor,
      MigrationTracer tracer,
      string header,
      string account,
      string prefix)
      : base(processor, tracer, header, "", prefix)
    {
      this.header2 = "(SA=" + account + ") " + header;
      this.header3 = "(SA=" + account + "|Prefix='" + prefix + "') " + header;
    }

    internal void InfoOnAccount(string msg)
    {
      MigrationTracer tracer = this.tracer.Enter(nameof (AccountPrefixTracer), nameof (InfoOnAccount));
      this.processor.ExecuteWorkAsync((Action<IVssRequestContext>) (req => tracer.Info(req, 197500, this.header2 + msg)));
    }

    internal override void Info(string msg)
    {
      MigrationTracer tracer = this.tracer.Enter(nameof (AccountPrefixTracer), nameof (Info));
      this.processor.ExecuteWorkAsync((Action<IVssRequestContext>) (req => tracer.Info(req, 197500, this.header3 + msg)));
    }
  }
}
