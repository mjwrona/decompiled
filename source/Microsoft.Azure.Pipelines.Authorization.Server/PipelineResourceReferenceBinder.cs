// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.PipelineResourceReferenceBinder
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  internal sealed class PipelineResourceReferenceBinder : ObjectBinder<PipelineResourceReference>
  {
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("ResourceType");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder m_authorizedBy = new SqlColumnBinder("AuthorizedBy");
    private SqlColumnBinder m_authorizedOn = new SqlColumnBinder("AuthorizedOn");

    public PipelineResourceReferenceBinder(IVssRequestContext requestContext)
    {
    }

    protected override PipelineResourceReference Bind()
    {
      ResourceType int32 = (ResourceType) this.m_resourceType.GetInt32((IDataReader) this.Reader);
      string str1;
      ResourceTypeNames.s_ValueToNameMap.TryGetValue(int32, out str1);
      string str2 = this.m_resourceId.GetString((IDataReader) this.Reader, false);
      PipelineResourceReference resourceReference = new PipelineResourceReference()
      {
        Id = str2,
        Type = str1
      };
      if (resourceReference != null)
      {
        resourceReference.DefinitionId = this.m_definitionId.GetNullableInt32((IDataReader) this.Reader);
        resourceReference.Authorized = true;
        resourceReference.AuthorizedBy = new Guid?(this.m_authorizedBy.GetGuid((IDataReader) this.Reader));
        resourceReference.AuthorizedOn = new DateTime?(this.m_authorizedOn.GetDateTime((IDataReader) this.Reader));
      }
      return resourceReference;
    }
  }
}
