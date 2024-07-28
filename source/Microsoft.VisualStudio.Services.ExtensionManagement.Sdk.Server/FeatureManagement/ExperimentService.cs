// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ExperimentService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  internal class ExperimentService : IExperimentService, IVssFrameworkService
  {
    private Random randomNumberGenerator = new Random();
    private const string c_area = "ab-tests";
    private const string c_experimentsRequestKey = "c_experimentsRequestKey";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public ExperimentFeature GetExperiment(IVssRequestContext requestContext, string contributionId) => this.GetExperimentLookup(requestContext, contributionId);

    public T RunPerformanceExperiment<T>(
      IVssRequestContext requestContext,
      string contributionId,
      Func<T> controlAlgorithm,
      Func<T> experimentalAlgorithm,
      bool runBothAlgorithms = false)
    {
      if (this.GetExperiment(requestContext, contributionId) == null)
        return controlAlgorithm();
      int num = requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, contributionId) ? 1 : 0;
      List<Func<T>> source = new List<Func<T>>();
      if (num == 0 | runBothAlgorithms)
        source.Add((Func<T>) (() => this.RunAlgorithm<T>(requestContext, contributionId, controlAlgorithm, false)));
      if (num != 0)
        source.Add((Func<T>) (() => this.RunAlgorithm<T>(requestContext, contributionId, experimentalAlgorithm, true)));
      return ((IEnumerable<T>) source.OrderBy<Func<T>, double>((Func<Func<T>, double>) (value => this.randomNumberGenerator.NextDouble())).ToList<Func<T>>().Select<Func<T>, T>((Func<Func<T>, T>) (algorithmRunner => algorithmRunner())).ToArray<T>()).First<T>();
    }

    public void RunPerformanceExperiment(
      IVssRequestContext requestContext,
      string contributionId,
      Action controlAlgorithm,
      Action experimentalAlgorithm,
      bool runBothAlgorithms = false)
    {
      this.RunPerformanceExperiment<object>(requestContext, contributionId, this.WrapActionInFunc(controlAlgorithm), this.WrapActionInFunc(experimentalAlgorithm), runBothAlgorithms);
    }

    private T RunAlgorithm<T>(
      IVssRequestContext requestContext,
      string contributionId,
      Func<T> algorithm,
      bool isExperimentalAlgorithm)
    {
      this.PublishLoaded(requestContext, contributionId, isExperimentalAlgorithm);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      T obj = algorithm();
      this.PublishAverageMeasurement(requestContext, contributionId, "ElapsedTime", isExperimentalAlgorithm, (double) stopwatch.ElapsedMilliseconds);
      return obj;
    }

    private ExperimentFeature GetExperimentLookup(
      IVssRequestContext requestContext,
      string contributionId)
    {
      IDictionary<string, ExperimentFeature> dictionary;
      if (requestContext.Items.ContainsKey("c_experimentsRequestKey"))
      {
        dictionary = (IDictionary<string, ExperimentFeature>) requestContext.Items["c_experimentsRequestKey"];
      }
      else
      {
        dictionary = (IDictionary<string, ExperimentFeature>) new Dictionary<string, ExperimentFeature>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.Items["c_experimentsRequestKey"] = (object) dictionary;
      }
      ExperimentFeature experimentLookup = (ExperimentFeature) null;
      if (!dictionary.TryGetValue(contributionId, out experimentLookup))
      {
        ContributedFeature feature = requestContext.GetService<IContributedFeatureService>().GetFeature(requestContext, contributionId);
        if (feature != null)
          experimentLookup = new ExperimentFeature(feature);
        dictionary[contributionId] = experimentLookup;
      }
      return experimentLookup;
    }

    private void PublishAverageMeasurement(
      IVssRequestContext requestContext,
      string contributionId,
      string measurementName,
      bool isExperimentalAlgorithm,
      double value)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("MeasurementName", measurementName);
      properties.Add(nameof (value), value);
      this.PublishTelemetry(requestContext, contributionId, "average", isExperimentalAlgorithm, properties);
    }

    private void PublishLoaded(
      IVssRequestContext requestContext,
      string contributionId,
      bool isExperimentalGroup)
    {
      this.PublishTelemetry(requestContext, contributionId, "load", isExperimentalGroup, new CustomerIntelligenceData());
    }

    private void PublishTelemetry(
      IVssRequestContext requestContext,
      string contributionId,
      string measurmentType,
      bool isExperimentalGroup,
      CustomerIntelligenceData properties)
    {
      ExperimentFeature experiment = this.GetExperiment(requestContext, contributionId);
      properties.Add("MeasurementType", measurmentType);
      properties.Add("ExperimentVariant", isExperimentalGroup ? "Variation" : "Control");
      properties.Add("ExperimentSchemaVersion", 1.0);
      properties.Add("GroupBy", experiment.GroupByStrategyName);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ab-tests", contributionId, properties);
    }

    private Func<object> WrapActionInFunc(Action action) => (Func<object>) (() =>
    {
      action();
      return (object) null;
    });
  }
}
