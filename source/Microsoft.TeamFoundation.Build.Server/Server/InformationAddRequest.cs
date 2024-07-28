// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.InformationAddRequest
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class InformationAddRequest : InformationChangeRequest, IValidatable
  {
    private List<InformationField> m_fields = new List<InformationField>();

    [XmlAttribute]
    public string NodeType { get; set; }

    [XmlAttribute]
    public int NodeId { get; set; }

    [XmlAttribute]
    public int ParentId { get; set; }

    public List<InformationField> Fields => this.m_fields;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.Validate(requestContext, context);
      ArgumentValidation.Check("NodeType", this.NodeType, false, (string) null);
      ArgumentValidation.CheckBound("NodeId", this.NodeId, int.MinValue, -1);
      this.m_fields.ForEach((Action<InformationField>) (x => x.NodeId = this.NodeId));
      Validation.CheckValidatable<InformationField>(requestContext, "Fields", (IList<InformationField>) this.m_fields, false, ValidationContext.Add);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[InformationAddRequest NodeId={0} ParentId={1} NodeType={2} Fields={3}]", (object) this.NodeId, (object) this.ParentId, (object) this.NodeType, (object) this.Fields.ListItems<InformationField>());
  }
}
