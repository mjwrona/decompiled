// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.IContributionFilterService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [DefaultServiceImplementation(typeof (ContributionService))]
  public interface IContributionFilterService : IVssFrameworkService
  {
    bool ApplyConstraints(
      IVssRequestContext requestContext,
      Contribution contribution,
      string phase,
      string relationship,
      ICollection<EvaluatedCondition> evaluatedConditions,
      ConstraintEvaluationOptions options = ConstraintEvaluationOptions.None,
      Func<ContributionConstraint, Contribution, bool?> evaluationCallback = null);
  }
}
