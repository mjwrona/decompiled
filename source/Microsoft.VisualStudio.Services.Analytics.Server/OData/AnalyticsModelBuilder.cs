// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.AnalyticsModelBuilder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData
{
  public class AnalyticsModelBuilder
  {
    private Dictionary<string, Type> _dynamicEntitySets = new Dictionary<string, Type>();
    private HttpConfiguration _httpConfig;
    private const string DisplayNameSpace = "Display";
    private const string ReferenceNameNamespace = "Ref";
    private static readonly EdmTerm _displayNameTerm = new EdmTerm("Display", "DisplayName", EdmPrimitiveTypeKind.String);
    private static readonly EdmTerm _descriptionTerm = new EdmTerm("Display", "Description", EdmPrimitiveTypeKind.String);
    private static readonly EdmTerm _referenceNameTerm = new EdmTerm("Ref", "ReferenceName", EdmPrimitiveTypeKind.String);
    private IEdmEntityType _userEntityType;
    private IDictionary<string, bool> _featureFlags;
    internal static readonly Dictionary<Type, string> s_clrTypeToEntitySetName = new Dictionary<Type, string>();
    internal static readonly HashSet<string> s_entityNames = new HashSet<string>();
    internal const string ApproxCountDistinctFunction = "ax.ApproxCountDistinct";
    internal const string StandardDeviationFunction = "ax.StandardDeviation";
    internal const string StandardDeviationPFunction = "ax.StandardDeviationP";
    internal const string VarianceFunction = "ax.Variance";
    internal const string VariancePFunction = "ax.VarianceP";
    private static Dictionary<Type, List<BoundMethodDefinition>> s_boundMethods = new Dictionary<Type, List<BoundMethodDefinition>>();

    public Action<ODataConventionModelBuilder> OnModelCreating { get; set; }

    static AnalyticsModelBuilder()
    {
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (WorkItemRevision), "WorkItemRevisions");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (WorkItem), "WorkItems");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (WorkItemSnapshot), "WorkItemSnapshot");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (WorkItemBoardSnapshot), "WorkItemBoardSnapshot");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (WorkItemLink), "WorkItemLinks");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (CalendarDate), "Dates");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Project), "Projects");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Iteration), "Iterations");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Area), "Areas");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Tag), "Tags");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Team), "Teams");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (BoardLocation), "BoardLocations");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (User), "Users");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (WorkItemTypeField), "WorkItemTypeFields");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestRun), "TestRuns");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestResult), "TestResults");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestResultDaily), "TestResultsDaily");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestConfiguration), "TestConfigurations");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestSuite), "TestSuites");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestPoint), "TestPoints");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TestPointHistorySnapshot), "TestPointHistorySnapshot");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Test), "Tests");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (PipelineRun), "PipelineRuns");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Branch), "Branches");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Process), "Processes");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (Pipeline), "Pipelines");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (PipelineTask), "PipelineTasks");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (PipelineRunActivityResult), "PipelineRunActivityResults");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (PipelineJob), "PipelineJobs");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (PipelineEnvironment), "PipelineEnvironments");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TaskAgentPoolSizeSnapshot), "TaskAgentPoolSizeSnapshots");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (CommitToDeployment), "CommitToDeployment");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubAccount), "GitHubAccounts");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubRepository), "GitHubRepositories");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubTeam), "GitHubTeams");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubUser), "GitHubUsers");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubCommit), "GitHubCommits");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubPullRequest), "GitHubPullRequest");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubPullRequestSnapshot), "GitHubPullRequestSnapshot");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubBranch), "GitHubBranch");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubContributorActivity), "GitHubContributorActivities");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (GitHubPullRequestReviewerActivity), "GitHubPullRequestReviewerActivities");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (ParallelPipelineJobsSnapshot), "ParallelPipelineJobsSnapshot");
      AnalyticsModelBuilder.s_clrTypeToEntitySetName.Add(typeof (TaskAgentRequestSnapshot), "TaskAgentRequestSnapshots");
      AnalyticsModelBuilder.s_entityNames.AddRange<string, HashSet<string>>((IEnumerable<string>) AnalyticsModelBuilder.s_clrTypeToEntitySetName.Values);
      AnalyticsModelBuilder.RegisterModelBoundMethods();
      AnalyticsModelBuilder.RegisterCustomFunctions();
    }

    public AnalyticsModelBuilder(
      HttpConfiguration httpConfig = null,
      IDictionary<string, bool> featureFlags = null)
    {
      this._httpConfig = httpConfig;
      this._featureFlags = featureFlags;
    }

    public IEdmModel GetEdmModel(
      ModelCreationProcessFields modelCreationParameter,
      Type contextType = null,
      int dbServiceVersion = 2147483647,
      int odataModelVersion = 2147483647)
    {
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) odataModelVersion, nameof (odataModelVersion));
      bool flag1 = false;
      if (contextType == (Type) null)
        flag1 = true;
      ODataConventionModelBuilder builder = this._httpConfig != null ? new ODataConventionModelBuilder(this._httpConfig) : new ODataConventionModelBuilder();
      this.AddEntities(odataModelVersion, dbServiceVersion, builder);
      if (odataModelVersion >= 3)
      {
        builder.AddEnumType(typeof (PredictModel));
        if (this.IsFeatureEnabled("Analytics.Transform.WorkItemTagsPredict") && builder.EntitySets.Any<EntitySetConfiguration>((Func<EntitySetConfiguration, bool>) (e => e.Name == "Tags")))
        {
          FunctionConfiguration functionConfiguration = builder.EntityType<WorkItem>().Collection.Function("RecommendTags");
          functionConfiguration.Namespace = "Microsoft.VisualStudio.Services.Analytics";
          functionConfiguration.ReturnsCollectionFromEntitySet<Tag>("Tags");
          functionConfiguration.Parameter<int>("WorkItemId");
        }
      }
      Dictionary<string, IEnumerable<EdmVocabularyAnnotationAttribute>> entityAnnotations = new Dictionary<string, IEnumerable<EdmVocabularyAnnotationAttribute>>();
      Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>> propertiesWithAnnotations = new Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>>();
      Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>> enumMembersWithAnnotations = new Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>>();
      HashSet<string> revisionlessReferenceNames = (HashSet<string>) null;
      HashSet<string> builtInReferenceNames = (HashSet<string>) null;
      Dictionary<string, ProcessFieldInfo> builtInProcessFieldsMap = (Dictionary<string, ProcessFieldInfo>) null;
      if (modelCreationParameter != null)
      {
        revisionlessReferenceNames = new HashSet<string>(modelCreationParameter.ProcessFields.Where<ProcessFieldInfo>((Func<ProcessFieldInfo, bool>) (f => !f.IsHistoryEnabled)).Select<ProcessFieldInfo, string>((Func<ProcessFieldInfo, string>) (f => f.ReferenceName)));
        builtInReferenceNames = new HashSet<string>(modelCreationParameter.BuiltInProcessFields.Select<ProcessFieldInfo, string>((Func<ProcessFieldInfo, string>) (f => f.ReferenceName)));
        builtInProcessFieldsMap = modelCreationParameter.BuiltInProcessFields.ToDictionary<ProcessFieldInfo, string, ProcessFieldInfo>((Func<ProcessFieldInfo, string>) (f => f.ReferenceName), (Func<ProcessFieldInfo, ProcessFieldInfo>) (f => f));
      }
      bool removeRevisionLess = revisionlessReferenceNames.AsEmptyIfNull<HashSet<string>>().Any<string>();
      bool removeBuiltIn = builtInReferenceNames.AsEmptyIfNull<HashSet<string>>().Any<string>();
      builder.OnModelCreating = (Action<ODataConventionModelBuilder>) (b =>
      {
        foreach (EntitySetConfiguration entitySet in b.EntitySets)
        {
          this.PrepareAnnotationsForProperties(entitySet.EntityType, builtInProcessFieldsMap.AsEmptyIfNull<Dictionary<string, ProcessFieldInfo>>(), propertiesWithAnnotations, odataModelVersion);
          entityAnnotations.Add(entitySet.EntityType.FullName, entitySet.ClrType.GetCustomAttributes<EdmVocabularyAnnotationAttribute>(true));
        }
        foreach (EnumTypeConfiguration enumType in b.EnumTypes)
          this.PrepareAnnotationsForEnumMembers(enumType, enumMembersWithAnnotations, odataModelVersion);
        if (this.OnModelCreating != null)
          this.OnModelCreating(b);
        foreach (EntitySetConfiguration entitySet in b.EntitySets)
        {
          HashSet<PropertyInfo> propertyInfoSet = new HashSet<PropertyInfo>();
          List<NavigationPropertyConfiguration> propertyConfigurationList = new List<NavigationPropertyConfiguration>();
          if (typeof (WorkItemPrimitive).IsAssignableFrom(entitySet.ClrType) && removeRevisionLess | removeBuiltIn)
          {
            bool flag2 = AnalyticsModelBuilder.IsHistoricalEntity(entitySet.ClrType);
            foreach (StructuralPropertyConfiguration propertyConfiguration in entitySet.EntityType.Properties.OfType<StructuralPropertyConfiguration>().Where<StructuralPropertyConfiguration>((Func<StructuralPropertyConfiguration, bool>) (p => p.Kind == PropertyKind.Primitive && p.OptionalProperty)))
            {
              string referenceName = propertyConfiguration.PropertyInfo.GetVersionedModifier<ReferenceNameAttribute>(odataModelVersion)?.ReferenceName;
              if (referenceName != null && (removeRevisionLess & flag2 && revisionlessReferenceNames.Contains(referenceName) || removeBuiltIn && !builtInReferenceNames.Contains(referenceName) && !referenceName.StartsWith("System.")))
                propertyInfoSet.Add(propertyConfiguration.PropertyInfo);
            }
          }
          foreach (PropertyConfiguration property in entitySet.EntityType.Properties)
          {
            if (property.PropertyInfo.GetVersionedModifier<ODataHideAttribute>(odataModelVersion) != null || property.PropertyInfo.GetVersionedModifier<ODataHideByServiceVersionAttribute>(dbServiceVersion) != null || property.PropertyInfo.GetVersionedModifier<DatabaseHideAttribute>(dbServiceVersion) != null)
            {
              propertyInfoSet.Add(property.PropertyInfo);
              foreach (NavigationPropertyBindingConfiguration binding in entitySet.FindBindings(property.Name))
                propertyConfigurationList.Add(binding.NavigationProperty);
            }
            else
            {
              VersionedRenameAttribute versionedRenameAttribute = (VersionedRenameAttribute) property.PropertyInfo.GetVersionedModifier<ODataRenameAttribute>(odataModelVersion) ?? (VersionedRenameAttribute) property.PropertyInfo.GetVersionedModifier<ODataRenameByServiceVersionAttribute>(dbServiceVersion);
              if (versionedRenameAttribute != null)
                property.Name = versionedRenameAttribute.Name;
            }
          }
          foreach (PropertyInfo propertyInfo in propertyInfoSet)
            entitySet.EntityType.RemoveProperty(propertyInfo);
          foreach (NavigationPropertyConfiguration navigationConfiguration in propertyConfigurationList)
            entitySet.RemoveBinding(navigationConfiguration);
        }
        HashSet<Type> hashSet = b.StructuralTypes.Select<StructuralTypeConfiguration, Type>((Func<StructuralTypeConfiguration, Type>) (s => s.ClrType)).ToHashSet<Type>();
        HashSet<Type> entityTypes = b.EntitySets.Select<EntitySetConfiguration, Type>((Func<EntitySetConfiguration, Type>) (es => es.ClrType)).ToHashSet<Type>();
        HashSet<Type> second = entityTypes;
        foreach (Type type in hashSet.Except<Type>((IEnumerable<Type>) second))
          b.RemoveStructuralType(type);
        foreach (EntitySetConfiguration entitySet in b.EntitySets)
        {
          foreach (NavigationPropertyConfiguration navigationConfiguration in entitySet.EntityType.NavigationProperties.Where<NavigationPropertyConfiguration>((Func<NavigationPropertyConfiguration, bool>) (x => !entityTypes.Contains(x.RelatedClrType))).ToList<NavigationPropertyConfiguration>())
          {
            entitySet.EntityType.RemoveProperty(navigationConfiguration.PropertyInfo);
            entitySet.RemoveBinding(navigationConfiguration);
          }
        }
      });
      EdmModel edmModel = builder.GetEdmModel() as EdmModel;
      this._userEntityType = (IEdmEntityType) (edmModel.EntityContainer.EntitySets().FirstOrDefault<IEdmEntitySet>((Func<IEdmEntitySet, bool>) (e => e.Name == "Users")).EntityType() as EdmEntityType);
      if (!flag1 && modelCreationParameter != null && modelCreationParameter.HasCustomFields)
      {
        foreach (KeyValuePair<string, Type> dynamicEntitySet in this._dynamicEntitySets)
          this.AddCustomFields(edmModel, (IEnumerable<ProcessFieldInfo>) modelCreationParameter.CustomProcessFields, dynamicEntitySet.Value, dynamicEntitySet.Key, revisionlessReferenceNames, dbServiceVersion);
      }
      this.AddDirectValueAnnotations(edmModel);
      this.AddPropertyAnnotations(edmModel, propertiesWithAnnotations);
      this.AddEnumMemberAnnotations(edmModel, enumMembersWithAnnotations);
      this.AddCapabilitiesAnnotations(edmModel);
      this.AddVocabularyAnnotations(edmModel, entityAnnotations);
      this.AddCustomAggregations(edmModel);
      return (IEdmModel) edmModel;
    }

    private void AddCustomAggregations(EdmModel model)
    {
      CustomAggregateMethodAnnotation methodAnnotation = new CustomAggregateMethodAnnotation();
      methodAnnotation.AddMethod("ax.ApproxCountDistinct", (IDictionary<Type, MethodInfo>) SqlFunctions.GetAggregationCustomMethods(typeof (SqlFunctions), "ApproxCountDistinct"));
      methodAnnotation.AddMethod("ax.StandardDeviation", (IDictionary<Type, MethodInfo>) SqlFunctions.GetAggregationCustomMethods(typeof (DbFunctions), "StandardDeviation"));
      methodAnnotation.AddMethod("ax.StandardDeviationP", (IDictionary<Type, MethodInfo>) SqlFunctions.GetAggregationCustomMethods(typeof (DbFunctions), "StandardDeviationP"));
      methodAnnotation.AddMethod("ax.Variance", (IDictionary<Type, MethodInfo>) SqlFunctions.GetAggregationCustomMethods(typeof (DbFunctions), "Var"));
      methodAnnotation.AddMethod("ax.VarianceP", (IDictionary<Type, MethodInfo>) SqlFunctions.GetAggregationCustomMethods(typeof (DbFunctions), "VarP"));
      model.SetAnnotationValue<CustomAggregateMethodAnnotation>((IEdmElement) model, methodAnnotation);
    }

    private static void RegisterModelBoundMethods()
    {
      foreach (Type key in AnalyticsModelBuilder.s_clrTypeToEntitySetName.Keys)
      {
        List<BoundMethodDefinition> methodDefinitionList = new List<BoundMethodDefinition>();
        foreach (MethodInfo method in key.GetMethods())
        {
          BoundFunctionAttribute customAttribute = method.GetCustomAttribute<BoundFunctionAttribute>();
          if (customAttribute != null)
            methodDefinitionList.Add(BoundMethodDefinition.Create(method, AnalyticsModelBuilder.s_clrTypeToEntitySetName, customAttribute.MinVersion, customAttribute.MaxVersion));
        }
        if (methodDefinitionList.Count > 0)
          AnalyticsModelBuilder.s_boundMethods.Add(key, methodDefinitionList);
      }
    }

    private static void RegisterCustomFunctions()
    {
      AnalyticsModelBuilder.RegisterCustomFunction("PercentileCont", "percentile_cont");
      AnalyticsModelBuilder.RegisterCustomFunction("PercentileDisc", "percentile_disc");
      AnalyticsModelBuilder.RegisterCustomFunction("Lag", "lag");
      AnalyticsModelBuilder.RegisterCustomFunction("Lead", "lead");
      AnalyticsModelBuilder.RegisterCustomFunction("RowNumber", "row_number");
      EdmEnumTypeReference enumTypeReference = new EdmEnumTypeReference((IEdmEnumType) EdmTypeUtils.GetEdmEnumType(typeof (PredictModel)), false);
      foreach (BoundMethodDefinition methodDefinition in AnalyticsModelBuilder.s_boundMethods.Values.SelectMany<List<BoundMethodDefinition>, BoundMethodDefinition>((Func<List<BoundMethodDefinition>, IEnumerable<BoundMethodDefinition>>) (l => (IEnumerable<BoundMethodDefinition>) l)))
        UriFunctionsBinder.BindUriFunctionName("Microsoft.VisualStudio.Services.Analytics." + methodDefinition.Name, methodDefinition.Method);
    }

    private static void RegisterCustomFunction(string methodName, string odataName)
    {
      foreach (MethodInfo geCustomMethod in SqlFunctions.GeCustomMethods(methodName))
      {
        IEdmPrimitiveTypeReference[] array = ((IEnumerable<ParameterInfo>) geCustomMethod.GetParameters()).Select<ParameterInfo, IEdmPrimitiveTypeReference>((Func<ParameterInfo, IEdmPrimitiveTypeReference>) (p => EdmTypeUtils.GetEdmPrimitiveTypeReferenceOrNull(p.ParameterType))).ToArray<IEdmPrimitiveTypeReference>();
        FunctionSignatureWithReturnType functionSignature = new FunctionSignatureWithReturnType((IEdmTypeReference) array[0], (IEdmTypeReference[]) array);
        ODataUriFunctions.AddCustomUriFunction(odataName, functionSignature, geCustomMethod);
      }
    }

    protected virtual void AddEntities(
      int odataModelVersion,
      int dbServiceVersion,
      ODataConventionModelBuilder builder)
    {
      this.BuildEntitySet<WorkItemRevision>(builder, odataModelVersion, dbServiceVersion, true);
      this.BuildEntitySet<WorkItem>(builder, odataModelVersion, dbServiceVersion, true);
      this.BuildEntitySet<WorkItemSnapshot>(builder, odataModelVersion, dbServiceVersion, true);
      this.BuildEntitySet<WorkItemBoardSnapshot>(builder, odataModelVersion, dbServiceVersion, true);
      this.BuildEntitySet<WorkItemLink>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<CalendarDate>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Project>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Iteration>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Area>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Tag>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Team>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<BoardLocation>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<User>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<WorkItemTypeField>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<TestRun>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<TestResult>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Test>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<TestConfiguration>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.ManualTestEntities");
      this.BuildEntitySet<TestSuite>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.ManualTestEntities");
      this.BuildEntitySet<TestPoint>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.ManualTestEntities");
      this.BuildEntitySet<TestPointHistorySnapshot>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.ManualTestEntities");
      this.BuildEntitySet<PipelineRun>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Branch>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Process>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<TestResultDaily>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<Pipeline>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<PipelineTask>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<PipelineRunActivityResult>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.ShowBuildPipelineEntitiesFeatureFlag");
      this.BuildEntitySet<PipelineJob>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.ShowBuildPipelineEntitiesFeatureFlag");
      this.BuildEntitySet<PipelineEnvironment>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Transform.LeadTimeCommitToDeployment");
      this.BuildEntitySet<TaskAgentPoolSizeSnapshot>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<CommitToDeployment>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Transform.LeadTimeCommitToDeployment");
      this.BuildEntitySet<GitHubAccount>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubRepository>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubTeam>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubUser>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubCommit>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubPullRequest>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubPullRequestSnapshot>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubBranch>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubContributorActivity>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<GitHubPullRequestReviewerActivity>(builder, odataModelVersion, dbServiceVersion, false, "Analytics.Model.GitHubInsightsCode");
      this.BuildEntitySet<ParallelPipelineJobsSnapshot>(builder, odataModelVersion, dbServiceVersion, false);
      this.BuildEntitySet<TaskAgentRequestSnapshot>(builder, odataModelVersion, dbServiceVersion, false);
    }

    private static bool IsHistoricalEntity(Type type) => typeof (IHistoricalWorkItem).IsAssignableFrom(type);

    private void AddVocabularyAnnotations(
      EdmModel model,
      Dictionary<string, IEnumerable<EdmVocabularyAnnotationAttribute>> entityAnnotations)
    {
      foreach (IEdmEntitySet entitySet in model.EntityContainer.EntitySets())
      {
        IEnumerable<EdmVocabularyAnnotationAttribute> annotationAttributes = (IEnumerable<EdmVocabularyAnnotationAttribute>) null;
        if (entityAnnotations.TryGetValue(entitySet.EntityType().FullName(), out annotationAttributes))
        {
          foreach (EdmVocabularyAnnotationAttribute attribute in annotationAttributes)
            AnalyticsModelBuilder.AddVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) entitySet, attribute);
        }
      }
    }

    private static void AddVocabularyAnnotation(
      EdmModel model,
      IEdmVocabularyAnnotatable target,
      EdmVocabularyAnnotationAttribute attribute)
    {
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, attribute.TermInstance, attribute.Value);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(attribute.Location));
      model.AddVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    private void AddCustomFields(
      EdmModel model,
      IEnumerable<ProcessFieldInfo> fields,
      Type witClrType,
      string entitySet,
      HashSet<string> revisionlessReferenceNames,
      int dbServiceVersion)
    {
      EdmEntityType element = model.EntityContainer.EntitySets().First<IEdmEntitySet>((Func<IEdmEntitySet, bool>) (e => e.Name == entitySet)).EntityType() as EdmEntityType;
      List<string> source = new List<string>();
      IEnumerable<ProcessFieldInfo> processFieldInfos = fields;
      if (AnalyticsModelBuilder.IsHistoricalEntity(witClrType))
        processFieldInfos = fields.Where<ProcessFieldInfo>((Func<ProcessFieldInfo, bool>) (f => !revisionlessReferenceNames.Contains(f.ReferenceName)));
      foreach (ProcessFieldInfo processFieldInfo in processFieldInfos)
      {
        string name = processFieldInfo.TableName.Replace("WorkItemRevision", string.Empty);
        PropertyInfo property1 = witClrType.GetProperty(name);
        DatabaseHideAttribute customAttribute = property1.GetCustomAttribute<DatabaseHideAttribute>();
        if (customAttribute == null || !customAttribute.ShouldApply(dbServiceVersion))
        {
          EdmPrimitiveTypeKind edmType = processFieldInfo.EdmType;
          string str = edmType == EdmPrimitiveTypeKind.Guid ? "SK" : string.Empty;
          PropertyInfo property2 = property1.PropertyType.GetProperty(processFieldInfo.ColumnName + str);
          IEdmProperty edmProperty = (IEdmProperty) element.AddStructuralProperty(processFieldInfo.PropertyName + str, edmType, true);
          model.SetAnnotationValue<ClrPropertyInfoAnnotation>((IEdmElement) edmProperty, new ClrPropertyInfoAnnotation(property2)
          {
            PropertiesPath = (IList<PropertyInfo>) new List<PropertyInfo>()
            {
              property1
            }
          });
          if (edmType == EdmPrimitiveTypeKind.Guid)
          {
            PropertyInfo property3 = property1.PropertyType.GetProperty(processFieldInfo.ColumnName);
            IEdmStructuralProperty structuralProperty = edmProperty as IEdmStructuralProperty;
            edmProperty = (IEdmProperty) element.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
              ContainsTarget = false,
              Name = processFieldInfo.PropertyName,
              Target = this._userEntityType,
              TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
              DependentProperties = (IEnumerable<IEdmStructuralProperty>) new IEdmStructuralProperty[1]
              {
                structuralProperty
              },
              PrincipalProperties = (IEnumerable<IEdmStructuralProperty>) new IEdmStructuralProperty[1]
              {
                this._userEntityType.FindProperty("UserSK") as IEdmStructuralProperty
              }
            });
            model.SetAnnotationValue<ClrPropertyInfoAnnotation>((IEdmElement) edmProperty, new ClrPropertyInfoAnnotation(property3)
            {
              PropertiesPath = (IList<PropertyInfo>) new List<PropertyInfo>()
              {
                property1
              }
            });
          }
          AnalyticsModelBuilder.SetVocabularyAnnotation(model, edmProperty, (IEdmTerm) AnalyticsModelBuilder._displayNameTerm, processFieldInfo.Name);
          AnalyticsModelBuilder.SetVocabularyAnnotation(model, edmProperty, (IEdmTerm) AnalyticsModelBuilder._referenceNameTerm, processFieldInfo.ReferenceName);
          AnalyticsModelBuilder.SetVocabularyAnnotation(model, edmProperty, (IEdmTerm) AnalyticsModelBuilder._descriptionTerm, processFieldInfo.Description);
        }
      }
      if (source.Any<string>())
        model.SetAnnotationValue<IdentityPropertyAnnotation>((IEdmElement) model, new IdentityPropertyAnnotation()
        {
          IdentityColumnPaths = source
        });
      ModelBoundQuerySettings boundQuerySettings = model.GetAnnotationValue<ModelBoundQuerySettings>((IEdmElement) element) ?? new ModelBoundQuerySettings();
      boundQuerySettings.DefaultSelectType = new SelectExpandType?(SelectExpandType.Automatic);
      boundQuerySettings.MaxTop = new int?();
      model.SetAnnotationValue<ModelBoundQuerySettings>((IEdmElement) element, boundQuerySettings);
    }

    private void AddCapabilitiesAnnotations(EdmModel model)
    {
      AnalyticsModelBuilder.AddVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) model.EntityContainer, (EdmVocabularyAnnotationAttribute) FilterFunctionsAnnotationAttribute.Default);
      AnalyticsModelBuilder.AddVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) model.EntityContainer, (EdmVocabularyAnnotationAttribute) ApplySupportedAnnotationAttribute.Default);
      AnalyticsModelBuilder.AddVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) model.EntityContainer, (EdmVocabularyAnnotationAttribute) BatchSupportedAnnotationAttribute.Default);
      AnalyticsModelBuilder.AddVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) model.EntityContainer, (EdmVocabularyAnnotationAttribute) BatchSupportAnnotationAttribute.Default);
    }

    private void PrepareAnnotationsForProperties(
      EntityTypeConfiguration entity,
      Dictionary<string, ProcessFieldInfo> builtInProcessFieldsMap,
      Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>> propertiesWithAnnotations,
      int odataModelVersion)
    {
      IEnumerable<PropertyConfiguration> properties = entity.Properties;
      string name1 = entity.Name;
      Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>> dictionary = new Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>();
      foreach (PropertyConfiguration propertyConfiguration in properties)
      {
        string name2 = propertyConfiguration.PropertyInfo.Name;
        List<KeyValuePair<IEdmTerm, IEdmExpression>> keyValuePairList = new List<KeyValuePair<IEdmTerm, IEdmExpression>>();
        LocalizedDescriptionAttribute customAttribute1 = propertyConfiguration.PropertyInfo.GetCustomAttribute<LocalizedDescriptionAttribute>(true);
        if (customAttribute1 != null)
          keyValuePairList.Add(new KeyValuePair<IEdmTerm, IEdmExpression>((IEdmTerm) AnalyticsModelBuilder._descriptionTerm, (IEdmExpression) new EdmStringConstant(customAttribute1.Description)));
        ReferenceNameAttribute referenceNameAttr = propertyConfiguration.PropertyInfo.GetVersionedModifier<ReferenceNameAttribute>(odataModelVersion);
        string str;
        if (referenceNameAttr != null)
        {
          ProcessFieldInfo processFieldInfo = builtInProcessFieldsMap.FirstOrDefault<KeyValuePair<string, ProcessFieldInfo>>((Func<KeyValuePair<string, ProcessFieldInfo>, bool>) (x => x.Key == referenceNameAttr.ReferenceName)).Value;
          LocalizedDisplayNameAttribute customAttribute2 = propertyConfiguration.PropertyInfo.GetCustomAttribute<LocalizedDisplayNameAttribute>(true);
          str = customAttribute2 != null && customAttribute2.Force || processFieldInfo == null ? customAttribute2?.DisplayName : processFieldInfo.Name;
          keyValuePairList.Add(new KeyValuePair<IEdmTerm, IEdmExpression>((IEdmTerm) AnalyticsModelBuilder._referenceNameTerm, (IEdmExpression) new EdmStringConstant(referenceNameAttr.ReferenceName)));
        }
        else
          str = propertyConfiguration.PropertyInfo.GetCustomAttribute<LocalizedDisplayNameAttribute>(true)?.DisplayName;
        if (str != null)
          keyValuePairList.Add(new KeyValuePair<IEdmTerm, IEdmExpression>((IEdmTerm) AnalyticsModelBuilder._displayNameTerm, (IEdmExpression) new EdmStringConstant(str)));
        keyValuePairList.AddRange(propertyConfiguration.PropertyInfo.GetCustomAttributes<EdmVocabularyAnnotationAttribute>(true).Select<EdmVocabularyAnnotationAttribute, KeyValuePair<IEdmTerm, IEdmExpression>>((Func<EdmVocabularyAnnotationAttribute, KeyValuePair<IEdmTerm, IEdmExpression>>) (a => new KeyValuePair<IEdmTerm, IEdmExpression>(a.TermInstance, a.Value))));
        if (keyValuePairList.Count > 0)
          dictionary.Add(name2, keyValuePairList);
      }
      if (dictionary.Count <= 0)
        return;
      propertiesWithAnnotations.Add(name1, dictionary);
    }

    private void PrepareAnnotationsForEnumMembers(
      EnumTypeConfiguration enumType,
      Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>> enumMembersWithAnnotations,
      int odataModelVersion)
    {
      IEnumerable<EnumMemberConfiguration> members = enumType.Members;
      Type clrType = enumType.ClrType;
      Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>> dictionary = new Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>();
      foreach (EnumMemberConfiguration memberConfiguration in members)
      {
        MemberInfo element = ((IEnumerable<MemberInfo>) clrType.GetMember(memberConfiguration.Name)).FirstOrDefault<MemberInfo>();
        string displayName = ((object) element != null ? element.GetCustomAttribute(typeof (LocalizedDisplayNameAttribute)) : (Attribute) null) is LocalizedDisplayNameAttribute customAttribute ? customAttribute.DisplayName : (string) null;
        List<KeyValuePair<IEdmTerm, IEdmExpression>> keyValuePairList = new List<KeyValuePair<IEdmTerm, IEdmExpression>>();
        if (displayName != null)
          keyValuePairList.Add(new KeyValuePair<IEdmTerm, IEdmExpression>((IEdmTerm) AnalyticsModelBuilder._displayNameTerm, (IEdmExpression) new EdmStringConstant(displayName)));
        if (keyValuePairList.Count > 0)
          dictionary.Add(memberConfiguration.Name, keyValuePairList);
      }
      if (dictionary.Count <= 0)
        return;
      enumMembersWithAnnotations.Add(enumType.Name, dictionary);
    }

    private void AddEnumMemberAnnotations(
      EdmModel model,
      Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>> enumMembersWithAnnotations)
    {
      foreach (EdmEnumType edmEnumType in model.SchemaElements.Where<IEdmSchemaElement>((Func<IEdmSchemaElement, bool>) (x => x is EdmEnumType)).Select<IEdmSchemaElement, EdmEnumType>((Func<IEdmSchemaElement, EdmEnumType>) (x => x as EdmEnumType)))
      {
        Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>> dictionary;
        if (enumMembersWithAnnotations.TryGetValue(edmEnumType.Name, out dictionary))
        {
          foreach (IEdmEnumMember member in edmEnumType.Members)
          {
            List<KeyValuePair<IEdmTerm, IEdmExpression>> keyValuePairList;
            if (dictionary.TryGetValue(member.Name, out keyValuePairList))
            {
              foreach (KeyValuePair<IEdmTerm, IEdmExpression> keyValuePair in keyValuePairList)
                AnalyticsModelBuilder.SetVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) member, keyValuePair.Key, keyValuePair.Value);
            }
          }
        }
      }
    }

    private void AddPropertyAnnotations(
      EdmModel model,
      Dictionary<string, Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>>> propertiesWithAnnotations)
    {
      foreach (IEdmEntitySet entitySet in model.EntityContainer.EntitySets())
      {
        Dictionary<string, List<KeyValuePair<IEdmTerm, IEdmExpression>>> dictionary;
        if (propertiesWithAnnotations.TryGetValue(entitySet.EntityType().Name, out dictionary))
        {
          foreach (IEdmProperty property in entitySet.EntityType().Properties())
          {
            List<KeyValuePair<IEdmTerm, IEdmExpression>> keyValuePairList;
            if (dictionary.TryGetValue(property.Name, out keyValuePairList))
            {
              foreach (KeyValuePair<IEdmTerm, IEdmExpression> keyValuePair in keyValuePairList)
                AnalyticsModelBuilder.SetVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) property, keyValuePair.Key, keyValuePair.Value);
            }
          }
        }
      }
    }

    private void AddDirectValueAnnotations(EdmModel model)
    {
      foreach (IEdmNavigationSource entitySet in model.EntityContainer.EntitySets())
      {
        foreach (IEdmProperty property in entitySet.EntityType().Properties())
        {
          foreach (EdmDirectValueAnnotationAttribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) model.GetPropertyInfo(property), typeof (EdmDirectValueAnnotationAttribute), true))
            customAttribute.SetAnnotation((IEdmModel) model, (IEdmElement) property);
        }
      }
    }

    private static void SetVocabularyAnnotation(
      EdmModel model,
      IEdmProperty property,
      IEdmTerm term,
      string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      AnalyticsModelBuilder.SetVocabularyAnnotation(model, (IEdmVocabularyAnnotatable) property, term, (IEdmExpression) new EdmStringConstant(value));
    }

    private static void SetVocabularyAnnotation(
      EdmModel model,
      IEdmVocabularyAnnotatable property,
      IEdmTerm term,
      IEdmExpression value)
    {
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(property, term, value);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(AnalyticsModelBuilder.ToSerializationLocation(property)));
      model.AddVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    private static EdmVocabularyAnnotationSerializationLocation ToSerializationLocation(
      IEdmVocabularyAnnotatable target)
    {
      return !(target is IEdmEntityContainer) ? EdmVocabularyAnnotationSerializationLocation.Inline : EdmVocabularyAnnotationSerializationLocation.OutOfLine;
    }

    internal void BuildEntitySet<TEntityType>(
      ODataConventionModelBuilder builder,
      int odataModelVersion,
      int dbServiceVersion,
      bool hasCustomFields,
      string enableFeatureFlag = null)
      where TEntityType : class
    {
      if (enableFeatureFlag != null && !this.IsFeatureEnabled(enableFeatureFlag))
        return;
      Type type = typeof (TEntityType);
      if (type.GetVersionedModifier<ODataHideAttribute>(odataModelVersion) != null || type.GetVersionedModifier<DatabaseHideAttribute>(dbServiceVersion) != null)
        return;
      string str = type.GetVersionedModifier<ODataRenameAttribute>(odataModelVersion)?.Name ?? this.GetEntitySetNameFromTable(type);
      EntitySetConfiguration<TEntityType> setConfiguration = builder.EntitySet<TEntityType>(str);
      List<BoundMethodDefinition> methodDefinitionList;
      if (AnalyticsModelBuilder.s_boundMethods.TryGetValue(type, out methodDefinitionList))
      {
        foreach (BoundMethodDefinition methodDefinition in methodDefinitionList)
        {
          if (methodDefinition.Min <= odataModelVersion && odataModelVersion <= methodDefinition.Max)
            methodDefinition.Generate<TEntityType>(setConfiguration.EntityType, builder);
        }
      }
      if (!hasCustomFields)
        return;
      this._dynamicEntitySets.Add(str, type);
    }

    internal virtual string GetEntitySetNameFromTable(Type type) => AnalyticsModelBuilder.s_clrTypeToEntitySetName[type];

    private bool IsFeatureEnabled(string feature)
    {
      bool flag;
      return this._featureFlags != null && this._featureFlags.TryGetValue(feature, out flag) & flag;
    }
  }
}
