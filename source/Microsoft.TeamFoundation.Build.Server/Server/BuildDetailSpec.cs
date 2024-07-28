// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDetailSpec
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
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
  public sealed class BuildDetailSpec : IValidatable
  {
    private object m_definitionFilter;
    private string m_definitionPattern;
    private List<string> m_informationTypes = new List<string>();

    public BuildDetailSpec()
    {
      this.MaxBuildsPerDefinition = int.MaxValue;
      this.MaxBuilds = int.MaxValue;
      this.QueryDeletedOption = QueryDeletedOption.ExcludeDeleted;
      this.QueryOptions = QueryOptions.None;
      this.QueryOrder = BuildQueryOrder.StartTimeAscending;
      this.Reason = BuildReason.All;
      this.Status = BuildStatus.All;
    }

    internal BuildDetailSpec(string definitionPath, string buildNumber)
      : this()
    {
      this.DefinitionFilter = (object) new BuildDefinitionSpec(definitionPath);
      this.DefinitionFilterType = DefinitionFilterType.DefinitionSpec;
      this.BuildNumber = buildNumber;
    }

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

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildNumber { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public DateTime MinFinishTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public DateTime MaxFinishTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public DateTime MinChangedTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildStatus Status { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildReason Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string Quality { get; set; }

    [XmlAttribute]
    [DefaultValue(QueryOptions.None)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryOptions QueryOptions { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildQueryOrder.StartTimeAscending)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildQueryOrder QueryOrder { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor { get; set; }

    [XmlIgnore]
    internal TeamFoundationIdentity RequestedForIdentity { get; set; }

    [XmlAttribute]
    [DefaultValue(2147483647)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int MaxBuildsPerDefinition { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public List<string> InformationTypes => this.m_informationTypes;

    [XmlAttribute]
    [DefaultValue(2147483647)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int MaxBuilds { get; set; }

    [XmlAttribute]
    [DefaultValue(QueryDeletedOption.ExcludeDeleted)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryDeletedOption QueryDeletedOption { get; set; }

    internal string TeamProject => this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec ? ((BuildDefinitionSpec) this.DefinitionFilter).TeamProject : (string) null;

    internal string Path => this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec ? ((BuildDefinitionSpec) this.DefinitionFilter).Path : (string) null;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      if (this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        BuildDefinitionSpec definitionFilter = this.DefinitionFilter as BuildDefinitionSpec;
        Validation.CheckValidatable(requestContext, "DefinitionSpec", (IValidatable) definitionFilter, false, ValidationContext.Query);
        this.m_definitionPattern = BuildPath.GetItemName(definitionFilter.FullPath);
      }
      else
        ArgumentValidation.CheckUriArray("DefinitionUris", (IList<string>) (this.DefinitionFilter as List<string>), "Definition", false, (string) null);
      if (this.MinFinishTime < DBHelper.MinAllowedDateTime)
        this.MinFinishTime = DBHelper.MinAllowedDateTime;
      if (this.MaxFinishTime < DBHelper.MinAllowedDateTime || this.MaxFinishTime > DBHelper.MaxAllowedDateTime)
        this.MaxFinishTime = DBHelper.MaxAllowedDateTime;
      if (this.MinChangedTime < DBHelper.MinAllowedDateTime)
        this.MinChangedTime = DBHelper.MinAllowedDateTime;
      if (this.MaxBuildsPerDefinition < 0)
        this.MaxBuildsPerDefinition = int.MaxValue;
      this.RequestedForIdentity = Validation.ResolveIdentity(requestContext, this.RequestedFor);
    }

    internal bool Match(BuildDetail build) => FileSpec.Match(build.BuildNumber, this.BuildNumber);

    internal bool Match(BuildDefinition definition) => this.DefinitionFilterType != DefinitionFilterType.DefinitionSpec || FileSpec.Match(definition.Name, this.m_definitionPattern);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDetailSpec BuildNumber={0} Status={1} DefinitionFilter={2} QueryOptions={3}]", (object) this.BuildNumber, (object) this.Status, this.DefinitionFilter is List<string> ? (object) ((List<string>) this.DefinitionFilter).ListItems<string>() : this.DefinitionFilter, (object) this.QueryOptions);
  }
}
