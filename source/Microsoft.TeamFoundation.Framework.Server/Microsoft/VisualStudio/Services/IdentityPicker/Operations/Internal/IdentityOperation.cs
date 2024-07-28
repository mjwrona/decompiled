// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.IdentityOperation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  public sealed class IdentityOperation
  {
    private static IOperationResponse Invoke(
      IVssRequestContext requestContext,
      IOperationRequest request)
    {
      try
      {
        Tracing.TraceEnter(requestContext, 701, request.GetType().Name + ".Validate()");
        request.Validate(requestContext);
      }
      catch (Exception ex)
      {
        Tracing.TraceException(requestContext, 708, ex);
        if (!(ex is IdentityPickerException))
          throw new IdentityPickerValidateException("Invalid request: " + request.GetType().Name, ex);
        throw;
      }
      finally
      {
        Tracing.TraceLeave(requestContext, 709, request.GetType().Name + ".Validate()");
      }
      try
      {
        Tracing.TraceEnter(requestContext, 711, request.GetType().Name + ".Process()");
        return request.Process(requestContext);
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityPickerAuthorizationException))
          Tracing.TraceException(requestContext, 718, ex);
        if (!(ex is IdentityPickerException))
          throw new IdentityPickerProcessException("Process exception in: " + request.GetType().Name, ex);
        throw;
      }
      finally
      {
        Tracing.TraceLeave(requestContext, 719, request.GetType().Name + ".Process()");
      }
    }

    public static SearchResponse Search(IVssRequestContext requestContext, SearchRequest request) => IdentityOperation.Invoke(requestContext, (IOperationRequest) request) as SearchResponse;

    internal static GetAvatarResponse GetAvatar(
      IVssRequestContext requestContext,
      GetAvatarRequest request)
    {
      return IdentityOperation.Invoke(requestContext, (IOperationRequest) request) as GetAvatarResponse;
    }

    internal static GetConnectionsResponse GetConnections(
      IVssRequestContext requestContext,
      GetConnectionsRequest request)
    {
      return IdentityOperation.Invoke(requestContext, (IOperationRequest) request) as GetConnectionsResponse;
    }

    internal static GetMruResponse GetMru(IVssRequestContext requestContext, GetMruRequest request) => IdentityOperation.Invoke(requestContext, (IOperationRequest) request) as GetMruResponse;

    internal static PatchMruResponse PatchMru(
      IVssRequestContext requestContext,
      PatchMruRequest request)
    {
      return IdentityOperation.Invoke(requestContext, (IOperationRequest) request) as PatchMruResponse;
    }
  }
}
