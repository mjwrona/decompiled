// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationPropertyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationPropertyService : 
    ITeamFoundationPropertyService,
    IVssFrameworkService
  {
    private ConcurrentDictionary<KeyValuePair<Guid, string>, PropertyDefinition> m_definitionCache = new ConcurrentDictionary<KeyValuePair<Guid, string>, PropertyDefinition>(TeamFoundationPropertyService.PropertyDefinitionEqualityComparer.Instance);
    private Dictionary<Guid, HashSet<ArtifactPropertyValueChangedCallback>> m_notifications = new Dictionary<Guid, HashSet<ArtifactPropertyValueChangedCallback>>();
    private ILockName m_notificationsLock;
    private ILockName m_rwLockName;
    private long m_completedRefreshId;
    private long m_outstandingRefreshId = 1;
    private Dictionary<Guid, ArtifactKind> m_artifactKindCache;
    private const string s_selectList = "SELECT  DISTINCT\r\n        pv1.ArtifactId\r\nFROM    tbl_PropertyValue pv1\r\n{0}WHERE   {1}";
    private const string s_selectList2 = "SELECT  DISTINCT\r\n        pv1.ArtifactId, pv1.DataspaceId\r\nFROM    tbl_PropertyValue pv1\r\n{0}WHERE   {1}";
    private const string s_joinCriteria = "JOIN    tbl_PropertyValue pv{0}\r\nON      ";
    private const string s_filterCriteria = "pv{0}.InternalKindId = {1}\r\n        AND pv{0}.PropertyId = {2}\r\n        AND pv{0}.{3} {4} @value{0}\r\n";
    private const string s_filterCriteriaWithPartitionId = "pv{0}.PartitionId = {5}\r\n        AND pv{0}.InternalKindId = {1}\r\n        AND pv{0}.PropertyId = {2}\r\n        AND pv{0}.{3} {4} @value{0}\r\n";
    private const string s_artifactEquality = "        AND pv{0}.ArtifactId = pv{1}.ArtifactId\r\n";
    private const string s_artifactEqualityWithDataspaceId = "        AND pv{0}.ArtifactId = pv{1}.ArtifactId AND pv{0}.DataspaceId = pv{1}.DataspaceId\r\n";
    public static readonly string[] AllPropertiesFilter = new string[1]
    {
      "*"
    };
    private const string c_area = "TeamFoundationPropertyService";
    private const string c_layer = "TeamFoundationPropertyService";

    internal TeamFoundationPropertyService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      this.m_rwLockName = requestContext.ServiceHost.CreateUniqueLockName(nameof (TeamFoundationPropertyService));
      this.m_notificationsLock = requestContext.ServiceHost.CreateUniqueLockName("TeamFoundationPropertyService.Notifications");
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.PropertyKindChanged, new SqlNotificationCallback(this.OnPropertyKindsChanged), true);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void CreateArtifactKind(IVssRequestContext requestContext, ArtifactKind artifactKind)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ArtifactKind>(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckForNull<string>(artifactKind.DataspaceCategory, "artifactKind.DataspaceCategory");
      ArgumentUtility.CheckForEmptyGuid(artifactKind.Kind, "artifactKind.Kind");
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>("Default"))
        component.CreateArtifactKind(artifactKind);
      this.InvalidatePropertyKindCache();
    }

    public IEnumerable<ArtifactKind> GetArtifactKinds(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IEnumerable<ArtifactKind>) this.GetArtifactKindCache(requestContext).Values.Select<ArtifactKind, ArtifactKind>((Func<ArtifactKind, ArtifactKind>) (x => x.Clone())).ToList<ArtifactKind>();
    }

    public ArtifactKind GetArtifactKind(IVssRequestContext requestContext, Guid kind) => this.GetArtifactKindInternal(requestContext, kind).Clone();

    private ArtifactKind GetArtifactKindInternal(IVssRequestContext requestContext, Guid kind)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArtifactKind artifactKindInternal;
      if (!this.GetArtifactKindCache(requestContext).TryGetValue(kind, out artifactKindInternal))
      {
        this.InvalidatePropertyKindCache();
        if (!this.GetArtifactKindCache(requestContext).TryGetValue(kind, out artifactKindInternal))
          throw new PropertyServiceException(TFCommonResources.FailedToMapPropertyKindToArtifactKind((object) kind.ToString()));
      }
      return artifactKindInternal;
    }

    public void DeleteArtifactKind(IVssRequestContext requestContext, Guid kind)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(kind, nameof (kind));
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>("Default"))
        component.DeleteArtifactKind(kind);
      this.InvalidatePropertyKindCache();
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      ArtifactSpec artifactSpec,
      IEnumerable<string> propertyNameFilters)
    {
      return this.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
      {
        artifactSpec
      }, propertyNameFilters);
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters)
    {
      return this.GetProperties(requestContext, artifactSpecs, propertyNameFilters, PropertiesOptions.None);
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      PropertiesOptions options)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ArtifactSpec>>(artifactSpecs, nameof (artifactSpecs));
      if (propertyNameFilters != null)
      {
        foreach (string propertyNameFilter in propertyNameFilters)
          PropertyValidation.ValidatePropertyFilter(propertyNameFilter);
      }
      else
        propertyNameFilters = (IEnumerable<string>) TeamFoundationPropertyService.AllPropertiesFilter;
      if (!artifactSpecs.Any<ArtifactSpec>())
        return new TeamFoundationDataReader(new object[1]
        {
          (object) new StreamingCollection<ArtifactPropertyValue>()
        });
      CommandGetArtifactPropertyValue disposableObject = (CommandGetArtifactPropertyValue) null;
      try
      {
        disposableObject = new CommandGetArtifactPropertyValue(requestContext);
        disposableObject.Execute(artifactSpecs, propertyNameFilters, options);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      IEnumerable<string> propertyNameFilters)
    {
      return this.GetProperties(requestContext, kind, propertyNameFilters, new Guid?(Guid.Empty));
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      IEnumerable<string> propertyNameFilters,
      Guid? dataspaceIdentifier)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (propertyNameFilters == null)
        propertyNameFilters = (IEnumerable<string>) TeamFoundationPropertyService.AllPropertiesFilter;
      ArtifactKind artifactKindInternal = this.GetArtifactKindInternal(requestContext, kind);
      CommandGetArtifactPropertyValue disposableObject = (CommandGetArtifactPropertyValue) null;
      try
      {
        disposableObject = new CommandGetArtifactPropertyValue(requestContext);
        disposableObject.Execute(artifactKindInternal, propertyNameFilters, dataspaceIdentifier);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      string expression,
      IEnumerable<string> propertyNameFilters)
    {
      return this.GetProperties(requestContext, kind, expression, propertyNameFilters, new Guid[1]
      {
        Guid.Empty
      });
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      string expression,
      IEnumerable<string> propertyNameFilters,
      Guid[] dataspaceIdentifiers)
    {
      return this.GetProperties(requestContext, kind, expression, propertyNameFilters, dataspaceIdentifiers, PropertiesOptions.None);
    }

    public TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      string expression,
      IEnumerable<string> propertyNameFilters,
      Guid[] dataspaceIdentifiers,
      PropertiesOptions options)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(expression, nameof (expression));
      QueryExpression queryExpression = new QueryExpression(expression);
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
      ArtifactKind artifactKindInternal = this.GetArtifactKindInternal(requestContext, kind);
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>(this.GetDataspaceCategory(requestContext, kind)))
      {
        TeamFoundationPropertyService.PropertyQuery propertyQuery = this.BuildPropertiesQuery(requestContext, artifactKindInternal, queryExpression.Tokens, component, dataspaceIdentifiers);
        if (!string.IsNullOrEmpty(propertyQuery.QueryString))
        {
          ResultCollection artifactsForQuery = component.GetArtifactsForQuery(kind, propertyQuery.QueryString, propertyQuery.Parameters);
          artifactSpecList.AddRange((IEnumerable<ArtifactSpec>) artifactsForQuery.GetCurrent<ArtifactSpec>().Items);
        }
      }
      return this.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, propertyNameFilters, options);
    }

    public bool SetProperties(
      IVssRequestContext requestContext,
      ArtifactSpec artifactSpec,
      IEnumerable<PropertyValue> propertyValues)
    {
      ArtifactPropertyValue[] artifactPropertyValueArray = new ArtifactPropertyValue[1]
      {
        new ArtifactPropertyValue(artifactSpec, propertyValues)
      };
      return this.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueArray);
    }

    public bool SetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues)
    {
      return this.SetProperties(requestContext, artifactPropertyValues, new DateTime?(), new Guid?());
    }

    public bool SetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      DateTime? changedDate,
      Guid? changedBy)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ArtifactPropertyValue>>(artifactPropertyValues, nameof (artifactPropertyValues));
      if (!artifactPropertyValues.Any<ArtifactPropertyValue>())
        return false;
      ArtifactPropertyValue var = artifactPropertyValues.FirstOrDefault<ArtifactPropertyValue>();
      ArgumentUtility.CheckForNull<ArtifactPropertyValue>(var, "propertyValue");
      ArgumentUtility.CheckForNull<ArtifactSpec>(var.Spec, "propertyValue.Spec");
      ArtifactKind artifactKindInternal = this.GetArtifactKindInternal(requestContext, var.Spec.Kind);
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>(this.GetDataspaceCategory(requestContext, var.Spec.Kind)))
        return component.SetPropertyValue(artifactPropertyValues, artifactKindInternal, changedDate, changedBy);
    }

    public bool SetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<PropertyValue> propertyValues)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ArtifactSpec>>(artifactSpecs, nameof (artifactSpecs));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) propertyValues, nameof (propertyValues));
      List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
      foreach (ArtifactSpec artifactSpec in artifactSpecs)
        artifactPropertyValueList.Add(new ArtifactPropertyValue(artifactSpec, propertyValues));
      return this.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList);
    }

    public int DeleteProperties(
      IVssRequestContext requestContext,
      Guid kind,
      IEnumerable<string> propertyNames,
      int batchSize = 2000,
      int? maxPropertiesToDelete = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) propertyNames, "propertyValues");
      ArgumentUtility.CheckForEmptyGuid(kind, nameof (kind));
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>(this.GetDataspaceCategory(requestContext, kind)))
        return component.DeleteProperties(kind, propertyNames, batchSize, maxPropertiesToDelete);
    }

    public void DeleteArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs)
    {
      this.DeleteArtifacts(requestContext, artifactSpecs, PropertiesOptions.None);
    }

    public void DeleteArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      PropertiesOptions options)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ArtifactSpec>>(artifactSpecs, nameof (artifactSpecs));
      if (!artifactSpecs.Any<ArtifactSpec>())
        return;
      ArtifactSpec var = artifactSpecs.FirstOrDefault<ArtifactSpec>();
      ArgumentUtility.CheckForNull<ArtifactSpec>(var, "spec");
      ArtifactKind artifactKindInternal = this.GetArtifactKindInternal(requestContext, var.Kind);
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>(this.GetDataspaceCategory(requestContext, var.Kind)))
        component.DeleteArtifacts(artifactSpecs, artifactKindInternal, options);
    }

    private TeamFoundationPropertyService.PropertyQuery BuildPropertiesQuery(
      IVssRequestContext requestContext,
      ArtifactKind artifactKind,
      IEnumerable<ExpressionToken> tokens,
      PropertyComponent component,
      Guid[] dataspaceIdentifiers)
    {
      List<SqlParameter> parameters = new List<SqlParameter>();
      StringBuilder completedQuery = new StringBuilder();
      int version = component.Version;
      int[] dataspaceIds = component.GetDataspaceIds(dataspaceIdentifiers);
      bool firstQueryAppended = false;
      for (int index = 0; index < dataspaceIdentifiers.Length; ++index)
      {
        StringBuilder joinClause = new StringBuilder();
        int num = 1;
        string whereClause = string.Empty;
        IEnumerator<ExpressionToken> enumerator = tokens.GetEnumerator();
        while (enumerator.MoveNext())
        {
          ExpressionToken current1 = enumerator.Current;
          switch (current1.TokenType)
          {
            case TokenType.OpenParen:
            case TokenType.CloseParen:
            case TokenType.And:
              continue;
            case TokenType.StringToken:
              enumerator.MoveNext();
              ExpressionToken current2 = enumerator.Current;
              enumerator.MoveNext();
              ExpressionToken current3 = enumerator.Current;
              int length = current1.Value.LastIndexOf('.');
              if (length <= 0)
                throw new InvalidOperationException("Property Name must be of the format {Property Name}.{LeadingStringValue|IntValue|DateTimeValue}");
              string str1 = current1.Value.Substring(length + 1);
              string str2;
              switch (str1.ToLowerInvariant())
              {
                case "intvalue":
                  str2 = "IntValue";
                  break;
                case "datetimevalue":
                  str2 = "DateTimeValue";
                  break;
                case "leadingstringvalue":
                  str2 = "LeadingStringValue";
                  break;
                default:
                  throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid column name {0}", (object) str1));
              }
              PropertyDefinition cachedDefinition = this.GetCachedDefinition(requestContext, dataspaceIdentifiers[index], current1.Value.Substring(0, length), component);
              if (cachedDefinition != null)
              {
                int propertyId = cachedDefinition.PropertyId;
                parameters.Add(new SqlParameter(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "@value{0}", (object) num), (object) current3.Value));
                if (num == 1)
                {
                  if (version == 1)
                    whereClause = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "pv{0}.InternalKindId = {1}\r\n        AND pv{0}.PropertyId = {2}\r\n        AND pv{0}.{3} {4} @value{0}\r\n", (object) num, (object) artifactKind.CompactKindId, (object) propertyId, (object) str2, (object) current2.Value);
                  else
                    whereClause = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "pv{0}.PartitionId = {5}\r\n        AND pv{0}.InternalKindId = {1}\r\n        AND pv{0}.PropertyId = {2}\r\n        AND pv{0}.{3} {4} @value{0}\r\n", (object) num, (object) artifactKind.CompactKindId, (object) propertyId, (object) str2, (object) current2.Value, (object) "@partitionId");
                  if (version >= 7)
                  {
                    whereClause += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AND pv{0}.DataspaceId = @dataspaceId{1}\r\n", (object) num, (object) index);
                    parameters.Add(new SqlParameter(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "@dataspaceId{0}", (object) index), (object) dataspaceIds[index]));
                  }
                }
                else
                {
                  joinClause.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "JOIN    tbl_PropertyValue pv{0}\r\nON      ", (object) num));
                  if (version == 1)
                    joinClause.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "pv{0}.InternalKindId = {1}\r\n        AND pv{0}.PropertyId = {2}\r\n        AND pv{0}.{3} {4} @value{0}\r\n", (object) num, (object) artifactKind.CompactKindId, (object) propertyId, (object) str2, (object) current2.Value));
                  else
                    joinClause.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "pv{0}.PartitionId = {5}\r\n        AND pv{0}.InternalKindId = {1}\r\n        AND pv{0}.PropertyId = {2}\r\n        AND pv{0}.{3} {4} @value{0}\r\n", (object) num, (object) artifactKind.CompactKindId, (object) propertyId, (object) str2, (object) current2.Value, (object) "@partitionId"));
                  joinClause.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, version < 7 ? "        AND pv{0}.ArtifactId = pv{1}.ArtifactId\r\n" : "        AND pv{0}.ArtifactId = pv{1}.ArtifactId AND pv{0}.DataspaceId = pv{1}.DataspaceId\r\n", (object) num, (object) (num - 1)));
                }
                ++num;
                continue;
              }
              continue;
            case TokenType.Or:
              if (current1.TokenType == TokenType.Or)
              {
                this.CompleteQuery(completedQuery, joinClause, whereClause, version, ref firstQueryAppended);
                num = 1;
                continue;
              }
              continue;
            default:
              throw new InvalidProgramException(FrameworkResources.QueryExpression_Malformed());
          }
        }
        this.CompleteQuery(completedQuery, joinClause, whereClause, version, ref firstQueryAppended);
      }
      if (version != 1 && completedQuery.Length != 0)
        completedQuery.Append("OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))");
      return new TeamFoundationPropertyService.PropertyQuery(completedQuery.ToString(), (IEnumerable<SqlParameter>) parameters);
    }

    private void CompleteQuery(
      StringBuilder completedQuery,
      StringBuilder joinClause,
      string whereClause,
      int version,
      ref bool firstQueryAppended)
    {
      if (string.IsNullOrEmpty(whereClause))
        return;
      if (firstQueryAppended)
        completedQuery.Append("\r\nUNION\r\n\r\n");
      else
        firstQueryAppended = true;
      completedQuery.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, version < 7 ? "SELECT  DISTINCT\r\n        pv1.ArtifactId\r\nFROM    tbl_PropertyValue pv1\r\n{0}WHERE   {1}" : "SELECT  DISTINCT\r\n        pv1.ArtifactId, pv1.DataspaceId\r\nFROM    tbl_PropertyValue pv1\r\n{0}WHERE   {1}", (object) joinClause.ToString(), (object) whereClause));
      joinClause.Length = 0;
    }

    private PropertyDefinition GetCachedDefinition(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      string definitionName,
      PropertyComponent propertyComponent)
    {
      KeyValuePair<Guid, string> key1 = TeamFoundationPropertyService.MakeDefinitionCacheKey(dataspaceIdentifier, definitionName);
      KeyValuePair<Guid, string> key2 = TeamFoundationPropertyService.MakeDefinitionCacheKey(PropertyDefinition.UninitializedDataspaceIdentifer, definitionName);
      PropertyDefinition cachedDefinition;
      if (!this.m_definitionCache.TryGetValue(key1, out cachedDefinition) && !this.m_definitionCache.TryGetValue(key2, out cachedDefinition))
      {
        foreach (PropertyDefinition propertyDefinition in this.GetPropertyDefinitions(requestContext, (IEnumerable<string>) new string[1]
        {
          definitionName
        }, propertyComponent))
          this.m_definitionCache[TeamFoundationPropertyService.MakeDefinitionCacheKey(propertyDefinition.DataspaceIdentifier, propertyDefinition.Name)] = propertyDefinition;
        if (!this.m_definitionCache.TryGetValue(key1, out cachedDefinition) && !this.m_definitionCache.TryGetValue(key2, out cachedDefinition))
          cachedDefinition = (PropertyDefinition) null;
      }
      return cachedDefinition;
    }

    private static KeyValuePair<Guid, string> MakeDefinitionCacheKey(
      Guid dataspaceIdentifier,
      string definitionName)
    {
      return new KeyValuePair<Guid, string>(dataspaceIdentifier, definitionName);
    }

    private IEnumerable<PropertyDefinition> GetPropertyDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<string> propertyNameFilters,
      PropertyComponent propertyComponent)
    {
      return (IEnumerable<PropertyDefinition>) propertyComponent.GetPropertyDefinitions(propertyNameFilters).GetCurrent<PropertyDefinition>().Items;
    }

    private string GetDataspaceCategory(IVssRequestContext requestContext, Guid kind) => this.GetArtifactKindInternal(requestContext, kind).DataspaceCategory;

    private IReadOnlyDictionary<Guid, ArtifactKind> GetArtifactKindCache(
      IVssRequestContext requestContext)
    {
      long num = Interlocked.Read(ref this.m_outstandingRefreshId);
      using (requestContext.AcquireReaderLock(this.m_rwLockName))
      {
        if (this.m_completedRefreshId >= num)
          return (IReadOnlyDictionary<Guid, ArtifactKind>) this.m_artifactKindCache;
      }
      Dictionary<Guid, ArtifactKind> dictionary;
      using (PropertyComponent component = requestContext.CreateComponent<PropertyComponent>())
        dictionary = component.GetPropertyKinds().ToDictionary<ArtifactKind, Guid>((Func<ArtifactKind, Guid>) (s => s.Kind));
      using (requestContext.AcquireReaderLock(this.m_rwLockName))
      {
        if (this.m_completedRefreshId >= num)
          return (IReadOnlyDictionary<Guid, ArtifactKind>) this.m_artifactKindCache;
      }
      using (requestContext.AcquireWriterLock(this.m_rwLockName))
      {
        if (this.m_completedRefreshId >= num)
          return (IReadOnlyDictionary<Guid, ArtifactKind>) this.m_artifactKindCache;
        this.m_completedRefreshId = num;
        this.m_artifactKindCache = dictionary;
      }
      return (IReadOnlyDictionary<Guid, ArtifactKind>) dictionary;
    }

    private void InvalidatePropertyKindCache() => Interlocked.Increment(ref this.m_outstandingRefreshId);

    private void OnPropertyKindsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.InvalidatePropertyKindCache();
    }

    private ILookup<string, Guid> GetDataspaceCategoryMap(
      IVssRequestContext requestContext,
      IEnumerable<Guid> artifactKinds)
    {
      if (artifactKinds == null)
        artifactKinds = this.GetArtifactKinds(requestContext).Select<ArtifactKind, Guid>((Func<ArtifactKind, Guid>) (kind => kind.Kind));
      return (artifactKinds ?? Enumerable.Empty<Guid>()).ToLookup<Guid, string>((Func<Guid, string>) (kind => this.GetDataspaceCategory(requestContext, kind)));
    }

    public void RegisterNotification(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ArtifactPropertyValueChangedCallback callback)
    {
      ArtifactKind artifactKindInternal = this.GetArtifactKindInternal(requestContext, artifactKind);
      if (!artifactKindInternal.Flags.HasFlag((Enum) ArtifactKindFlags.SendSqlNotificationOnWrites))
        throw new InvalidOperationException(string.Format("Cannot subscribe to a notification on kind {0} since it does not have the {1} flag set.", (object) artifactKind, (object) ArtifactKindFlags.SendSqlNotificationOnWrites));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, artifactKindInternal.DataspaceCategory, artifactKind, new SqlNotificationHandler(this.DispatchArtifactPropertyValueNotification), false);
      using (requestContext.Lock(this.m_notificationsLock))
      {
        if (!this.m_notifications.TryGetValue(artifactKind, out HashSet<ArtifactPropertyValueChangedCallback> _))
          this.m_notifications[artifactKind] = new HashSet<ArtifactPropertyValueChangedCallback>();
        this.m_notifications[artifactKind].Add(callback);
      }
    }

    private void DispatchArtifactPropertyValueNotification(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      List<ArtifactPropertyValueChangedCallback> valueChangedCallbackList = (List<ArtifactPropertyValueChangedCallback>) null;
      using (requestContext.Lock(this.m_notificationsLock))
      {
        HashSet<ArtifactPropertyValueChangedCallback> collection;
        if (this.m_notifications.TryGetValue(args.Class, out collection))
        {
          valueChangedCallbackList = new List<ArtifactPropertyValueChangedCallback>(collection.Count);
          valueChangedCallbackList.AddRange((IEnumerable<ArtifactPropertyValueChangedCallback>) collection);
        }
      }
      if (valueChangedCallbackList == null)
        return;
      ArtifactPropertyValueKey[] propertyValues = args.Deserialize<ArtifactPropertyValueKey[]>();
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      foreach (ArtifactPropertyValueKey propertyValueKey in propertyValues)
        propertyValueKey.Spec.DataspaceIdentifier = service.QueryDataspace(requestContext, propertyValueKey.DataspaceId).DataspaceIdentifier;
      foreach (ArtifactPropertyValueChangedCallback valueChangedCallback in valueChangedCallbackList)
        valueChangedCallback(requestContext, (IEnumerable<ArtifactPropertyValueKey>) propertyValues);
    }

    public void UnregisterNotification(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ArtifactPropertyValueChangedCallback callback)
    {
      ArtifactKind artifactKindInternal = this.GetArtifactKindInternal(requestContext, artifactKind);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, artifactKindInternal.DataspaceCategory, artifactKind, new SqlNotificationHandler(this.DispatchArtifactPropertyValueNotification), false);
      using (requestContext.Lock(this.m_notificationsLock))
      {
        HashSet<ArtifactPropertyValueChangedCallback> valueChangedCallbackSet;
        if (!this.m_notifications.TryGetValue(artifactKind, out valueChangedCallbackSet) || !valueChangedCallbackSet.Remove(callback) || valueChangedCallbackSet.Count != 1)
          return;
        this.m_notifications.Remove(artifactKind);
      }
    }

    private class PropertyQuery
    {
      public PropertyQuery(string queryString, IEnumerable<SqlParameter> parameters)
      {
        this.QueryString = queryString;
        this.Parameters = parameters;
      }

      public string QueryString { get; set; }

      public IEnumerable<SqlParameter> Parameters { get; set; }
    }

    private class PropertyDefinitionEqualityComparer : IEqualityComparer<KeyValuePair<Guid, string>>
    {
      public static readonly IEqualityComparer<KeyValuePair<Guid, string>> Instance = (IEqualityComparer<KeyValuePair<Guid, string>>) new TeamFoundationPropertyService.PropertyDefinitionEqualityComparer();

      public bool Equals(KeyValuePair<Guid, string> x, KeyValuePair<Guid, string> y) => x.Key == y.Key && StringComparer.OrdinalIgnoreCase.Equals(x.Value, y.Value);

      public int GetHashCode(KeyValuePair<Guid, string> obj) => obj.Key.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
  }
}
