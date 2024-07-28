// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.YmlCoverageStatusCheckConfigurationProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class YmlCoverageStatusCheckConfigurationProvider : 
    ICoverageStatusCheckConfigurationProvider
  {
    private const string CodeCoverageYamlFilePath = "azurepipelines-coverage.yml";

    public CoverageStatusCheckConfiguration GetCoverageStatusCheckConfiguration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      string buildSourceVersionCommit,
      Dictionary<string, object> ciData)
    {
      CoverageStatusCheckConfiguration checkConfiguration1 = (CoverageStatusCheckConfiguration) null;
      string str = "Default";
      CoverageCacheProvider coverageCacheProvider = new CoverageCacheProvider();
      string cacheContainerName = pipelineContext.Id.ToString() + buildSourceVersionCommit + "azurepipelines-coverage.yml";
      tcmRequestContext.Logger.Info(1015909, "CacheContainerName: " + cacheContainerName);
      string repoProperty = coverageCacheProvider.GetRepoProperty(tcmRequestContext.RequestContext, cacheContainerName, "DiffCoverageStatusCheck");
      if (repoProperty != null && repoProperty != string.Empty)
      {
        ciData.Add("isCoverageSettingsFromCache", (object) true);
        return JsonConvert.DeserializeObject<CoverageStatusCheckConfiguration>(repoProperty);
      }
      try
      {
        CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
        checkConfiguration1 = coverageConfiguration.GetCoverageStatusCheckConfiguration(tcmRequestContext.RequestContext);
        checkConfiguration1.IsPRExperienceEnabled = coverageConfiguration.GetCodeCoverageStatusFeature(tcmRequestContext.RequestContext);
        checkConfiguration1.IsCommentsEnabled = coverageConfiguration.GetCoveragePRCommentsFeature(tcmRequestContext.RequestContext);
        checkConfiguration1.IsFolderLevelPolicyEnabled = false;
        int coverageYamlMaxBytes = coverageConfiguration.GetCodeCoverageYamlMaxBytes(tcmRequestContext.RequestContext);
        using (Stream file = versionControlProvider.GetFile(tcmRequestContext, pipelineContext, buildSourceVersionCommit, "azurepipelines-coverage.yml"))
        {
          using (Stream stream = this.ReadStream(file, coverageYamlMaxBytes))
          {
            if (stream.Length > (long) coverageYamlMaxBytes)
              throw new Exception(string.Format("Code coverage yaml file size is {0} bytes, which exceeded the limit of {1} bytes", (object) stream.Length, (object) coverageYamlMaxBytes));
            using (StreamReader input = new StreamReader(stream))
            {
              object obj1 = new DeserializerBuilder().WithNodeTypeResolver((INodeTypeResolver) new YmlCoverageStatusCheckConfigurationProvider.ExpandoNodeTypeResolver(), (Action<IRegistrationLocationSelectionSyntax<INodeTypeResolver>>) (ls => ls.InsteadOf<DefaultContainersNodeTypeResolver>())).Build().Deserialize((TextReader) input);
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__1 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__1.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__1;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj2 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__0, obj1, (object) null);
              if (target1((CallSite) p1, obj2))
                throw new Exception(CoverageResources.InvalidCoverageConfigurationGenericErrorMessage);
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__3 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, object> target2 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__3.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, object>> p3 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__3;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__2 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj3 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__2.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__2, obj1);
              if (target2((CallSite) p3, obj3) is string)
              {
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__7 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target3 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__7.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p7 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__7;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__6 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__6 = CallSite<Func<CallSite, Type, object, string, StringComparison, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Equals", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, Type, object, string, StringComparison, object> target4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__6.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, Type, object, string, StringComparison, object>> p6 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__6;
                Type type = typeof (string);
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__5 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target5 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__5.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p5 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__5;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__4 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__4.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__4, obj1);
                object obj5 = target5((CallSite) p5, obj4);
                object obj6 = target4((CallSite) p6, type, obj5, "off", StringComparison.OrdinalIgnoreCase);
                if (target3((CallSite) p7, obj6))
                {
                  checkConfiguration1.IsPRExperienceEnabled = false;
                  ciData.Add("IsEnabled", (object) checkConfiguration1.IsPRExperienceEnabled);
                  return checkConfiguration1;
                }
              }
              checkConfiguration1.IsPRExperienceEnabled = true;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__16 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target6 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__16.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p16 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__16;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__10 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__10 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ContainsProperty", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object> target7 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__10.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>> p10 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__10;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__9 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, object> target8 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__9.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, object>> p9 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__9;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__8 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj7 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__8.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__8, obj1);
              object obj8 = target8((CallSite) p9, obj7);
              object obj9 = target7((CallSite) p10, this, obj8, "comments");
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__15 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              object obj10;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              if (!YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__15.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__15, obj9))
              {
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__14 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool, object> target9 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__14.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool, object>> p14 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__14;
                object obj11 = obj9;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__13 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comments", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target10 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__13.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p13 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__13;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__12 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target11 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__12.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p12 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__12;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__11 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj12 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__11.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__11, obj1);
                object obj13 = target11((CallSite) p12, obj12);
                int num = target10((CallSite) p13, obj13) is string ? 1 : 0;
                obj10 = target9((CallSite) p14, obj11, num != 0);
              }
              else
                obj10 = obj9;
              if (target6((CallSite) p16, obj10))
              {
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__21 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target12 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__21.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p21 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__21;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__20 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__20 = CallSite<Func<CallSite, Type, object, string, StringComparison, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Equals", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, Type, object, string, StringComparison, object> target13 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__20.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, Type, object, string, StringComparison, object>> p20 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__20;
                Type type1 = typeof (string);
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__19 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comments", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target14 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__19.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p19 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__19;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__18 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target15 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__18.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p18 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__18;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__17 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj14 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__17.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__17, obj1);
                object obj15 = target15((CallSite) p18, obj14);
                object obj16 = target14((CallSite) p19, obj15);
                object obj17 = target13((CallSite) p20, type1, obj16, "on", StringComparison.OrdinalIgnoreCase);
                if (target12((CallSite) p21, obj17))
                {
                  checkConfiguration1.IsCommentsEnabled = true;
                }
                else
                {
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__26 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, bool> target16 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__26.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, bool>> p26 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__26;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__25 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__25 = CallSite<Func<CallSite, Type, object, string, StringComparison, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Equals", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, Type, object, string, StringComparison, object> target17 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__25.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, Type, object, string, StringComparison, object>> p25 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__25;
                  Type type2 = typeof (string);
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__24 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comments", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, object> target18 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__24.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, object>> p24 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__24;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__23 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, object> target19 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__23.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, object>> p23 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__23;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__22 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj18 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__22.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__22, obj1);
                  object obj19 = target19((CallSite) p23, obj18);
                  object obj20 = target18((CallSite) p24, obj19);
                  object obj21 = target17((CallSite) p25, type2, obj20, "off", StringComparison.OrdinalIgnoreCase);
                  if (target16((CallSite) p26, obj21))
                    checkConfiguration1.IsCommentsEnabled = false;
                }
              }
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__43 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__43 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target20 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__43.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p43 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__43;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__29 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__29 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ContainsProperty", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object> target21 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__29.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>> p29 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__29;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__28 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, object> target22 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__28.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, object>> p28 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__28;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__27 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj22 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__27.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__27, obj1);
              object obj23 = target22((CallSite) p28, obj22);
              object obj24 = target21((CallSite) p29, this, obj23, "diff");
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__35 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              object obj25;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              if (!YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__35.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__35, obj24))
              {
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__34 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object, object> target23 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__34.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object, object>> p34 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__34;
                object obj26 = obj24;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__33 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object, object> target24 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__33.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object, object>> p33 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__33;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__32 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "diff", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target25 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__32.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p32 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__32;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__31 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target26 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__31.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p31 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__31;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__30 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj27 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__30.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__30, obj1);
                object obj28 = target26((CallSite) p31, obj27);
                object obj29 = target25((CallSite) p32, obj28);
                object obj30 = target24((CallSite) p33, obj29, (object) null);
                obj25 = target23((CallSite) p34, obj26, obj30);
              }
              else
                obj25 = obj24;
              object obj31 = obj25;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__42 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              object obj32;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              if (!YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__42.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__42, obj31))
              {
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__41 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object, object> target27 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__41.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object, object>> p41 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__41;
                object obj33 = obj31;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__40 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__40 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object, object> target28 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__40.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object, object>> p40 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__40;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__39 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "target", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target29 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__39.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p39 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__39;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__38 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "diff", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target30 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__38.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p38 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__38;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__37 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target31 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__37.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p37 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__37;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__36 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj34 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__36.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__36, obj1);
                object obj35 = target31((CallSite) p37, obj34);
                object obj36 = target30((CallSite) p38, obj35);
                object obj37 = target29((CallSite) p39, obj36);
                object obj38 = target28((CallSite) p40, obj37, (object) null);
                obj32 = target27((CallSite) p41, obj33, obj38);
              }
              else
                obj32 = obj31;
              if (target20((CallSite) p43, obj32))
              {
                str = "UserSpecified";
                CoverageStatusCheckConfiguration checkConfiguration2 = checkConfiguration1;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__49 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__49 = CallSite<Func<CallSite, object, double?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (double?), typeof (YmlCoverageStatusCheckConfigurationProvider)));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, double?> target32 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__49.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, double?>> p49 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__49;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__48 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__48 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "GetTargetValue", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, object> target33 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__48.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, object>> p48 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__48;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__47 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__47 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "target", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target34 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__47.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p47 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__47;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__46 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__46 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "diff", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target35 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__46.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p46 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__46;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__45 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__45 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, object> target36 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__45.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, object>> p45 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__45;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__44 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj39 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__44.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__1.\u003C\u003Ep__44, obj1);
                object obj40 = target36((CallSite) p45, obj39);
                object obj41 = target35((CallSite) p46, obj40);
                object obj42 = target34((CallSite) p47, obj41);
                object obj43 = target33((CallSite) p48, this, obj42);
                double? nullable = target32((CallSite) p49, obj43);
                checkConfiguration2.DiffCoverageThreshold = nullable;
              }
            }
          }
        }
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null && ex.InnerException is VssServiceException && ex.InnerException.Message.Contains("TF401174"))
        {
          ciData.Add("CodeCoverageYamlWarning", (object) ex.InnerException.Message);
          tcmRequestContext.Logger.Warning(1015694, string.Format("Ignoring the error. Coverage yaml file does not exist: {0}. Error: {1}", (object) "azurepipelines-coverage.yml", (object) ex.InnerException));
        }
        else
        {
          checkConfiguration1.ExceptionMessage = ex.InnerException.Message;
          ciData.Add("CodeCoverageYamlError", ex.InnerException != null ? (object) ex.InnerException.Message : (object) ex.Message);
          return checkConfiguration1;
        }
      }
      catch (Exception ex)
      {
        checkConfiguration1.ExceptionMessage = ex.Message;
        ciData.Add("CodeCoverageYamlError", (object) ex.Message);
        tcmRequestContext.Logger.Error(1015685, string.Format("Error while reading status check configuration from: {0}: {1}", (object) "azurepipelines-coverage.yml", (object) ex));
        return checkConfiguration1;
      }
      ciData["CodeCoverageYamlType"] = (object) str;
      ciData["DiffCoverageTarget"] = (object) checkConfiguration1.DiffCoverageThreshold;
      ciData["IsCommentsEnabled"] = (object) checkConfiguration1.IsCommentsEnabled;
      ciData["isCoverageSettingsFromCache"] = (object) false;
      coverageCacheProvider.SetRepoProperty(tcmRequestContext.RequestContext, cacheContainerName, "DiffCoverageStatusCheck", JsonConvert.SerializeObject((object) checkConfiguration1));
      return checkConfiguration1;
    }

    private bool ContainsProperty(object obj, string propertyName)
    {
      if (!(obj is IDictionary<string, object>))
        return false;
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IDictionary<string, object>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (IDictionary<string, object>), typeof (YmlCoverageStatusCheckConfigurationProvider)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__2.\u003C\u003Ep__0, obj).ContainsKey(propertyName);
    }

    private Stream ReadStream(Stream stream, int maxBytesLimit)
    {
      if (stream.CanSeek)
        return stream;
      byte[] buffer = new byte[4096];
      int num = 0;
      MemoryStream memoryStream = new MemoryStream();
      int count;
      while ((count = stream.Read(buffer, 0, 4096)) > 0)
      {
        num += count;
        memoryStream.Write(buffer, 0, count);
        if (num > maxBytesLimit)
          break;
      }
      memoryStream.Position = 0L;
      return (Stream) memoryStream;
    }

    private double GetTargetValue(object targetValue)
    {
      double targetValue1 = 0.0;
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p10 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__10;
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.Not, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target2 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "IsNullOrEmpty", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__0, typeof (string), targetValue);
      object obj2 = target2((CallSite) p1, obj1);
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      object obj3;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__5.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__5, obj2))
      {
        // ISSUE: reference to a compiler-generated field
        if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target3 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__4.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__4;
        object obj4 = obj2;
        // ISSUE: reference to a compiler-generated field
        if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__3 = CallSite<\u003C\u003EF\u007B00000200\u007D<CallSite, Type, object, double, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "TryParse", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: variable of a compiler-generated type
        \u003C\u003EF\u007B00000200\u007D<CallSite, Type, object, double, object> target4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<\u003C\u003EF\u007B00000200\u007D<CallSite, Type, object, double, object>> p3 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__3;
        Type type = typeof (double);
        // ISSUE: reference to a compiler-generated field
        if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, char[], object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "TrimEnd", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__2.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__2, targetValue, new char[2]
        {
          '%',
          ' '
        });
        ref double local = ref targetValue1;
        object obj6 = target4((CallSite) p3, type, obj5, ref local);
        obj3 = target3((CallSite) p4, obj4, obj6);
      }
      else
        obj3 = obj2;
      object obj7 = obj3;
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      object obj8;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__7.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__7, obj7))
      {
        // ISSUE: reference to a compiler-generated field
        if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj8 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__6.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__6, obj7, targetValue1 >= 0.0);
      }
      else
        obj8 = obj7;
      object obj9 = obj8;
      // ISSUE: reference to a compiler-generated field
      if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      object obj10;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__9.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__9, obj9))
      {
        // ISSUE: reference to a compiler-generated field
        if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj10 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__8.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__4.\u003C\u003Ep__8, obj9, targetValue1 <= 100.0);
      }
      else
        obj10 = obj9;
      if (target1((CallSite) p10, obj10))
        return targetValue1;
      throw new Exception(string.Format("Error while parsing the target for diff coverage status check: {0}", targetValue));
    }

    public Dictionary<string, double> GetFolderTargetsFromYml(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      Dictionary<string, object> ciData)
    {
      Dictionary<string, double> folderTargetsFromYml = new Dictionary<string, double>();
      if (pipelineContext.CodeCoverageSettings == null)
        pipelineContext.CodeCoverageSettings = new CoverageStatusCheckConfiguration();
      AzureReposProvider azureReposProvider = new AzureReposProvider();
      int num1 = 0;
      double num2 = 0.0;
      TestManagementRequestContext tcmRequestContext1 = tcmRequestContext;
      PipelineContext pipelineContext1 = pipelineContext;
      string sourceVersion = pipelineContext.SourceVersion;
      using (Stream file = azureReposProvider.GetFile(tcmRequestContext1, pipelineContext1, sourceVersion, "azurepipelines-coverage.yml"))
      {
        using (StreamReader input = new StreamReader(file))
        {
          object obj1 = new DeserializerBuilder().WithNodeTypeResolver((INodeTypeResolver) new YmlCoverageStatusCheckConfigurationProvider.ExpandoNodeTypeResolver(), (Action<IRegistrationLocationSelectionSyntax<INodeTypeResolver>>) (ls => ls.InsteadOf<DefaultContainersNodeTypeResolver>())).Build().Deserialize((TextReader) input);
          // ISSUE: reference to a compiler-generated field
          if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__2.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p2 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__2;
          // ISSUE: reference to a compiler-generated field
          if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__1 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ContainsProperty", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object> target2 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__1.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>> p1 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__1;
          // ISSUE: reference to a compiler-generated field
          if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__0, obj1);
          object obj3 = target2((CallSite) p1, this, obj2, "folder");
          if (target1((CallSite) p2, obj3))
          {
            pipelineContext.CodeCoverageSettings.IsFolderLevelPolicyEnabled = true;
            // ISSUE: reference to a compiler-generated field
            if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__17 == null)
            {
              // ISSUE: reference to a compiler-generated field
              YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (YmlCoverageStatusCheckConfigurationProvider)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, IEnumerable> target3 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__17.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, IEnumerable>> p17 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__17;
            // ISSUE: reference to a compiler-generated field
            if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__16 == null)
            {
              // ISSUE: reference to a compiler-generated field
              YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "folder", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object> target4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__16.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object>> p16 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__16;
            // ISSUE: reference to a compiler-generated field
            if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__15 == null)
            {
              // ISSUE: reference to a compiler-generated field
              YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "coverage", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__15.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__15, obj1);
            object obj5 = target4((CallSite) p16, obj4);
            foreach (object obj6 in target3((CallSite) p17, obj5))
            {
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__4 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target5 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__4.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p4 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__4;
              // ISSUE: reference to a compiler-generated field
              if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__3 == null)
              {
                // ISSUE: reference to a compiler-generated field
                YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__3 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ContainsProperty", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj7 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__3.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__3, this, obj6, "path");
              if (target5((CallSite) p4, obj7))
              {
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__6 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target6 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__6.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p6 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__6;
                // ISSUE: reference to a compiler-generated field
                if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__5 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__5 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ContainsProperty", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj8 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__5.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__5, this, obj6, "target");
                if (target6((CallSite) p6, obj8))
                {
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__8 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__8 = CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "GetTargetValue", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, object> target7 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__8.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, YmlCoverageStatusCheckConfigurationProvider, object, object>> p8 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__8;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__7 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "target", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj9 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__7.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__7, obj6);
                  object obj10 = target7((CallSite) p8, this, obj9);
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__10 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__10 = CallSite<Action<CallSite, Dictionary<string, double>, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Action<CallSite, Dictionary<string, double>, object, object> target8 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__10.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Action<CallSite, Dictionary<string, double>, object, object>> p10 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__10;
                  Dictionary<string, double> dictionary = folderTargetsFromYml;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__9 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "path", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj11 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__9.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__9, obj6);
                  object obj12 = obj10;
                  target8((CallSite) p10, dictionary, obj11, obj12);
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__12 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, double>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (double), typeof (YmlCoverageStatusCheckConfigurationProvider)));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, double> target9 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__12.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, double>> p12 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__12;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__11 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__11 = CallSite<Func<CallSite, double, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.AddAssign, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj13 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__11.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__11, num2, obj10);
                  num2 = target9((CallSite) p12, obj13);
                  ++num1;
                }
                else
                {
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__14 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__14 = CallSite<Action<CallSite, Dictionary<string, double>, object, double>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Action<CallSite, Dictionary<string, double>, object, double> target10 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__14.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Action<CallSite, Dictionary<string, double>, object, double>> p14 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__14;
                  Dictionary<string, double> dictionary = folderTargetsFromYml;
                  // ISSUE: reference to a compiler-generated field
                  if (YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__13 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "path", typeof (YmlCoverageStatusCheckConfigurationProvider), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj14 = YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__13.Target((CallSite) YmlCoverageStatusCheckConfigurationProvider.\u003C\u003Eo__5.\u003C\u003Ep__13, obj6);
                  double? coverageThreshold = pipelineContext.CodeCoverageSettings.DiffCoverageThreshold;
                  double num3;
                  if (!coverageThreshold.HasValue)
                  {
                    num3 = 70.0;
                  }
                  else
                  {
                    coverageThreshold = pipelineContext.CodeCoverageSettings.DiffCoverageThreshold;
                    num3 = coverageThreshold.Value;
                  }
                  target10((CallSite) p14, dictionary, obj14, num3);
                }
              }
            }
          }
          ciData.Add("IsFolderLevelPolicyAdded", (object) pipelineContext.CodeCoverageSettings.IsFolderLevelPolicyEnabled);
          ciData.Add("NumberOfFolderLevelPolicyConfigured", (object) folderTargetsFromYml.Count);
          ciData.Add("NumberOfCustomThresholdFolders", (object) num1);
          ciData.Add("AverageCustomFolderThreshold", (object) (num2 / (double) num1));
        }
      }
      return folderTargetsFromYml;
    }

    public class ExpandoNodeTypeResolver : INodeTypeResolver
    {
      public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
      {
        if (currentType == typeof (object))
        {
          switch (nodeEvent)
          {
            case SequenceStart _:
              currentType = typeof (List<object>);
              return true;
            case MappingStart _:
              currentType = typeof (ExpandoObject);
              return true;
          }
        }
        return false;
      }
    }
  }
}
