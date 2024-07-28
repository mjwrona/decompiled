// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplateBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionTemplateBinder : BuildObjectBinder<BuildDefinitionTemplate>
  {
    private SqlColumnBinder m_templateId = new SqlColumnBinder("TemplateId");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_template = new SqlColumnBinder("Template");

    public BuildDefinitionTemplateBinder(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override BuildDefinitionTemplate Bind()
    {
      BuildDefinitionTemplate definitionTemplate = new BuildDefinitionTemplate();
      definitionTemplate.Id = this.m_templateId.GetString((IDataReader) this.Reader, false);
      definitionTemplate.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      definitionTemplate.Description = this.m_description.GetString((IDataReader) this.Reader, true);
      definitionTemplate.CanDelete = true;
      BuildDefinition buildDefinition = (BuildDefinition) null;
      try
      {
        buildDefinition = ServerBuildDefinitionHelpers.Deserialize(this.m_template.GetString((IDataReader) this.Reader, false));
      }
      catch (Exception ex)
      {
        this.TraceException("DefinitionTemplateService", ex);
      }
      definitionTemplate.Template = buildDefinition;
      return definitionTemplate;
    }
  }
}
