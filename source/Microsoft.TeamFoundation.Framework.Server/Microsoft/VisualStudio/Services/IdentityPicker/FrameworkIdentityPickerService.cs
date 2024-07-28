// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.FrameworkIdentityPickerService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.IdentityPicker.Extensions;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkIdentityPickerService : IdentityPickerService
  {
    private Guid m_serviceHostId;
    private static IDictionary<Type, IList<IIdentityPickerExtension>> opTypeExtensionMapping;
    private const string c_IdentityPickerExtensionsFeatureFlag = "VisualStudio.Services.IdentityPicker.Extensions";

    public override void ServiceStart(IVssRequestContext requestContext)
    {
      Tracing.TraceEnter(requestContext, 601, nameof (ServiceStart));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.m_serviceHostId = requestContext.ServiceHost.InstanceId;
        FrameworkIdentityPickerService.LoadExtensions(requestContext);
      }
      catch (Exception ex)
      {
        Tracing.TraceException(requestContext, 608, ex);
        throw new IdentityPickerServiceException("FrameworkIdentityPickerService ServiceStart", ex);
      }
      finally
      {
        Tracing.TraceLeave(requestContext, 609, nameof (ServiceStart));
      }
    }

    public override void ServiceEnd(IVssRequestContext context)
    {
    }

    internal override SearchResponse Search(
      IVssRequestContext requestContext,
      SearchRequest request)
    {
      return this.Invoke(requestContext, (OperationRequest) request, nameof (Search)) as SearchResponse;
    }

    internal override GetAvatarResponse GetAvatar(
      IVssRequestContext requestContext,
      GetAvatarRequest request)
    {
      return this.Invoke(requestContext, (OperationRequest) request, nameof (GetAvatar)) as GetAvatarResponse;
    }

    internal override GetConnectionsResponse GetConnections(
      IVssRequestContext requestContext,
      GetConnectionsRequest request)
    {
      return this.Invoke(requestContext, (OperationRequest) request, nameof (GetConnections)) as GetConnectionsResponse;
    }

    internal override GetMruResponse GetMru(
      IVssRequestContext requestContext,
      GetMruRequest request)
    {
      return this.Invoke(requestContext, (OperationRequest) request, nameof (GetMru)) as GetMruResponse;
    }

    internal override PatchMruResponse PatchMru(
      IVssRequestContext requestContext,
      PatchMruRequest request)
    {
      return this.Invoke(requestContext, (OperationRequest) request, nameof (PatchMru)) as PatchMruResponse;
    }

    private static void LoadExtensions(IVssRequestContext requestContext)
    {
      if (FrameworkIdentityPickerService.opTypeExtensionMapping != null)
        return;
      Dictionary<Type, IList<IIdentityPickerExtension>> dictionary = new Dictionary<Type, IList<IIdentityPickerExtension>>();
      IDisposableReadOnlyList<IIdentityPickerExtension> extensions = requestContext.GetExtensions<IIdentityPickerExtension>(ExtensionLifetime.Service);
      if (extensions == null)
      {
        Tracing.TraceInfo(requestContext, 609, "GetExtensions is returning null instead of an empty IReadOnlyList");
      }
      else
      {
        foreach (IIdentityPickerExtension identityPickerExtension in (IEnumerable<IIdentityPickerExtension>) extensions)
        {
          IList<IIdentityPickerExtension> identityPickerExtensionList;
          if (!dictionary.TryGetValue(identityPickerExtension.GetExtendedOperationType(), out identityPickerExtensionList))
          {
            identityPickerExtensionList = (IList<IIdentityPickerExtension>) new List<IIdentityPickerExtension>();
            dictionary[identityPickerExtension.GetExtendedOperationType()] = identityPickerExtensionList;
          }
          identityPickerExtensionList.Add(identityPickerExtension);
        }
        FrameworkIdentityPickerService.opTypeExtensionMapping = (IDictionary<Type, IList<IIdentityPickerExtension>>) dictionary;
      }
    }

    private OperationResponse Invoke(
      IVssRequestContext requestContext,
      OperationRequest request,
      [CallerMemberName] string callerName = "")
    {
      Tracing.TraceEnter(requestContext, 611, callerName);
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      ArgumentUtility.CheckForNull<OperationRequest>(request, nameof (request));
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.IdentityPicker.Extensions"))
          return this.InvokeDefaultExtension(requestContext, request);
        Type type = request.GetType();
        IList<IIdentityPickerExtension> source;
        if (FrameworkIdentityPickerService.opTypeExtensionMapping == null || !FrameworkIdentityPickerService.opTypeExtensionMapping.TryGetValue(type, out source) || source == null || source.Count < 1)
          return this.InvokeDefaultExtension(requestContext, request);
        List<IIdentityPickerExtension> list = source.Where<IIdentityPickerExtension>((Func<IIdentityPickerExtension, bool>) (ext => FrameworkIdentityPickerService.CheckIfRequestAndExtensionIdsAreEqual(request, ext))).ToList<IIdentityPickerExtension>();
        if (list.Count < 1)
          return this.InvokeDefaultExtension(requestContext, request);
        IIdentityPickerExtension identityPickerExtension = list.Count <= 1 ? list.First<IIdentityPickerExtension>() : throw new IdentityPickerMultipleExtensionsException("Cannot have more than one extensions that match an incoming request by ExtensionId");
        return !identityPickerExtension.EvaluateApplicability(requestContext, request) ? this.InvokeDefaultExtension(requestContext, request) : identityPickerExtension.Invoke(requestContext, request);
      }
      catch (IdentityPickerImageRetrievalException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityPickerException))
          Tracing.TraceException(requestContext, 618, ex);
        if (ex != null && ex.InnerException is CircuitBreakerException)
          throw ex.InnerException;
        throw;
      }
      finally
      {
        Tracing.TraceLeave(requestContext, 619, callerName);
      }
    }

    private static bool CheckIfRequestAndExtensionIdsAreEqual(
      OperationRequest request,
      IIdentityPickerExtension extension)
    {
      object obj;
      Guid result;
      return request != null && request.ExtensionData != null && request.ExtensionData.Options != null && request.ExtensionData.Options.TryGetValue("ExtensionId", out obj) && obj != null && Guid.TryParse(obj.ToString(), out result) && result != Guid.Empty && result.Equals(extension.GetExtensionId());
    }

    private OperationResponse InvokeDefaultExtension(
      IVssRequestContext requestContext,
      OperationRequest request)
    {
      this.DefaultExtensionValidate(requestContext, request);
      return this.DefaultExtensionProcess(requestContext, request);
    }

    private void DefaultExtensionValidate(
      IVssRequestContext requestContext,
      OperationRequest request)
    {
      ArgumentUtility.CheckForNull<OperationRequest>(request, "IOperationRequest");
    }

    private OperationResponse DefaultExtensionProcess(
      IVssRequestContext requestContext,
      OperationRequest request)
    {
      return request.Invoke(requestContext);
    }
  }
}
