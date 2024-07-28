// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.ScopeTokenHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class ScopeTokenHelper
  {
    public static readonly Guid DistributedTaskSecurityNamespaceId = new Guid("101EAE8C-1709-47F9-B228-0E476C35B3BA");
    public const string PipelinesSdkServiceIdentity = "PipelinesSDK";
    public static readonly Guid PipelinesSdkServiceIdentityGuid = new Guid("ECEDF3A5-9275-4C34-B9E8-15454CCED81B");
    public const string UpdateTimelineRecordScopePrefix = "UpdateTimelineRecord";
    public const string GetReferencedResourcesScopePrefix = "GetReferencedResources";

    public static string CreateUpdateTimelineRecordToken(Guid planId, Guid recordId) => string.Format("{0}/plans/{1}/records/{2}", (object) "UpdateTimelineRecord", (object) planId, (object) recordId);

    public static string CreateGetReferencedResourcesToken(Guid planId, string nodeId) => string.Format("{0}/plans/{1}/nodes/{2}", (object) "GetReferencedResources", (object) planId, (object) nodeId);

    public static Microsoft.VisualStudio.Services.Identity.Identity GetPipelinesSdkIdentity(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return IdentityHelper.GetFrameworkIdentity(vssRequestContext, FrameworkIdentityType.ServiceIdentity, "PipelinesSDK", ScopeTokenHelper.PipelinesSdkServiceIdentityGuid.ToString("D")) ?? vssRequestContext.GetService<IdentityService>().CreateFrameworkIdentity(vssRequestContext, FrameworkIdentityType.ServiceIdentity, "PipelinesSDK", ScopeTokenHelper.PipelinesSdkServiceIdentityGuid.ToString("D"), "PipelinesSDK");
    }
  }
}
