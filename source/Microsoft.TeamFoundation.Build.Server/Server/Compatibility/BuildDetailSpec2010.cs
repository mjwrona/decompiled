// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDetailSpec2010
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

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildDetailSpec")]
  public class BuildDetailSpec2010 : IValidatable
  {
    private object m_definitionFilter;
    private string m_definitionPattern;
    private List<string> m_informationTypes = new List<string>();

    public BuildDetailSpec2010()
    {
      this.MaxBuildsPerDefinition = int.MaxValue;
      this.QueryDeletedOption = QueryDeletedOption2010.ExcludeDeleted;
      this.QueryOptions = QueryOptions2010.None;
      this.QueryOrder = BuildQueryOrder2010.StartTimeAscending;
      this.Reason = BuildReason2010.All;
      this.Status = BuildStatus2010.All;
    }

    internal BuildDetailSpec2010(string definitionPath, string buildNumber)
      : this()
    {
      this.DefinitionFilter = (object) new BuildDefinitionSpec2010(definitionPath);
      this.DefinitionFilterType = DefinitionFilterType.DefinitionSpec;
      this.BuildNumber = buildNumber;
    }

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
    public BuildStatus2010 Status { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason2010.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildReason2010 Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string Quality { get; set; }

    [XmlAttribute]
    [DefaultValue(QueryOptions2010.None)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryOptions2010 QueryOptions { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildQueryOrder2010.StartTimeAscending)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildQueryOrder2010 QueryOrder { get; set; }

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
    [DefaultValue(QueryDeletedOption2010.ExcludeDeleted)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryDeletedOption2010 QueryDeletedOption { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string DefinitionPath
    {
      get => (string) null;
      set
      {
      }
    }

    internal string TeamProject => this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec ? ((BuildGroupItemSpec2010) this.DefinitionFilter).TeamProject : (string) null;

    internal string GroupPath => this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec ? ((BuildGroupItemSpec2010) this.DefinitionFilter).GroupPath : (string) null;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      if (this.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        BuildDefinitionSpec2010 definitionFilter = this.DefinitionFilter as BuildDefinitionSpec2010;
        Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "DefinitionSpec", (IValidatable) definitionFilter, false, ValidationContext.Query);
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
      this.RequestedForIdentity = Microsoft.TeamFoundation.Build.Server.Validation.ResolveIdentity(requestContext, this.RequestedFor);
    }

    internal bool Match(BuildDetail2010 build) => FileSpec.Match(build.BuildNumber, this.BuildNumber);

    internal bool Match(BuildDefinition2010 definition) => this.DefinitionFilterType != DefinitionFilterType.DefinitionSpec || FileSpec.Match(definition.Name, this.m_definitionPattern);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDetailSpec2010 BuildNumber={0} Status={1} DefinitionFilter={2} QueryOptions={3}]", (object) this.BuildNumber, (object) this.Status, this.DefinitionFilter is List<string> ? (object) ((List<string>) this.DefinitionFilter).ListItems<string>() : this.DefinitionFilter, (object) this.QueryOptions);
  }
}
