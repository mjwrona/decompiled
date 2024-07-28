// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ResourceReferenceBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class ResourceReferenceBinder : BuildObjectBinder<ResourceReference>
  {
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("ResourceType");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder m_authorizedBy = new SqlColumnBinder("AuthorizedBy");
    private SqlColumnBinder m_authorizedOn = new SqlColumnBinder("AuthorizedOn");

    public ResourceReferenceBinder(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override ResourceReference Bind()
    {
      ResourceReference resourceReference = (ResourceReference) null;
      int int32 = this.m_resourceType.GetInt32((IDataReader) this.Reader);
      string str = this.m_resourceId.GetString((IDataReader) this.Reader, false);
      switch (int32)
      {
        case 1:
          Guid result1;
          if (Guid.TryParse(str, out result1))
          {
            resourceReference = (ResourceReference) new ServiceEndpointReference()
            {
              Id = result1
            };
            break;
          }
          break;
        case 2:
          int result2;
          if (int.TryParse(str, out result2))
          {
            resourceReference = (ResourceReference) new AgentPoolQueueReference()
            {
              Id = result2
            };
            break;
          }
          break;
        case 3:
          Guid result3;
          if (Guid.TryParse(str, out result3))
          {
            resourceReference = (ResourceReference) new SecureFileReference()
            {
              Id = result3
            };
            break;
          }
          break;
        case 4:
          int result4;
          if (int.TryParse(str, out result4))
          {
            resourceReference = (ResourceReference) new VariableGroupReference()
            {
              Id = result4
            };
            break;
          }
          break;
      }
      if (resourceReference != null)
      {
        resourceReference.Authorized = true;
        resourceReference.AuthorizedBy = new Guid?(this.m_authorizedBy.GetGuid((IDataReader) this.Reader));
        resourceReference.AuthorizedOn = new DateTime?(this.m_authorizedOn.GetDateTime((IDataReader) this.Reader));
      }
      return resourceReference;
    }
  }
}
