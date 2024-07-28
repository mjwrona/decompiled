// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.InformationEditRequest2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("InformationEditRequest")]
  public sealed class InformationEditRequest2010 : InformationChangeRequest2010
  {
    private List<InformationField2010> m_fields = new List<InformationField2010>();

    public InformationEditRequest2010() => this.Options = InformationEditOptions2010.MergeFields;

    [XmlAttribute]
    public int NodeId { get; set; }

    [XmlAttribute]
    [DefaultValue(InformationEditOptions2010.MergeFields)]
    public InformationEditOptions2010 Options { get; set; }

    public List<InformationField2010> Fields => this.m_fields;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[InformationEditRequest2010 NodeId={0} Options={1} Fields={2}]", (object) this.NodeId, (object) this.Options, (object) this.Fields.ListItems<InformationField2010>());
  }
}
