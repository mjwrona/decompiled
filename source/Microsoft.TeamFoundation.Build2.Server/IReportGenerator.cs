// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IReportGenerator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [InheritedExport]
  public interface IReportGenerator
  {
    string ReportFormat { get; }

    string MediaType { get; }

    Encoding Encoding { get; }

    string GetBuildReport(IVssRequestContext requestContext, Microsoft.TeamFoundation.Build.WebApi.Build build);

    string GetBuildReport(IVssRequestContext requestContext, Guid projectId, int buildId);
  }
}
