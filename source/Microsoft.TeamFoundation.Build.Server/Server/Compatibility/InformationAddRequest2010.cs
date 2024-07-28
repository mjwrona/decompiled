// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.InformationAddRequest2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("InformationAddRequest")]
  public sealed class InformationAddRequest2010 : InformationChangeRequest2010, IValidatable
  {
    private List<InformationField2010> m_fields = new List<InformationField2010>();

    [XmlAttribute]
    public string NodeType { get; set; }

    [XmlAttribute]
    public int NodeId { get; set; }

    [XmlAttribute]
    public int ParentId { get; set; }

    public List<InformationField2010> Fields => this.m_fields;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.Validate(requestContext, context);
      ArgumentValidation.Check("NodeType", this.NodeType, false, (string) null);
      ArgumentValidation.CheckBound("NodeId", this.NodeId, int.MinValue, -1);
      if (this.m_fields == null)
        return;
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<InformationField2010>(requestContext, "Fields", (IList<InformationField2010>) this.m_fields, false, ValidationContext.Add);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[InformationAddRequest2010 NodeId={0} ParentId={1} NodeType={2} Fields={3}]", (object) this.NodeId, (object) this.ParentId, (object) this.NodeType, (object) this.Fields.ListItems<InformationField2010>());
  }
}
