// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.IProcessTemplates
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  public interface IProcessTemplates
  {
    [Obsolete("Please use the TemplateHeaders method instead")]
    XmlNode GetTemplateNames();

    TemplateHeader[] TemplateHeaders();

    TemplateHeader[] DeleteTemplate(int templateId);

    TemplateHeader[] MakeDefaultTemplate(int templateId);

    [Obsolete("Please use AddUpdateTemplate instead")]
    int AddTemplate(string name, string description, string metadata, string state);

    int GetTemplateIndex(string name);

    void AddUpdateTemplate(
      string name,
      string description,
      string metadata,
      string state,
      string zipFileName);

    [Obsolete("Please use AddUpdateTemplate instead")]
    void UploadMethodology(string fileName, int templateId);

    string GetTemplateData(int templateId);
  }
}
