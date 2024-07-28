// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildQueueSpec
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnSerialization("BeforeSerialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildQueueSpec : IValidatable
  {
    private object m_definitionFilter;
    private List<string> m_informationTypes = new List<string>();

    public BuildQueueSpec()
    {
      this.QueryOptions = QueryOptions.All;
      this.Status = QueueStatus.All;
      this.Reason = BuildReason.All;
    }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildControllerSpec ControllerSpec { get; set; }

    [XmlChoiceIdentifier("DefinitionFilterType")]
    [XmlElement("DefinitionUris", typeof (List<string>))]
    [XmlElement("DefinitionSpec", typeof (BuildDefinitionSpec))]
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
          case BuildDefinitionSpec _:
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

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public List<string> InformationTypes => this.m_informationTypes;

    [XmlAttribute]
    [DefaultValue(QueryOptions.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryOptions QueryOptions { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildReason Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor { get; set; }

    [XmlIgnore]
    internal TeamFoundationIdentity RequestedForIdentity { get; set; }

    [XmlAttribute]
    [DefaultValue(QueueStatus.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueueStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int CompletedAge { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Validation.CheckValidatable(requestContext, "ControllerSpec", (IValidatable) this.ControllerSpec, false, ValidationContext.Query);
      if (this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
        Validation.CheckValidatable(requestContext, "DefinitionSpec", (IValidatable) (this.DefinitionFilter as BuildDefinitionSpec), false, ValidationContext.Query);
      else
        ArgumentValidation.CheckUriArray("DefinitionUris", (IList<string>) (this.DefinitionFilter as List<string>), "Definition", false, (string) null);
      ArgumentValidation.CheckBound("CompletedAge", this.CompletedAge, -1);
      this.CompletedAge *= -1;
      this.RequestedForIdentity = Validation.ResolveIdentity(requestContext, this.RequestedFor);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildQueueSpec Status={0} QueryOptions={1} DefinitionFilter={2} ControllerSpec={3}]", (object) this.Status, (object) this.QueryOptions, this.DefinitionFilter is List<string> ? (object) ((List<string>) this.DefinitionFilter).ListItems<string>() : this.DefinitionFilter, (object) this.ControllerSpec);
  }
}
