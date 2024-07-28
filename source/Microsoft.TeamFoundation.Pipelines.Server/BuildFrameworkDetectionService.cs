// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.BuildFrameworkDetectionService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class BuildFrameworkDetectionService : IBuildFrameworkDetectionService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IReadOnlyList<DetectedBuildFramework> Detect(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      BuildFrameworkDetectionType detectionType)
    {
      using (new Tracer<BuildFrameworkDetectionService>(requestContext, TracePoints.BuildFrameworkDetection.DetectEnter, TracePoints.BuildFrameworkDetection.DetectLeave, nameof (Detect)))
      {
        TreeAnalysis treeAnalysis = this.GetTreeAnalysis(requestContext, projectId, connectionId, repositoryType, repository, branch);
        return treeAnalysis == null ? (IReadOnlyList<DetectedBuildFramework>) Array.Empty<DetectedBuildFramework>() : this.Detect(requestContext, projectId, repositoryType, repository, connectionId, branch, treeAnalysis, detectionType);
      }
    }

    public IReadOnlyList<DetectedBuildFramework> Detect(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      TreeAnalysis treeAnalysis,
      BuildFrameworkDetectionType detectionType)
    {
      using (new Tracer<BuildFrameworkDetectionService>(requestContext, TracePoints.BuildFrameworkDetection.DetectEnter, TracePoints.BuildFrameworkDetection.DetectLeave, nameof (Detect)))
      {
        CachingFileContentsProvider fileContentsProvider = new CachingFileContentsProvider((IFileContentsProvider) new SourceProviderFileContentsProvider(projectId, repositoryType, repository, connectionId, branch));
        return this.DetectInternal(requestContext, repositoryType, repository, treeAnalysis, (InstrumentedFileContentsProvider) fileContentsProvider, detectionType);
      }
    }

    public IReadOnlyList<DetectedBuildFramework> Detect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType)
    {
      InstrumentedFileContentsProvider fileContentsProvider1 = new InstrumentedFileContentsProvider(fileContentsProvider);
      return this.DetectInternal(requestContext, (string) null, (string) null, treeAnalysis, fileContentsProvider1, detectionType);
    }

    private IReadOnlyList<DetectedBuildFramework> DetectInternal(
      IVssRequestContext requestContext,
      string repositoryType,
      string repository,
      TreeAnalysis treeAnalysis,
      InstrumentedFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType)
    {
      IReadOnlyList<IBuildFrameworkDetector> detectors = this.GetDetectors(requestContext);
      List<DetectedBuildFramework> detectedBuildFrameworkList = new List<DetectedBuildFramework>(detectors.Count);
      foreach (IBuildFrameworkDetector detector in (IEnumerable<IBuildFrameworkDetector>) detectors)
      {
        DetectedBuildFramework detectedBuildFramework;
        if (this.TryDetectInternal(detector, requestContext, repositoryType, repository, treeAnalysis, fileContentsProvider, detectionType, out detectedBuildFramework))
          detectedBuildFrameworkList.Add(detectedBuildFramework);
      }
      DetectedBuildFramework detectedBuildFramework1;
      if (!detectedBuildFrameworkList.Any<DetectedBuildFramework>() && this.TryDetectInternal(this.GetFallbackDetector(requestContext), requestContext, repositoryType, repository, treeAnalysis, fileContentsProvider, detectionType, out detectedBuildFramework1))
        detectedBuildFrameworkList.Add(detectedBuildFramework1);
      return requestContext.GetService<IDetectedBuildFrameworkWeightingService>().AssignWeights((IReadOnlyList<DetectedBuildFramework>) detectedBuildFrameworkList);
    }

    private bool TryDetectInternal(
      IBuildFrameworkDetector detector,
      IVssRequestContext requestContext,
      string repositoryType,
      string repository,
      TreeAnalysis treeAnalysis,
      InstrumentedFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      fileContentsProvider.StartGatheringStatistics(detector.GetType().Name, detectionType, treeAnalysis.FileCount);
      bool success;
      using (fileContentsProvider.StatisticsBuilder.MeasureAnalysisTime())
        success = detector.TryDetect(requestContext, treeAnalysis, (IFileContentsProvider) fileContentsProvider, detectionType, out detectedBuildFramework);
      this.LogStatistics(requestContext, repositoryType, repository, success, fileContentsProvider.GetStatistics());
      return success;
    }

    private void LogStatistics(
      IVssRequestContext requestContext,
      string repositoryType,
      string repositoryId,
      bool success,
      DetectorStats detectorstats)
    {
      object obj1 = (object) new ExpandoObject();
      // ISSUE: reference to a compiler-generated field
      if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "RepositoryType", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__0, obj1, repositoryType ?? "");
      // ISSUE: reference to a compiler-generated field
      if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "RepositoryId", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__1.Target((CallSite) BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__1, obj1, repositoryId ?? "");
      // ISSUE: reference to a compiler-generated field
      if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DetectorSuccess", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__2.Target((CallSite) BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__2, obj1, success.ToString());
      if (detectorstats != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DetectorName", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__3.Target((CallSite) BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__3, obj1, detectorstats.DetectorName);
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DetectionType", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj6 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__4.Target((CallSite) BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__4, obj1, detectorstats.DetectionType.ToString());
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "FileCacheHits", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target1 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p5 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__5;
        object obj7 = obj1;
        int num1 = detectorstats.FileCacheHits;
        string str1 = num1.ToString();
        object obj8 = target1((CallSite) p5, obj7, str1);
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "FileCacheMisses", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target2 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p6 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__6;
        object obj9 = obj1;
        num1 = detectorstats.FileCacheMisses;
        string str2 = num1.ToString();
        object obj10 = target2((CallSite) p6, obj9, str2);
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "FilesReadCount", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target3 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__7.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p7 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__7;
        object obj11 = obj1;
        num1 = detectorstats.FilesReadCount;
        string str3 = num1.ToString();
        object obj12 = target3((CallSite) p7, obj11, str3);
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "TotalFileCount", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target4 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__8.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p8 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__8;
        object obj13 = obj1;
        num1 = detectorstats.TotalFileCount;
        string str4 = num1.ToString();
        object obj14 = target4((CallSite) p8, obj13, str4);
        double totalMilliseconds1 = detectorstats.TotalAnalysisTime.TotalMilliseconds;
        double totalMilliseconds2 = TimeSpan.FromMilliseconds(detectorstats.FilesReadCount > 0 ? totalMilliseconds1 / (double) detectorstats.FilesReadCount : 0.0).TotalMilliseconds;
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DetectorTimeMS", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target5 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__9.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p9 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__9;
        object obj15 = obj1;
        long num2 = (long) totalMilliseconds1;
        string str5 = num2.ToString();
        object obj16 = target5((CallSite) p9, obj15, str5);
        // ISSUE: reference to a compiler-generated field
        if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "AverageFileAnalysisTimeMS", typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target6 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__10.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p10 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__10;
        object obj17 = obj1;
        num2 = (long) totalMilliseconds2;
        string str6 = num2.ToString();
        object obj18 = target6((CallSite) p10, obj17, str6);
      }
      IVssRequestContext requestContext1 = requestContext;
      int statistics = TracePoints.BuildFrameworkDetection.Statistics;
      string area = TracePoints.Area;
      // ISSUE: reference to a compiler-generated field
      if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (BuildFrameworkDetectionService)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__12.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p12 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__12;
      // ISSUE: reference to a compiler-generated field
      if (BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__11 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SerializeObject", (IEnumerable<Type>) null, typeof (BuildFrameworkDetectionService), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj19 = BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__11.Target((CallSite) BuildFrameworkDetectionService.\u003C\u003Eo__7.\u003C\u003Ep__11, typeof (JsonConvert), obj1);
      string format = target((CallSite) p12, obj19);
      object[] objArray = Array.Empty<object>();
      requestContext1.TraceAlways(statistics, TraceLevel.Info, area, nameof (BuildFrameworkDetectionService), format, objArray);
    }

    private IReadOnlyList<IBuildFrameworkDetector> GetDetectors(IVssRequestContext requestContext) => requestContext.GetService<IBuildFrameworkDetectorProviderService>().GetDetectors(requestContext);

    private IBuildFrameworkDetector GetFallbackDetector(IVssRequestContext requestContext) => requestContext.GetService<IBuildFrameworkDetectorProviderService>().GetFallbackDetector(requestContext);

    private TreeAnalysis GetTreeAnalysis(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? connectionId,
      string repositoryType,
      string repository,
      string branch)
    {
      return requestContext.GetService<ITreeAnalysisProviderService>().GetTreeAnalysis(requestContext, projectId, connectionId, repositoryType, repository, branch);
    }
  }
}
