// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasePreferences
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class BasePreferences : IXmlSerializable
  {
    protected virtual void CopyTo(BasePreferences bp)
    {
      bp.Language = this.Language;
      bp.Culture = this.Culture;
      bp.TimeZone = this.TimeZone;
    }

    public CultureInfo Language { set; get; }

    public virtual CultureInfo Culture { set; get; }

    public TimeZoneInfo TimeZone { set; get; }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.WriteXml(XmlWriter writer) => PreferencesHelper.WriteXml(this, writer);

    void IXmlSerializable.ReadXml(XmlReader reader) => PreferencesHelper.ReadXml(this, reader);
  }
}
