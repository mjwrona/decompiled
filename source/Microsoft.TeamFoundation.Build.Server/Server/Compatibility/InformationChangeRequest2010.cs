// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.InformationChangeRequest2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlInclude(typeof (InformationAddRequest2010))]
  [XmlInclude(typeof (InformationEditRequest2010))]
  [XmlInclude(typeof (InformationDeleteRequest2010))]
  [XmlType("InformationChangeRequest")]
  public abstract class InformationChangeRequest2010 : IValidatable
  {
    [XmlAttribute]
    [ClientType(typeof (Uri))]
    public string BuildUri { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context) => this.Validate(requestContext, context);

    internal void Validate(IVssRequestContext requestContext, ValidationContext context) => ArgumentValidation.CheckUri("BuildUri", this.BuildUri, "Build", false, (string) null);
  }
}
