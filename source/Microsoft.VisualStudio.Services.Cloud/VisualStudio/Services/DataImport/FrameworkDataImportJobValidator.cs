// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.FrameworkDataImportJobValidator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DataImport
{
  internal class FrameworkDataImportJobValidator : IFrameworkDataImportRequestVisitor<bool>
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly TeamFoundationJobDefinition m_jobDefinition;

    internal FrameworkDataImportJobValidator(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      this.m_requestContext = requestContext;
      this.m_jobDefinition = jobDefinition;
    }

    public bool Visit(CreateCollectionDataImportRequest request) => true;

    public bool Visit(DatabaseDataImportRequest request) => true;

    public bool Visit(FileCopyDataImportRequest request) => true;

    public bool Visit(HostUpgradeDataImportRequest request) => true;

    public bool Visit(OnlinePostHostUpgradeDataImportRequest request) => true;

    public bool Visit(StopHostAfterUpgradeDataImportRequest request) => true;

    public bool Visit(ObtainDatabaseHoldDataImportRequest request) => true;

    public bool Visit(HostMoveDataImportRequest request) => true;

    public bool Visit(ActivateDataImportRequest request) => true;

    public bool Visit(DataImportDehydrateRequest request) => true;

    public bool Visit(RemoveDataImportRequest request)
    {
      if (this.m_jobDefinition != null)
        return true;
      IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      IVssRequestContext requestContext = !(vssRequestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS) ? vssRequestContext : throw new DataImportEntryDoesNotExistException(request.RequestId);
      Guid hostId = request.HostId;
      TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(requestContext, hostId);
      if (serviceHostProperties != null)
      {
        if (serviceHostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
          throw new DataImportUnexpectedRemovalException(string.Format("Expected host to be in stopped state. {0}", (object) serviceHostProperties));
        if (vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMappings(vssRequestContext, request.HostId).Any<HostInstanceMapping>((Func<HostInstanceMapping, bool>) (x => x.Status != 0)))
          throw new DataImportUnexpectedRemovalException("Expected no active/moved instance allocations for the host");
      }
      return true;
    }
  }
}
