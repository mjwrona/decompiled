// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal abstract class BuildSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    private ServiceVersion m_serviceVersion = ServiceVersion.V1;
    private IProjectService m_projectService;
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly string s_area = "Build";
    private static readonly string s_layer = nameof (BuildSqlResourceComponent);

    static BuildSqlResourceComponent()
    {
      BuildSqlResourceComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900010, new SqlExceptionFactory(typeof (BuildDefinitionAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900011, new SqlExceptionFactory(typeof (BuildDefinitionUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900043, new SqlExceptionFactory(typeof (BuildDefinitionUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900542, new SqlExceptionFactory(typeof (BuildDefinitionUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900017, new SqlExceptionFactory(typeof (BuildDefinitionDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900020, new SqlExceptionFactory(typeof (InvalidBuildGroupItemUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900012, new SqlExceptionFactory(typeof (BuildGroupAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900013, new SqlExceptionFactory(typeof (BuildGroupDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900009, new SqlExceptionFactory(typeof (BuildNumberAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900018, new SqlExceptionFactory(typeof (BuildQualityDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900028, new SqlExceptionFactory(typeof (CannotDeleteDefinitionBuildExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900025, new SqlExceptionFactory(typeof (DuplicateInformationChangeRequestException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900021, new SqlExceptionFactory(typeof (InvalidBuildRequestException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900005, new SqlExceptionFactory(typeof (InvalidBuildUriException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900026, new SqlExceptionFactory(typeof (InformationNodeDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900024, new SqlExceptionFactory(typeof (InformationParentNodeDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900008, new SqlExceptionFactory(typeof (InvalidPlatformFlavorException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900522, new SqlExceptionFactory(typeof (BuildAgentAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900521, new SqlExceptionFactory(typeof (BuildAgentAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900520, new SqlExceptionFactory(typeof (BuildAgentDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900523, new SqlExceptionFactory(typeof (BuildAgentUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900525, new SqlExceptionFactory(typeof (BuildAgentUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900526, new SqlExceptionFactory(typeof (BuildAgentUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900512, new SqlExceptionFactory(typeof (BuildControllerAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900511, new SqlExceptionFactory(typeof (BuildControllerAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900513, new SqlExceptionFactory(typeof (BuildControllerDeletionException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900514, new SqlExceptionFactory(typeof (BuildControllerDeletionException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900510, new SqlExceptionFactory(typeof (BuildControllerDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900501, new SqlExceptionFactory(typeof (BuildServiceHostAlreadyExistsException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900500, new SqlExceptionFactory(typeof (BuildServiceHostDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900502, new SqlExceptionFactory(typeof (BuildServiceHostOwnershipException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900534, new SqlExceptionFactory(typeof (QueuedBuildDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900531, new SqlExceptionFactory(typeof (QueuedBuildDoesNotExistException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900532, new SqlExceptionFactory(typeof (QueuedBuildUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900533, new SqlExceptionFactory(typeof (QueuedBuildUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900538, new SqlExceptionFactory(typeof (QueuedBuildUpdateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900530, new SqlExceptionFactory(typeof (InvalidQueueRequestException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900540, new SqlExceptionFactory(typeof (InvalidBuildAgentReservationException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900541, new SqlExceptionFactory(typeof (InvalidBuildAgentReservationException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900544, new SqlExceptionFactory(typeof (InvalidBuildAgentReservationException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900033, new SqlExceptionFactory(typeof (InvalidSharedResourceRequestException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900035, new SqlExceptionFactory(typeof (SharedResourceAlreadyAcquiredException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900034, new SqlExceptionFactory(typeof (SharedResourceAlreadyRequestedException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900036, new SqlExceptionFactory(typeof (BuildNotDeletedException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900037, new SqlExceptionFactory(typeof (DuplicateProcessTemplateException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900038, new SqlExceptionFactory(typeof (MultipleDefaultProcessTemplatesException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900039, new SqlExceptionFactory(typeof (MultipleUpgradeProcessTemplatesException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900040, new SqlExceptionFactory(typeof (ProcessTemplateNotFoundException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900041, new SqlExceptionFactory(typeof (ProcessTemplateNotFoundException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900042, new SqlExceptionFactory(typeof (BuildDefinitionDisabledException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900535, new SqlExceptionFactory(typeof (CannotStartBuildException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900536, new SqlExceptionFactory(typeof (CannotStartBuildException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900537, new SqlExceptionFactory(typeof (CannotStartBuildException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900539, new SqlExceptionFactory(typeof (CannotStartBuildException)));
      BuildSqlResourceComponent.s_sqlExceptionFactories.Add(900545, new SqlExceptionFactory(typeof (BuildDeploymentAlreadyExistsException)));
    }

    protected BuildSqlResourceComponent() => this.ContainerErrorCode = 50000;

    public ServiceVersion ServiceVersion
    {
      get => this.m_serviceVersion;
      protected set => this.m_serviceVersion = value;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) BuildSqlResourceComponent.s_sqlExceptionFactories;

    public IProjectService ProjectService
    {
      get
      {
        if (this.m_projectService == null)
        {
          using (this.AcquireExemptionLock())
            this.m_projectService = this.RequestContext.GetService<IProjectService>();
        }
        return this.m_projectService;
      }
    }

    public SqlParameter BindUri(string parameterName, string value, bool allowNull)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding uri Name='{0}' Uri='{1}' AllowNull='{2}'", (object) parameterName, (object) value, (object) allowNull);
      return this.BindString(parameterName, value, BuildConstants.MaxUriLength, allowNull, SqlDbType.VarChar);
    }

    public SqlParameter BindIdentity(string parameterName, TeamFoundationIdentity identity)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding identity Name='{0}' Identity='{1}'", (object) parameterName, identity == null ? (object) (string) null : (object) identity.UniqueName);
      return identity == null ? this.BindNullValue(parameterName, SqlDbType.NVarChar) : this.BindString(parameterName, identity.TeamFoundationId.ToString("B"), 38, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
    }

    protected SqlParameter BindIdentityFilter(
      string parameterName,
      string filterValue,
      TeamFoundationIdentity identity)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding identity filter Name='{0}' Identity='{1}' Filter='{2}'", (object) parameterName, identity == null ? (object) (string) null : (object) identity.UniqueName, (object) filterValue);
      if (string.IsNullOrEmpty(filterValue))
        return this.BindNullValue(parameterName, SqlDbType.NVarChar);
      return identity == null ? this.BindString(parameterName, filterValue, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar) : this.BindIdentity(parameterName, identity);
    }

    public SqlParameter BindItemUriToInt32(string parameterName, string value)
    {
      int parameterValue = int.Parse(DBHelper.ExtractDbId(value), (IFormatProvider) CultureInfo.InvariantCulture);
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding uri to integer Name='{0}' Uri='{1}' Int32='{2}'", (object) parameterName, (object) value, (object) parameterValue);
      return this.BindInt(parameterName, parameterValue);
    }

    public SqlParameter BindItemUriToInt64(string parameterName, string value)
    {
      long parameterValue = long.Parse(DBHelper.ExtractDbId(value), (IFormatProvider) CultureInfo.InvariantCulture);
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding uri to long Name='{0}' Uri='{1}' Int64='{2}'", (object) parameterName, (object) value, (object) parameterValue);
      return this.BindLong(parameterName, parameterValue);
    }

    public SqlParameter BindItemName(
      string parameterName,
      string value,
      int maxLength,
      bool allowNull)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding item name Name='{0}' Value='{1}' AllowNull='{2}'", (object) parameterName, (object) value, (object) allowNull);
      return this.BindString(parameterName, DBHelper.ServerPathToDBPath(value), maxLength, allowNull, SqlDbType.NVarChar);
    }

    public SqlParameter BindItemPath(string parameterName, string value, bool allowNull)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding item path Name='{0}' Value='{1}' AllowNull='{2}'", (object) parameterName, (object) value, (object) allowNull);
      return this.BindString(parameterName, DBHelper.ServerPathToDBPath(value), BuildConstants.MaxPathLength, allowNull, SqlDbType.NVarChar);
    }

    public SqlParameter BindUncPath(string parameterName, string value, bool allowNull)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding UNC path Name='{0}' Value='{1}' AllowNull='{2}'", (object) parameterName, (object) value, (object) allowNull);
      return this.BindString(parameterName, value, BuildConstants.MaxPathLength, allowNull, SqlDbType.NVarChar);
    }

    public SqlParameter BindTeamProject(
      IVssRequestContext requestContext,
      string parameterName,
      string value,
      bool allowNull)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding team project Name='{0}' Value='{1}' AllowNull='{2}'", (object) parameterName, (object) value, (object) allowNull);
      if (string.IsNullOrEmpty(value) & allowNull || BuildCommonUtil.IsStar(value))
        return this.BindNullValue(parameterName, SqlDbType.VarChar);
      TeamProject projectFromGuidOrName = this.GetTeamProjectFromGuidOrName(requestContext, value);
      return this.BindString(parameterName, projectFromGuidOrName.Uri, BuildConstants.MaxUriLength, false, SqlDbType.VarChar);
    }

    public SqlParameter BindTeamProjectDataspace(
      IVssRequestContext requestContext,
      string parameterName,
      string value,
      bool allowNull)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding team project Name='{0}' Value='{1}' AllowNull='{2}'", (object) parameterName, (object) value, (object) allowNull);
      if (string.IsNullOrEmpty(value) & allowNull || BuildCommonUtil.IsStar(value))
        return this.BindNullValue(parameterName, SqlDbType.Int);
      TeamProject projectFromGuidOrName = this.GetTeamProjectFromGuidOrName(requestContext, value);
      return this.BindInt(parameterName, this.GetDataspaceId(projectFromGuidOrName.Id));
    }

    public SqlParameter BindTeamProjectTableDataspace(
      IVssRequestContext requestContext,
      string parameterName,
      ICollection<string> values)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding team projects table Name='{0}' Values='{1}'", (object) parameterName, (object) values);
      List<int> intList = new List<int>();
      foreach (string uri in (IEnumerable<string>) values)
      {
        try
        {
          int dataspaceId = this.GetDataspaceId(this.GetTeamProjectFromUri(requestContext, uri).Id);
          intList.Add(dataspaceId);
        }
        catch (DataspaceNotFoundException ex)
        {
          this.RequestContext.Trace(0, TraceLevel.Warning, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Could not find dataspace for project \"{0}\".", (object) uri);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          this.RequestContext.Trace(0, TraceLevel.Warning, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Could not find project \"{0}\".", (object) uri);
        }
      }
      return this.BindTable<int>(parameterName, (TeamFoundationTableValueParameter<int>) new Int32Table((IEnumerable<int>) intList));
    }

    protected SqlParameter BindNullableDateTime(string parameterName, DateTime value)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding nullable date time Name='{0}' Value='{1}'", (object) parameterName, (object) value);
      return value < DBHelper.MinAllowedDateTime ? this.BindNullValue(parameterName, SqlDbType.DateTime) : this.BindDateTime(parameterName, value);
    }

    protected SqlParameter BindUtcDateTime(string parameterName, DateTime value)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding UTC date time Name='{0}' Value='{1}'", (object) parameterName, (object) value);
      return this.BindNullableDateTime(parameterName, value.ToUniversalTime());
    }

    public SqlParameter BindArtifactPropertyValueList(
      string name,
      IEnumerable<ArtifactPropertyValue> properties)
    {
      using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w))
        {
          xmlTextWriter.WriteStartDocument();
          xmlTextWriter.WriteStartElement("PropertyValueList");
          if (properties != null)
          {
            foreach (ArtifactPropertyValue property in properties)
            {
              foreach (PropertyValue propertyValue in property.PropertyValues)
              {
                this.RequestContext.Trace(0, TraceLevel.Verbose, BuildSqlResourceComponent.s_area, BuildSqlResourceComponent.s_layer, "Binding property value Name='{0}' ArtifactId='{1}' PropertyName='{2}' PropertyValue='{3}'", (object) name, (object) property.Spec.Id, (object) propertyValue.PropertyName, propertyValue.Value);
                xmlTextWriter.WriteStartElement("pv");
                xmlTextWriter.WriteStartAttribute("aid");
                if (property.Spec.Id != null)
                  xmlTextWriter.WriteValue((object) property.Spec.Id);
                xmlTextWriter.WriteEndAttribute();
                xmlTextWriter.WriteAttributeString("v", "0");
                xmlTextWriter.WriteAttributeString("pn", propertyValue.PropertyName);
                xmlTextWriter.WriteStartAttribute("ds");
                xmlTextWriter.WriteValue(this.GetDataspaceId(property.Spec.DataspaceIdentifier));
                xmlTextWriter.WriteEndAttribute();
                XmlPropertyWriter.WriteValue((XmlWriter) xmlTextWriter, propertyValue.PropertyName, propertyValue.Value);
                xmlTextWriter.WriteEndElement();
              }
            }
          }
          xmlTextWriter.WriteEndElement();
          xmlTextWriter.WriteEndDocument();
          return this.BindXml(name, w.ToString());
        }
      }
    }

    public string VersionControlPathToDataspaceDBPath(string path)
    {
      path = TFVCPathHelper.ConvertToPathWithProjectGuid((System.Func<string, Guid>) (x => this.GetTeamProjectFromGuidOrName(this.RequestContext, x).Id), path);
      return DBHelper.VersionControlPathToDBPath(path);
    }

    public string DataspaceDBPathToVersionControlPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      if (string.Equals("$\\", path, StringComparison.Ordinal))
        return "$/";
      path = DBHelper.DBPathToVersionControlPath(path);
      path = TFVCPathHelper.ConvertToPathWithProjectName((System.Func<Guid, string>) (x => this.GetTeamProjectFromGuid(this.RequestContext, x).Name), path);
      return path;
    }

    internal TeamProject GetTeamProjectFromGuid(IVssRequestContext requestContext, Guid projectId)
    {
      using (this.AcquireExemptionLock())
        return this.ProjectService.GetTeamProjectFromGuid(requestContext, projectId);
    }

    internal TeamProject GetTeamProjectFromGuidOrName(
      IVssRequestContext requestContext,
      string value)
    {
      using (this.AcquireExemptionLock())
        return this.ProjectService.GetTeamProjectFromGuidOrName(requestContext, value);
    }

    internal TeamProject GetTeamProjectFromUri(IVssRequestContext requestContext, string uri)
    {
      using (this.AcquireExemptionLock())
        return this.ProjectService.GetTeamProjectFromUri(requestContext, uri);
    }

    internal string GetUniqueName(
      IVssRequestContext requestContext,
      BuildIdentityResolver resolver,
      string identityValue,
      out string displayName,
      out TeamFoundationIdentity identity)
    {
      using (this.AcquireExemptionLock())
      {
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        return resolver.GetUniqueName(requestContext, service, identityValue, out displayName, out identity);
      }
    }

    public static KeyValuePairInt32Int32Table IntsToOrderedTable(IEnumerable<int> values)
    {
      int index = 0;
      return new KeyValuePairInt32Int32Table((IEnumerable<KeyValuePair<int, int>>) values.Select<int, KeyValuePair<int, int>>((System.Func<int, KeyValuePair<int, int>>) (x => new KeyValuePair<int, int>(++index, x))).ToArray<KeyValuePair<int, int>>());
    }

    public static Int32Table UrisToInt32Table(IEnumerable<string> uris) => new Int32Table((IEnumerable<int>) uris.Select<string, int>((System.Func<string, int>) (x => int.Parse(DBHelper.ExtractDbId(x), (IFormatProvider) CultureInfo.InvariantCulture))).ToArray<int>());

    public static Int32Table UrisToDistinctInt32Table(IEnumerable<string> uris) => new Int32Table((IEnumerable<int>) uris.Select<string, int>((System.Func<string, int>) (x => int.Parse(DBHelper.ExtractDbId(x), (IFormatProvider) CultureInfo.InvariantCulture))).Distinct<int>().ToArray<int>());

    internal static KeyValuePairInt32StringTable UrisToOrderedStringTable(IEnumerable<string> uris)
    {
      int itemIndex = 0;
      return new KeyValuePairInt32StringTable((IEnumerable<KeyValuePair<int, string>>) uris.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (x => new KeyValuePair<int, string>(++itemIndex, DBHelper.ExtractDbId(x)))).ToArray<KeyValuePair<int, string>>());
    }

    public static StringTable UrisToStringTable(IEnumerable<string> uris) => new StringTable((IEnumerable<string>) uris.Select<string, string>((System.Func<string, string>) (x => DBHelper.ExtractDbId(x))).ToArray<string>());

    public SqlParameter BindUrisToInt32Table(string parameterName, IEnumerable<string> uris) => this.BindInt32Table(parameterName, (IEnumerable<int>) uris.Select<string, int>((System.Func<string, int>) (x => int.Parse(DBHelper.ExtractDbId(x), (IFormatProvider) CultureInfo.InvariantCulture))).ToArray<int>());

    public SqlParameter BindUrisToDistinctInt32Table(string parameterName, IEnumerable<string> uris) => this.BindInt32Table(parameterName, (IEnumerable<int>) uris.Select<string, int>((System.Func<string, int>) (x => int.Parse(DBHelper.ExtractDbId(x), (IFormatProvider) CultureInfo.InvariantCulture))).Distinct<int>().ToArray<int>());

    public SqlParameter BindUrisToOrderedStringTable(string parameterName, IEnumerable<string> uris)
    {
      int itemIndex = 0;
      KeyValuePair<int, string>[] array = uris.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (x => new KeyValuePair<int, string>(++itemIndex, DBHelper.ExtractDbId(x)))).ToArray<KeyValuePair<int, string>>();
      return this.BindKeyValuePairInt32StringTable(parameterName, (IEnumerable<KeyValuePair<int, string>>) array);
    }

    public SqlParameter BindIntsToOrderedIntsTable(string parameterName, IEnumerable<int> ints)
    {
      int index = 0;
      KeyValuePair<int, int>[] array = ints.Select<int, KeyValuePair<int, int>>((System.Func<int, KeyValuePair<int, int>>) (x => new KeyValuePair<int, int>(++index, x))).ToArray<KeyValuePair<int, int>>();
      return this.BindKeyValuePairInt32Int32Table(parameterName, (IEnumerable<KeyValuePair<int, int>>) array);
    }
  }
}
