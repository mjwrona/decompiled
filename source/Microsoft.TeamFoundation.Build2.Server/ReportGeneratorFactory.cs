// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ReportGeneratorFactory
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class ReportGeneratorFactory : IReportGeneratorFactory, IDisposable
  {
    private Dictionary<string, IReportGenerator> m_reportGeneratorsMap;
    private IDisposableReadOnlyList<IReportGenerator> m_reportGenerators;

    public IReportGenerator GetReportGenerator(
      IVssRequestContext requestContext,
      string format,
      bool throwIfNotFound)
    {
      if (this.m_reportGeneratorsMap == null)
      {
        Dictionary<string, IReportGenerator> dictionary = new Dictionary<string, IReportGenerator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_reportGenerators = requestContext.GetExtensions<IReportGenerator>();
        foreach (IReportGenerator reportGenerator in (IEnumerable<IReportGenerator>) this.m_reportGenerators)
        {
          if (!dictionary.ContainsKey(reportGenerator.ReportFormat))
            dictionary.Add(reportGenerator.ReportFormat, reportGenerator);
        }
        this.m_reportGeneratorsMap = dictionary;
      }
      IReportGenerator reportGenerator1 = (IReportGenerator) null;
      if (!this.m_reportGeneratorsMap.TryGetValue(format, out reportGenerator1) & throwIfNotFound)
        throw new ReportFormatTypeNotSupportedException(BuildServerResources.ReportFormatNotSupported((object) format));
      return reportGenerator1;
    }

    void IDisposable.Dispose()
    {
      if (this.m_reportGeneratorsMap != null)
      {
        this.m_reportGeneratorsMap.Clear();
        this.m_reportGeneratorsMap = (Dictionary<string, IReportGenerator>) null;
      }
      if (this.m_reportGenerators == null)
        return;
      this.m_reportGenerators.Dispose();
      this.m_reportGenerators = (IDisposableReadOnlyList<IReportGenerator>) null;
    }
  }
}
