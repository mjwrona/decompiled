// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.MruServiceAdapter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity.Mru;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class MruServiceAdapter : IdentityProvider
  {
    internal override IList<Guid> GetIdentities(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.IdentityPicker.Tracing.TraceEnter(requestContext, 531, "MRU.GetIdentities");
      try
      {
        Guid userId = requestContext.GetUserId(true);
        IVssRequestContext context = requestContext.Elevate();
        return (IList<Guid>) context.GetService<IdentityMruService>().ReadMruIdentities(context, userId, IdentityMruService.SharedDefaultContainerId).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (id => id.Id)).ToList<Guid>();
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityPickerException))
          throw new IdentityPickerAdapterException("MRU.GetIdentities:" + ex.Message, ex);
        throw;
      }
      finally
      {
        Microsoft.VisualStudio.Services.IdentityPicker.Tracing.TraceLeave(requestContext, 539, "MRU.GetIdentities");
      }
    }

    internal override bool RemoveIdentities(
      IVssRequestContext requestContext,
      IList<string> identityIds)
    {
      Microsoft.VisualStudio.Services.IdentityPicker.Tracing.TraceEnter(requestContext, 551, "MRU.RemoveIdentities");
      try
      {
        if (identityIds == null || identityIds.Count == 0)
          throw new IdentityPickerArgumentException("MRU IdentityIds cannot be null or empty");
        Guid userId = requestContext.GetUserId(true);
        List<Guid> identityIds1 = MruServiceAdapter.ParseIdentityIds(identityIds);
        IVssRequestContext context = requestContext.Elevate();
        context.GetService<IdentityMruService>().RemoveMruIdentities(context, userId, IdentityMruService.SharedDefaultContainerId, (IList<Guid>) identityIds1);
        return true;
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityPickerException))
          throw new IdentityPickerAdapterException("MRU.RemoveIdentities:" + ex.Message, ex);
        throw;
      }
      finally
      {
        Microsoft.VisualStudio.Services.IdentityPicker.Tracing.TraceLeave(requestContext, 559, "MRU.RemoveIdentities");
      }
    }

    internal override bool AddIdentities(
      IVssRequestContext requestContext,
      IList<string> identityIds)
    {
      Microsoft.VisualStudio.Services.IdentityPicker.Tracing.TraceEnter(requestContext, 541, "MRU.AddIdentities");
      try
      {
        if (identityIds == null || identityIds.Count == 0)
          throw new IdentityPickerArgumentException("MRU IdentityIds cannot be null or empty");
        Guid userId = requestContext.GetUserId(true);
        List<Guid> identityIds1 = MruServiceAdapter.ParseIdentityIds(identityIds);
        IVssRequestContext context = requestContext.Elevate();
        context.GetService<IdentityMruService>().AddMruIdentities(context, userId, IdentityMruService.SharedDefaultContainerId, (IList<Guid>) identityIds1);
        return true;
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityPickerException))
          throw new IdentityPickerAdapterException("MRU.AddIdentities:" + ex.Message, ex);
        throw;
      }
      finally
      {
        Microsoft.VisualStudio.Services.IdentityPicker.Tracing.TraceLeave(requestContext, 549, "MRU.AddIdentities");
      }
    }

    internal override IDictionary<string, QueryTokenResult> GetIdentities(
      IdentityProviderAdapterGetRequest ipdGetRequest)
    {
      throw new NotImplementedException();
    }

    internal override byte[] GetIdentityImage(
      IVssRequestContext requestContext,
      string objectId,
      Dictionary<string, object> options = null)
    {
      throw new NotImplementedException();
    }

    private static List<Guid> ParseIdentityIds(IList<string> identityIds)
    {
      List<Guid> identityIds1 = new List<Guid>();
      foreach (string identityId in (IEnumerable<string>) identityIds)
      {
        Guid result;
        if (Guid.TryParse(identityId, out result))
          identityIds1.Add(result);
      }
      return identityIds1;
    }
  }
}
