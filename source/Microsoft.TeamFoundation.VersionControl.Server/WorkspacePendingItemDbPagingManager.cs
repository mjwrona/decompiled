// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspacePendingItemDbPagingManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class WorkspacePendingItemDbPagingManager : 
    VersionControlDBPagingManager<ItemPathPair>,
    ICheckinItemManager,
    IEnumerable<ItemPathPair>,
    IEnumerable
  {
    public WorkspacePendingItemDbPagingManager(
      VersionControlRequestContext versionControlRequestContext,
      string token,
      int lobParameterId)
      : base(versionControlRequestContext, true)
    {
      this.m_serverItemWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_xmlTextWriter = new XmlTextWriter((TextWriter) this.m_serverItemWriter);
      if (lobParameterId == 0)
      {
        this.m_xmlTextWriter.WriteStartDocument();
        this.m_xmlTextWriter.WriteStartElement(this.RootElement);
      }
      this.m_db = versionControlRequestContext.RequestContext.CreateComponent<LobParameterComponent>("VersionControl");
      this.m_db.Token = token;
      this.m_db.ParameterId = lobParameterId;
      this.m_parameterId = lobParameterId;
      this.m_isPagedOverride = true;
    }

    protected override string RootElement => "items";

    protected override ItemPathPair ReadSingleElement(XmlReader reader) => new ItemPathPair(reader.GetAttribute("s"), reader.GetAttribute("d"));

    protected override void WriteSingleElement(ItemPathPair item)
    {
      this.m_xmlTextWriter.WriteStartElement("i");
      this.m_xmlTextWriter.WriteAttributeString("s", item.ProjectNamePath);
      this.m_xmlTextWriter.WriteAttributeString("d", item.ProjectGuidPath);
      this.m_xmlTextWriter.WriteEndElement();
    }

    void ICheckinItemManager.Flush(bool isLastPage) => this.Flush(isLastPage);

    int ICheckinItemManager.get_ParameterId() => this.ParameterId;

    int ICheckinItemManager.get_TotalCount() => this.TotalCount;

    List<ItemPathPair> ICheckinItemManager.get_FirstPage() => this.FirstPage;
  }
}
