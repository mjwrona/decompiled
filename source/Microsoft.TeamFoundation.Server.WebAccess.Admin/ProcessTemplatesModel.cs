// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessTemplatesModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class ProcessTemplatesModel
  {
    private List<ProcessTemplateDescriptorModel> m_processTemplates = new List<ProcessTemplateDescriptorModel>();

    public List<ProcessTemplateDescriptorModel> Templates => this.m_processTemplates;

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["Templates"] = (object) this.Templates;
      return json;
    }
  }
}
