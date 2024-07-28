// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceTemplateComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecurityNamespaceTemplateComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<SecurityNamespaceTemplateComponent>(1, true),
      (IComponentCreator) new ComponentCreator<SecurityNamespaceTemplateComponent2>(2)
    }, "SecurityNamespaceTemplate");
    private bool? m_isSpsOrMps;

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_isSpsOrMps = new bool?(requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS || requestContext.ServiceInstanceType() == ServiceInstanceTypes.MPS);
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public virtual IReadOnlyList<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> QuerySecurityNamespaceTemplates(
      out long sequenceId)
    {
      sequenceId = 0L;
      if (!this.m_isSpsOrMps.HasValue)
        throw new InvalidOperationException();
      if (!this.RequestContext.ExecutionEnvironment.IsHostedDeployment || this.m_isSpsOrMps.Value)
        return (IReadOnlyList<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>) Array.Empty<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>();
      return (IReadOnlyList<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>) new List<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>()
      {
        new SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate(2, new Guid("5A27515B-CCD7-42C9-84F1-54C998F03866"), "<RemoteSecurityNamespaceDescription serviceOwner=\"951917AC-A960-4999-8464-E3F0AA25B381\" />"),
        new SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate(2, new Guid("9BF6F8D7-772D-4C01-93F6-652A7A44CEF1"), "<RemoteSecurityNamespaceDescription serviceOwner=\"951917AC-A960-4999-8464-E3F0AA25B381\" />"),
        new SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate(4, new Guid("5A27515B-CCD7-42C9-84F1-54C998F03866"), "<RemoteSecurityNamespaceDescription serviceOwner=\"6D6BE6BD-5AC8-4785-BA3D-127AFAB45FCC\" />")
      }.AsReadOnly();
    }

    public virtual long UpdateSecurityNamespaceTemplates(
      IEnumerable<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> templates)
    {
      throw new NotImplementedException();
    }

    public virtual long DeleteSecurityNamespaceTemplates(
      IEnumerable<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> templates)
    {
      throw new NotImplementedException();
    }

    public class SecurityNamespaceTemplate
    {
      public readonly int HostType;
      public readonly Guid NamespaceId;
      public readonly string Description;

      public SecurityNamespaceTemplate(int hostType, Guid namespaceId, string description = null)
      {
        this.HostType = hostType;
        this.NamespaceId = namespaceId;
        this.Description = description;
      }
    }
  }
}
