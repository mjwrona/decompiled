// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildQueueSpec2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildQueueSpec")]
  public sealed class BuildQueueSpec2010 : IValidatable
  {
    private object m_definitionFilter;

    public BuildQueueSpec2010()
    {
      this.Options = QueryOptions2010.All;
      this.StatusFlags = QueueStatus2010.All;
    }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildControllerSpec2010 ControllerSpec { get; set; }

    [XmlChoiceIdentifier("DefinitionFilterType")]
    [XmlElement("DefinitionUris", typeof (List<string>))]
    [XmlElement("DefinitionSpec", typeof (BuildDefinitionSpec2010))]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public object DefinitionFilter
    {
      get => this.m_definitionFilter;
      set
      {
        switch (value)
        {
          case null:
            break;
          case BuildDefinitionSpec2010 _:
            this.m_definitionFilter = value;
            this.DefinitionFilterType = DefinitionFilterType.DefinitionSpec;
            break;
          case IEnumerable<string> _:
            this.m_definitionFilter = !(value is List<string>) ? (object) new List<string>((IEnumerable<string>) value) : value;
            this.DefinitionFilterType = DefinitionFilterType.DefinitionUris;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    [XmlIgnore]
    public DefinitionFilterType DefinitionFilterType { get; set; }

    [XmlAttribute]
    [DefaultValue(QueryOptions2010.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, PropertyName = "QueryOptions", Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryOptions2010 Options { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [DefaultValue(QueueStatus2010.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, PropertyName = "Status", Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueueStatus2010 StatusFlags { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int CompletedAge { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "ControllerSpec", (IValidatable) this.ControllerSpec, false, ValidationContext.Query);
      if (this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
        Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "DefinitionSpec", (IValidatable) (this.DefinitionFilter as BuildDefinitionSpec2010), false, ValidationContext.Query);
      else
        ArgumentValidation.CheckUriArray("DefinitionUris", (IList<string>) (this.DefinitionFilter as List<string>), "Definition", false, (string) null);
      ArgumentValidation.CheckBound("CompletedAge", this.CompletedAge, -1);
      this.CompletedAge *= -1;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildQueueSpec2010 StatusFlags={0} Options={1} DefinitionFilter={2} ControllerSpec={3}]", (object) this.StatusFlags, (object) this.Options, this.DefinitionFilter is List<string> ? (object) ((List<string>) this.DefinitionFilter).ListItems<string>() : this.DefinitionFilter, (object) this.ControllerSpec);
  }
}
