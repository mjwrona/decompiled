// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestBuildChangesCatalogService
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class TestBuildChangesCatalogService : ITestBuildChangesCatalogService, IVssFrameworkService
  {
    private const string CSFileExtension = ".cs";
    private const string VBFileExtension = ".vb";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TestImpactBuildData QueryCodeChanges(
      TestImpactRequestContext requestContext,
      Guid projectId,
      DefinitionRunInfo definitionRunInfo)
    {
      int definitionId = definitionRunInfo.DefinitionType != Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build ? TestImpactServer.GetReleaseDetail(requestContext, definitionRunInfo.DefinitionRunId, projectId).ReleaseDefinitionReference.Id : TestImpactServer.GetBuildDetail(requestContext, projectId, definitionRunInfo.DefinitionRunId).Definition.Id;
      using (CodeChangesDataBase component = requestContext.RequestContext.CreateComponent<CodeChangesDataBase>())
        return component.QueryCodeChanges(projectId, (int) definitionRunInfo.DefinitionType, definitionId, definitionRunInfo.DefinitionRunId);
    }

    public void PublishBuildChanges(
      TestImpactRequestContext requestContext,
      Guid projectId,
      DefinitionRunInfo definitionRunInfo,
      TestImpactBuildData testImpactBuildData)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      requestContext1.Trace(15113003, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "PublishBuildChanges: ProjectId: {0} Buildid: {0}", (object) projectId, (object) definitionRunInfo.DefinitionRunId);
      try
      {
        if (definitionRunInfo.DefinitionType == Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build)
        {
          Microsoft.TeamFoundation.Build.WebApi.Build buildDetail = TestImpactServer.GetBuildDetail(requestContext, projectId, definitionRunInfo.DefinitionRunId);
          this.PublishCodeChanges(requestContext1, projectId, definitionRunInfo.DefinitionRunId, buildDetail.Definition.Id, definitionRunInfo.DefinitionType, testImpactBuildData);
        }
        else
        {
          if (definitionRunInfo.DefinitionType != Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Release)
            return;
          Release releaseDetail = TestImpactServer.GetReleaseDetail(requestContext, definitionRunInfo.DefinitionRunId, projectId);
          this.PublishCodeChanges(requestContext1, projectId, definitionRunInfo.DefinitionRunId, releaseDetail.ReleaseDefinitionReference.Id, definitionRunInfo.DefinitionType, testImpactBuildData);
        }
      }
      catch (Exception ex)
      {
        requestContext1.TraceException(15113005, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, ex);
      }
    }

    private void PublishCodeChanges(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      int buildId,
      int definitionId,
      Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType definitionType,
      TestImpactBuildData testImpactBuildData)
    {
      Dictionary<string, object> eventData = new Dictionary<string, object>()
      {
        {
          TestImpactServiceCIProperty.DefinitionRunId,
          (object) buildId
        },
        {
          TestImpactServiceCIProperty.RebaseLimit,
          (object) testImpactBuildData.RebaseLimit
        }
      };
      Stopwatch stopwatch = Stopwatch.StartNew();
      tfsRequestContext.Trace(15113004, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "PublishCodeChanges: Using new test impact flow. Total changes: {0}", (object) ((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>) testImpactBuildData.CodeChanges).Count<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>());
      SignatureType signatureType = SignatureType.Method;
      if (testImpactBuildData.CodeChanges != null && ((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>) testImpactBuildData.CodeChanges).Count<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>() > 0)
      {
        eventData.Add(TestImpactServiceCIProperty.CodeChangesCount, (object) ((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>) testImpactBuildData.CodeChanges).Count<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>());
        FileTypeInfo codeChangeDetails = this.GetCodeChangeDetails(testImpactBuildData);
        eventData.Add(TestImpactServiceCIProperty.KnownTypeFileCount, (object) codeChangeDetails.KnownTypeCount);
        eventData.Add(TestImpactServiceCIProperty.UnknownTypeFileCount, (object) codeChangeDetails.UnknownTypeCount);
        signatureType = ((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>) testImpactBuildData.CodeChanges).ElementAt<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>(0).SignatureType;
      }
      using (CodeChangesDataBase component = tfsRequestContext.CreateComponent<CodeChangesDataBase>())
      {
        if (testImpactBuildData.RebaseLimit > 0)
          component.PublishCodeChanges(projectId, (int) definitionType, definitionId, buildId, (int) signatureType, (IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>) testImpactBuildData.CodeChanges, testImpactBuildData.RebaseLimit);
        else
          component.PublishCodeChanges(projectId, (int) definitionType, definitionId, buildId, (int) signatureType, (IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>) testImpactBuildData.CodeChanges, 50);
      }
      stopwatch.Stop();
      eventData.Add(TestImpactServiceCIProperty.TimeTakenToPublishCodeChanges, (object) stopwatch.ElapsedMilliseconds);
      CILogger.Instance.PublishCI(tfsRequestContext, TestImpactServiceCIFeature.PublishCodeChanges, eventData);
    }

    private FileTypeInfo GetCodeChangeDetails(TestImpactBuildData testImpactBuildData)
    {
      FileTypeInfo codeChangeDetails = new FileTypeInfo();
      foreach (Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange codeChange in testImpactBuildData.CodeChanges)
      {
        if (codeChange.FileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || codeChange.FileName.EndsWith(".vb", StringComparison.OrdinalIgnoreCase))
          ++codeChangeDetails.KnownTypeCount;
        else
          ++codeChangeDetails.UnknownTypeCount;
      }
      return codeChangeDetails;
    }
  }
}
