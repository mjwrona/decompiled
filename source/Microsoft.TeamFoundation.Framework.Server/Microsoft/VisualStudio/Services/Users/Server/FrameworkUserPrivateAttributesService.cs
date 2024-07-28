// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.FrameworkUserPrivateAttributesService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  internal class FrameworkUserPrivateAttributesService : 
    IUserPrivateAttributesService,
    IVssFrameworkService
  {
    private static readonly string s_Area = "User";
    private static readonly string s_Layer = nameof (FrameworkUserPrivateAttributesService);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public UserAttribute GetPrivateAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName)
    {
      requestContext.TraceEnter(348274690, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (GetPrivateAttribute));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().GetPrivateAttribute(vssRequestContext, descriptor, attributeName);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return FrameworkUserPrivateAttributesService.GetClient(requestContext).GetPrivateAttributeAsync(descriptor, attributeName, cancellationToken: requestContext.CancellationToken).SyncResult<UserAttribute>();
      }
      finally
      {
        requestContext.TraceLeave(962477082, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (GetPrivateAttribute));
      }
    }

    public IList<UserAttribute> QueryPrivateAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null)
    {
      requestContext.TraceEnter(1765500404, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (QueryPrivateAttributes));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().QueryPrivateAttributes(vssRequestContext, descriptor, queryPattern, modifiedAfter);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return (IList<UserAttribute>) FrameworkUserPrivateAttributesService.GetClient(requestContext).QueryPrivateAttributesAsync(descriptor, queryPattern, modifiedAfter, (object) requestContext.CancellationToken).SyncResult<List<UserAttribute>>();
      }
      finally
      {
        requestContext.TraceLeave(1422542645, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (QueryPrivateAttributes));
      }
    }

    public IList<UserAttribute> SetPrivateAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      IList<SetUserAttributeParameters> attributeParametersList)
    {
      requestContext.TraceEnter(213996241, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (SetPrivateAttributes));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().SetPrivateAttributes(vssRequestContext, descriptor, attributeParametersList);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return (IList<UserAttribute>) FrameworkUserPrivateAttributesService.GetClient(requestContext).SetPrivateAttributesAsync(descriptor, (IEnumerable<SetUserAttributeParameters>) attributeParametersList, cancellationToken: requestContext.CancellationToken).SyncResult<List<UserAttribute>>();
      }
      finally
      {
        requestContext.TraceLeave(1853158033, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (SetPrivateAttributes));
      }
    }

    public void DeletePrivateAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName)
    {
      requestContext.TraceEnter(1381498745, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (DeletePrivateAttribute));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().DeletePrivateAttribute(vssRequestContext, descriptor, attributeName);
        }
        else
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
            FrameworkUserPrivateAttributesService.GetClient(requestContext).DeletePrivateAttributeAsync(descriptor, attributeName, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      finally
      {
        requestContext.TraceLeave(842669120, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (DeletePrivateAttribute));
      }
    }

    public UserAttribute GetPrivateAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName)
    {
      requestContext.TraceEnter(1639793581, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (GetPrivateAttribute));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().GetPrivateAttribute(vssRequestContext, userId, attributeName);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return FrameworkUserPrivateAttributesService.GetClient(requestContext).GetPrivateAttributeAsync(userId, attributeName, cancellationToken: requestContext.CancellationToken).SyncResult<UserAttribute>();
      }
      finally
      {
        requestContext.TraceLeave(1410485391, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (GetPrivateAttribute));
      }
    }

    public IList<UserAttribute> QueryPrivateAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null)
    {
      requestContext.TraceEnter(1839830635, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (QueryPrivateAttributes));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().QueryPrivateAttributes(vssRequestContext, userId, queryPattern, modifiedAfter);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return (IList<UserAttribute>) FrameworkUserPrivateAttributesService.GetClient(requestContext).QueryPrivateAttributesAsync(userId, queryPattern, modifiedAfter, (object) requestContext.CancellationToken).SyncResult<List<UserAttribute>>();
      }
      finally
      {
        requestContext.TraceLeave(1363600773, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (QueryPrivateAttributes));
      }
    }

    public IList<UserAttribute> SetPrivateAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      IList<SetUserAttributeParameters> attributeParametersList)
    {
      requestContext.TraceEnter(2082708272, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (SetPrivateAttributes));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().SetPrivateAttributes(vssRequestContext, userId, attributeParametersList);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return (IList<UserAttribute>) FrameworkUserPrivateAttributesService.GetClient(requestContext).SetPrivateAttributesAsync(userId, (IEnumerable<SetUserAttributeParameters>) attributeParametersList, cancellationToken: requestContext.CancellationToken).SyncResult<List<UserAttribute>>();
      }
      finally
      {
        requestContext.TraceLeave(1943462283, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (SetPrivateAttributes));
      }
    }

    public void DeletePrivateAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName)
    {
      requestContext.TraceEnter(89903850, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (DeletePrivateAttribute));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<FrameworkUserPrivateAttributesService>().DeletePrivateAttribute(vssRequestContext, userId, attributeName);
        }
        else
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
            FrameworkUserPrivateAttributesService.GetClient(requestContext).DeletePrivateAttributeAsync(userId, attributeName, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      finally
      {
        requestContext.TraceLeave(786614657, FrameworkUserPrivateAttributesService.s_Area, FrameworkUserPrivateAttributesService.s_Layer, nameof (DeletePrivateAttribute));
      }
    }

    private static FrameworkUserPrivateHttpClient GetClient(IVssRequestContext requestContext) => requestContext.GetClient<FrameworkUserPrivateHttpClient>();
  }
}
