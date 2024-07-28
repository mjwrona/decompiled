// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MigrationLogger
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class MigrationLogger
  {
    private IVssRequestContext m_RequestContext;
    private IServicingContext m_servicingContext;

    public MigrationLogger(TfsTestManagementRequestContext context) => this.m_RequestContext = context.RequestContext;

    public MigrationLogger(TestManagementRequestContext context, IServicingContext servicingContext)
    {
      this.m_RequestContext = context.RequestContext;
      this.m_servicingContext = servicingContext;
    }

    public void Log(TraceLevel traceLevel, string message)
    {
      if (traceLevel == TraceLevel.Off)
        return;
      if (this.m_servicingContext != null)
      {
        ServicingStepLogEntryKind entryKind = ServicingStepLogEntryKind.Informational;
        switch (traceLevel)
        {
          case TraceLevel.Error:
            entryKind = ServicingStepLogEntryKind.Error;
            break;
          case TraceLevel.Warning:
            entryKind = ServicingStepLogEntryKind.Warning;
            break;
        }
        this.m_servicingContext.Log(entryKind, message);
      }
      else
        this.m_RequestContext.Trace(1015001, traceLevel, "TestManagementJob", "BusinessLayer", message);
    }
  }
}
