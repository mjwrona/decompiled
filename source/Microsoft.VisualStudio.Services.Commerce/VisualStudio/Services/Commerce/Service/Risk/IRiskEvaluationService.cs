// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.IRiskEvaluationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk
{
  [DefaultServiceImplementation(typeof (RiskEvaluationService))]
  public interface IRiskEvaluationService : IVssFrameworkService
  {
    RiskEvaluationResult GetRiskEvaluation(
      IVssRequestContext deploymentContext,
      string ipAddress,
      string userAgent,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid hostId,
      Guid? sessionId,
      Guid? tenantId,
      Guid? objectId,
      string galleryId,
      int changedQuantity);
  }
}
