// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityTemplateComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityTemplateComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<SecurityTemplateComponent>(1, true),
      (IComponentCreator) new ComponentCreator<SecurityTemplateComponent2>(2)
    }, "SecurityTemplate");
    private bool? m_isTfs;
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static SecurityTemplateComponent() => SecurityTemplateComponent.s_sqlExceptionFactories.Add(800401, new SqlExceptionFactory(typeof (CannotUpdateSecurityTemplateEntryAtInstallTimeException)));

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_isTfs = new bool?(requestContext.ServiceInstanceType() == ServiceInstanceTypes.TFS);
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public virtual IReadOnlyList<SecurityTemplateComponent.SecurityTemplateEntry> QuerySecurityTemplateEntries(
      out long sequenceId)
    {
      sequenceId = 0L;
      if (!this.m_isTfs.HasValue)
        throw new InvalidOperationException();
      if (!this.RequestContext.ExecutionEnvironment.IsHostedDeployment || !this.m_isTfs.Value)
        return (IReadOnlyList<SecurityTemplateComponent.SecurityTemplateEntry>) Array.Empty<SecurityTemplateComponent.SecurityTemplateEntry>();
      return (IReadOnlyList<SecurityTemplateComponent.SecurityTemplateEntry>) new List<SecurityTemplateComponent.SecurityTemplateEntry>()
      {
        new SecurityTemplateComponent.SecurityTemplateEntry(new Guid("F1C8B843-9DC0-48A4-ABE6-0BFD13BA21FA"), 4, new Guid("2E9EB7ED-3C0A-47D4-87C1-0FFDD275FD87"), WellKnownAclStores.System)
        {
          TokenTemplate = "{ \"type\": \"literal\", \"value\": \"repoV2/\" }",
          SubjectTemplate = "{ \"type\": \"servicePrincipal\", \"value\": \"0000000F-0000-8888-8000-000000000000\" }",
          Allow = 2,
          Deny = 0
        },
        new SecurityTemplateComponent.SecurityTemplateEntry(new Guid("3C5B5E2C-A87C-4073-9B47-91278313C540"), 4, new Guid("2E9EB7ED-3C0A-47D4-87C1-0FFDD275FD87"), WellKnownAclStores.System)
        {
          TokenTemplate = "{ \"type\": \"literal\", \"value\": \"repoV2/\" }",
          SubjectTemplate = "{ \"type\": \"servicePrincipal\", \"value\": \"00000010-0000-8888-8000-000000000000\" }",
          Allow = 2,
          Deny = 0
        },
        new SecurityTemplateComponent.SecurityTemplateEntry(new Guid("55C63ED8-EAD5-41FC-91CA-F4930AE1A4D6"), 4, new Guid("3C15A8B7-AF1A-45C2-AA97-2CB97078332E"), WellKnownAclStores.System)
        {
          TokenTemplate = "{ \"type\": \"literal\", \"value\": \"$/\" }",
          SubjectTemplate = "{ \"type\": \"servicePrincipal\", \"value\": \"0000000F-0000-8888-8000-000000000000\" }",
          Allow = 1,
          Deny = 0
        },
        new SecurityTemplateComponent.SecurityTemplateEntry(new Guid("EE108DB9-EED4-48CF-BCC1-DB83B9FC8135"), 4, new Guid("3C15A8B7-AF1A-45C2-AA97-2CB97078332E"), WellKnownAclStores.System)
        {
          TokenTemplate = "{ \"type\": \"literal\", \"value\": \"$/\" }",
          SubjectTemplate = "{ \"type\": \"servicePrincipal\", \"value\": \"00000010-0000-8888-8000-000000000000\" }",
          Allow = 1,
          Deny = 0
        }
      }.AsReadOnly();
    }

    public virtual long UpdateSecurityTemplateEntries(
      IEnumerable<SecurityTemplateComponent.SecurityTemplateEntry> entries)
    {
      throw new NotImplementedException();
    }

    public virtual long DeleteSecurityTemplateEntries(IEnumerable<Guid> ids) => throw new NotImplementedException();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SecurityTemplateComponent.s_sqlExceptionFactories;

    public class SecurityTemplateEntry
    {
      public readonly Guid Id;
      public readonly int HostType;
      public readonly Guid NamespaceId;
      public readonly Guid AclStoreId;
      public string TokenTemplate;
      public string SubjectTemplate;
      public int Allow;
      public int Deny;

      public SecurityTemplateEntry(Guid id, int hostType, Guid namespaceId, Guid aclStoreId)
      {
        this.Id = id;
        this.HostType = hostType;
        this.NamespaceId = namespaceId;
        this.AclStoreId = aclStoreId;
      }

      private SecurityTemplateEntry(Guid id) => this.Id = id;

      public static SecurityTemplateComponent.SecurityTemplateEntry CreateDeletionRequest(Guid id) => new SecurityTemplateComponent.SecurityTemplateEntry(id);
    }
  }
}
