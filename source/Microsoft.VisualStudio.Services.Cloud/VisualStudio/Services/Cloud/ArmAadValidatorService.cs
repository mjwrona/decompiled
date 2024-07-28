// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ArmAadValidatorService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.WindowsAzure.Arm.Infra.ARMAuthenticationAAD;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ArmAadValidatorService : IArmAadValidatorService, IVssFrameworkService
  {
    private IAadTokenValidator m_aadTokenValidator;
    private static readonly string[] s_tags = new string[1]
    {
      "ArmAadValidator"
    };
    private const string c_area = "ArmAadValidatorService";
    private const string c_layer = "Service";

    public ArmAadValidatorService()
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_aadTokenValidator = (IAadTokenValidator) new ArmAadTokenValidator(systemRequestContext.ExecutionEnvironment.IsCloudDeployment ? (ArmAadTokenValidatorEnum.CloudEnvironment) 1 : (ArmAadTokenValidatorEnum.CloudEnvironment) 0, new Guid(systemRequestContext.GetService<IAadConfigurationService>().GetSettings(systemRequestContext).ClientId), new ArmAadTokenValidator.Logger((object) this, __methodptr(ArmLoggerFunction)));

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal ArmAadValidatorService(
      ArmAadTokenValidatorEnum.CloudEnvironment cloudEnvironment,
      Guid appGuid,
      ArmAadTokenValidator.Logger clientLoggerFunc)
    {
      this.m_aadTokenValidator = (IAadTokenValidator) new ArmAadTokenValidator(cloudEnvironment, appGuid, clientLoggerFunc);
    }

    public bool ApiProtectedWithProofOfPossession(
      string httpMethod,
      Uri signedRequestUri,
      string authHeader,
      Guid activityId,
      CancellationToken cancellationToken)
    {
      return this.m_aadTokenValidator.ApiProtectedWithProofOfPossession(httpMethod, signedRequestUri, authHeader, activityId, cancellationToken).SyncResult<bool>();
    }

    private void ArmLoggerFunction(string message)
    {
      if (!TeamFoundationTracingService.IsRawTracingEnabled(90003102, TraceLevel.Info, nameof (ArmAadValidatorService), "Service", ArmAadValidatorService.s_tags))
        return;
      TeamFoundationTracingService.TraceRaw(90003102, TraceLevel.Info, nameof (ArmAadValidatorService), "Service", message);
    }
  }
}
