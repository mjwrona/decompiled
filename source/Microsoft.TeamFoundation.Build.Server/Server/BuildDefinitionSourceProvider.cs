// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProvider
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildDefinitionSourceProvider : IValidatable
  {
    private string m_name;

    internal static List<BuildDefinitionSourceProvider> GetDefaultProviders()
    {
      List<BuildDefinitionSourceProvider> defaultProviders = new List<BuildDefinitionSourceProvider>();
      defaultProviders.AddRange((IEnumerable<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProvider[3]
      {
        new BuildDefinitionSourceProvider()
        {
          Name = BuildSourceProviders.Git
        },
        new BuildDefinitionSourceProvider()
        {
          Name = BuildSourceProviders.TfGit
        },
        new BuildDefinitionSourceProvider()
        {
          Name = BuildSourceProviders.TfVersionControl
        }
      });
      return defaultProviders;
    }

    public BuildDefinitionSourceProvider()
    {
      this.Fields = new List<NameValueField>();
      this.SupportedTriggerTypes = DefinitionTriggerType.All;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name
    {
      get => this.m_name;
      set
      {
        this.m_name = value;
        if (BuildSourceProviders.IsTfGit(this.Name))
          this.SupportedTriggerTypes = DefinitionTriggerType.ContinuousIntegration | DefinitionTriggerType.BatchedContinuousIntegration | DefinitionTriggerType.Schedule | DefinitionTriggerType.ScheduleForced;
        else if (BuildSourceProviders.IsGit(this.Name))
          this.SupportedTriggerTypes = DefinitionTriggerType.ScheduleForced;
        else
          this.SupportedTriggerTypes = DefinitionTriggerType.All;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public DefinitionTriggerType SupportedTriggerTypes { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private, PropertyName = "InternalFields")]
    public List<NameValueField> Fields { get; set; }

    internal int Id { get; set; }

    internal string DefinitionUri { get; set; }

    internal Guid ProjectId { get; set; }

    internal DateTime LastModified { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context) => ArgumentValidation.Check("Name", (object) this.Name, false);

    internal bool TryGetRepoInfo(out string teamProject, out string name)
    {
      teamProject = (string) null;
      name = (string) null;
      if (!BuildSourceProviders.IsTfGit(this.Name))
        return false;
      NameValueField nameValueField = this.Fields.Where<NameValueField>((Func<NameValueField, bool>) (x => string.Equals(x.Name, BuildSourceProviders.GitProperties.RepositoryName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<NameValueField>();
      return nameValueField != null && BuildSourceProviders.GitProperties.ParseUniqueRepoName(nameValueField.Value, out teamProject, out name);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[SourceProvider DefinitionUri={0} Name={1}]", (object) this.DefinitionUri, (object) this.Name);
  }
}
