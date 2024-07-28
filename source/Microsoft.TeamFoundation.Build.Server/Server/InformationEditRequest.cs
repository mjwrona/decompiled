// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.InformationEditRequest
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class InformationEditRequest : InformationChangeRequest, IValidatable
  {
    private List<InformationField> m_fields = new List<InformationField>();

    public InformationEditRequest() => this.Options = InformationEditOptions.MergeFields;

    [XmlAttribute]
    public int NodeId { get; set; }

    [XmlAttribute]
    [DefaultValue(InformationEditOptions.MergeFields)]
    public InformationEditOptions Options { get; set; }

    public List<InformationField> Fields => this.m_fields;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.Validate(requestContext, context);
      this.m_fields.ForEach((Action<InformationField>) (x => x.NodeId = this.NodeId));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[InformationEditRequest NodeId={0} Options={1} Fields={2}]", (object) this.NodeId, (object) this.Options, (object) this.Fields.ListItems<InformationField>());
  }
}
