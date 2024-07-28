// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.DataTransformPipelineBase`1
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public abstract class DataTransformPipelineBase<RecordType> : 
    IDataTransformPipeline,
    IDataServicesService,
    IChartTransformer
  {
    public IVssRequestContext RequestContext { get; set; }

    public IProvideFilteredData<RecordType> DataProvider { get; set; }

    public IInterpretTimedData<RecordType> DataInterpreter { get; set; }

    public IInterpretQueryText QueryInterpreter { get; set; }

    public abstract IEnumerable<TransformResult> GetResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInfo,
      IEnumerable<TransformOptions> options);

    public IInterpretQueryText GetQueryTextInterpreter() => this.QueryInterpreter;

    protected void RecordTabulationCost(string scope, List<ITabulator<RecordType>> tabulators)
    {
      if (tabulators == null)
        return;
      foreach (ITabulator<RecordType> tabulator in tabulators)
      {
        if (tabulator is ICountLoad loadCost)
          TelemetryHelper.PublishTabulationCost(this.RequestContext, scope, tabulator.TabulationInstructions.Options, loadCost);
      }
    }

    protected ITabulator<RecordType> InstantiateTabulator(
      IVssRequestContext requestContext,
      TransformInstructions<RecordType> instructions)
    {
      Tabulator<RecordType> tabulator = new Tabulator<RecordType>();
      tabulator.TabulationInstructions = instructions;
      tabulator.RecordInterpreter = this.DataInterpreter;
      return (ITabulator<RecordType>) tabulator;
    }

    protected void ProcessResults(
      IVssRequestContext requestContext,
      IEnumerable<ITabulator<RecordType>> tabulators,
      IEnumerable<RecordType> dataset)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      int capacity = 100;
      List<RecordType> buffer = new List<RecordType>(capacity);
      foreach (RecordType recordType in dataset)
      {
        if ((double) stopwatch.ElapsedMilliseconds >= DataServicesTimeoutPolicy.GetTransformerTimeout(requestContext).TotalMilliseconds)
          throw new TimeoutException();
        buffer.Add(recordType);
        if (buffer.Count == capacity)
          this.TabulateRecords((IList<RecordType>) buffer, tabulators, requestContext);
      }
      this.TabulateRecords((IList<RecordType>) buffer, tabulators, requestContext);
    }

    private void TabulateRecords(
      IList<RecordType> buffer,
      IEnumerable<ITabulator<RecordType>> tabulators,
      IVssRequestContext requestContext)
    {
      if (buffer.Count <= 0)
        return;
      foreach (ITabulator<RecordType> tabulator in tabulators)
        tabulator.Tabulate((IEnumerable<RecordType>) buffer, requestContext);
      buffer.Clear();
    }
  }
}
