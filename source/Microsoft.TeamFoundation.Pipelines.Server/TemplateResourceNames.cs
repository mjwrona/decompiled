// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TemplateResourceNames
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class TemplateResourceNames
  {
    private readonly string m_resourceNamePrefix;
    private readonly string m_metaDataResourceName;

    public TemplateResourceNames(string resourceNamePrefix, string metaDataResourceName)
    {
      this.m_resourceNamePrefix = resourceNamePrefix;
      this.m_metaDataResourceName = metaDataResourceName;
    }

    public IEnumerable<string> GetTemplateIds() => ((IEnumerable<string>) typeof (TemplatesService).Assembly.GetManifestResourceNames()).Where<string>(new Func<string, bool>((object) this, __methodptr(\u003CGetTemplateIds\u003Eg__IsTemplateResourceName\u007C3_0))).Select<string, string>(new Func<string, string>((object) this, __methodptr(\u003CGetTemplateIds\u003Eg__GetTemplateId\u007C3_1)));

    public string GetTemplateResourceName(string templateId) => this.m_resourceNamePrefix + templateId + ".yml";

    public string GetMetaDataResourceName(string templateResourceName) => templateResourceName.RemovePrefix(this.m_resourceNamePrefix).Replace(".yml", "_" + this.m_metaDataResourceName + ".resources");
  }
}
