// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Licensing.FrameworkLicensingRightsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Licensing.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Licensing
{
  public class FrameworkLicensingRightsService : ILicensingRightsService, IVssFrameworkService
  {
    private readonly CommandPropertiesSetter cbWith10SecondTimeout = new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 10));
    internal const string s_preValidateTransferIdentityRightsCommandKey = "PreValidateTransferIdentityRights";
    internal const string s_transferIdentityRightsCommandKey = "TransferIdentityRights";
    private const string area = "Licensing";
    private const string layer = "FrameworkLicensingRightsService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void PreValidateTransferIdentityRights(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      this.WrapCircuitBreaker(requestContext, (Action) (() => this.GetHttpClient(requestContext).TransferIdentityRightsAsync(userIdTransferMap, new bool?(true)).SyncResult()), nameof (PreValidateTransferIdentityRights));
    }

    public void TransferIdentityRights(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      this.WrapCircuitBreaker(requestContext, (Action) (() => this.GetHttpClient(requestContext).TransferIdentityRightsAsync(userIdTransferMap, new bool?(false)).SyncResult()), nameof (TransferIdentityRights));
    }

    private protected virtual IAccountLicensingHttpClient GetHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetAccountLicensingHttpClient();
    }

    private void WrapCircuitBreaker(
      IVssRequestContext requestContext,
      Action action,
      string commandKey)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(this.cbWith10SecondTimeout);
      CommandService commandService = new CommandService(requestContext, setter, action, (Action) (() => requestContext.TraceAlways(3786399, TraceLevel.Error, "Licensing", nameof (FrameworkLicensingRightsService), "Encountered error in FrameworkLicensingRightsService")));
      try
      {
        commandService.Execute();
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new LicenseServiceUnavailableException(ex.Message);
      }
    }
  }
}
