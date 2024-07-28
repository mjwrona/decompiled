// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.Pipeline
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class Pipeline : PipelineReference
  {
    private PipelineConfiguration m_configuration;
    private byte[] m_serializedConfiguration;

    public Pipeline(Guid projectId, int pipelineId, int revision, string name, string folder)
      : this(projectId, pipelineId, revision, name, folder, (byte[]) null)
    {
    }

    internal Pipeline(
      Guid projectId,
      int pipelineId,
      int revision,
      string name,
      string folder,
      byte[] serializedConfiguration)
      : base(projectId, pipelineId, revision, name, folder)
    {
      this.m_serializedConfiguration = serializedConfiguration;
    }

    public PipelineConfiguration GetConfiguration(IVssRequestContext requestContext)
    {
      if (this.m_configuration == null && this.m_serializedConfiguration != null && this.m_serializedConfiguration.Length != 0)
      {
        using (requestContext.TraceScope(nameof (Pipeline), nameof (GetConfiguration)))
        {
          try
          {
            this.m_configuration = JsonUtility.Deserialize<PipelineConfiguration>(this.m_serializedConfiguration);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (Pipeline), ex);
            this.m_serializedConfiguration = (byte[]) null;
          }
        }
      }
      return this.m_configuration;
    }

    public void SetConfiguration(PipelineConfiguration configuration)
    {
      this.m_configuration = configuration;
      this.m_serializedConfiguration = (byte[]) null;
    }

    public virtual ISecuredObject ToSecuredObject() => (ISecuredObject) new SecuredObject(Guid.Empty, 1, "pipeline dummy token");
  }
}
