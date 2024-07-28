// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsContextTypesCacheService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsContextTypesCacheService : 
    VssVersionedMemoryCacheService<AnalyticsContextTypesCacheService.ODataModelKey, MetadataModel>
  {
    public static readonly Guid ProcessChangedNotificationId = new Guid("88DA0908-358C-4A6C-B431-01072D2FDA9D");
    internal static readonly Guid ServiceVersionChangedId = new Guid("8CBE5B20-9A74-42F2-A454-972D7996A62D");
    private static readonly char[] _splitChars = new char[1]
    {
      ','
    };
    private static readonly MemoryCacheConfiguration<AnalyticsContextTypesCacheService.ODataModelKey, MetadataModel> s_configuration = new MemoryCacheConfiguration<AnalyticsContextTypesCacheService.ODataModelKey, MetadataModel>().WithInactivityInterval(TimeSpan.FromMinutes(15.0)).WithCleanupInterval(TimeSpan.FromMinutes(15.0));
    private static readonly string[] _modelBuilderFeatureFlags = new string[7]
    {
      "Analytics.Model.ShowBuildPipelineEntitiesFeatureFlag",
      "Analytics.Model.ShowDummyDims",
      "Analytics.Model.GitHubInsightsCode",
      "Analytics.Model.ManualTestEntities",
      "Analytics.Model.ShowReleaseManagementIterationOneEntities",
      "Analytics.Transform.LeadTimeCommitToDeployment",
      "Analytics.Transform.WorkItemTagsPredict"
    };

    public AnalyticsContextTypesCacheService()
      : base((IEqualityComparer<AnalyticsContextTypesCacheService.ODataModelKey>) EqualityComparer<AnalyticsContextTypesCacheService.ODataModelKey>.Default, AnalyticsContextTypesCacheService.s_configuration)
    {
    }

    public virtual MetadataModel GetModel(IVssRequestContext requestContext, Guid? processId)
    {
      AnalyticsContextTypesCacheService.ODataModelKey odataModelKey = new AnalyticsContextTypesCacheService.ODataModelKey(processId, requestContext.GetODataModelVersion());
      MetadataModel metadataModel = (MetadataModel) null;
      return this.TryGetValue(requestContext, odataModelKey, out metadataModel) ? metadataModel : this.UpdateModel(requestContext, odataModelKey);
    }

    private MetadataModel UpdateModel(
      IVssRequestContext requestContext,
      AnalyticsContextTypesCacheService.ODataModelKey contextKey)
    {
      MetadataModel model = this.CreateModel(requestContext, contextKey);
      using (IVssVersionedCacheContext<AnalyticsContextTypesCacheService.ODataModelKey, MetadataModel> versionedContext = this.CreateVersionedContext(requestContext))
      {
        int num = (int) versionedContext.TryUpdate(requestContext, contextKey, model);
      }
      return model;
    }

    private MetadataModel CreateModel(
      IVssRequestContext requestContext,
      AnalyticsContextTypesCacheService.ODataModelKey contextKey)
    {
      MetadataModel model = (MetadataModel) null;
      requestContext.TraceEnter(12011005, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (CreateModel));
      try
      {
        ModelCreationProcessFields modelCreationParameter = (ModelCreationProcessFields) null;
        int dbServiceVersion = 0;
        using (AnalyticsMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<AnalyticsMetadataComponent>("Analytics"))
        {
          modelCreationParameter = replicaAwareComponent.GetModelCreationProcessFields(contextKey.ProcessId);
          dbServiceVersion = replicaAwareComponent.GetVersion();
        }
        requestContext.Trace(12011002, TraceLevel.Verbose, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), string.Format("Cache miss for process id '{0}', odata version '{1}'. Creating context with {2} fields.", (object) contextKey.ProcessId, (object) contextKey.ODataModelVersion, (object) modelCreationParameter.CustomProcessFields.Count));
        AnalyticsContextTypesCacheService.MetadataModelKey metadataModelKey = new AnalyticsContextTypesCacheService.MetadataModelKey(modelCreationParameter, dbServiceVersion, contextKey.ODataModelVersion);
        if (requestContext.IsFeatureEnabled("Analytics.Model.CacheCommonModels"))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          AnalyticsContextTypesCacheService.CommonMetadataModelCache service = vssRequestContext.GetService<AnalyticsContextTypesCacheService.CommonMetadataModelCache>();
          if (!service.TryGetValue(vssRequestContext, metadataModelKey, out model))
          {
            model = AnalyticsContextTypesCacheService.CreateModel(requestContext, metadataModelKey);
            service.TryAdd(vssRequestContext, metadataModelKey, model);
          }
        }
        else
          model = AnalyticsContextTypesCacheService.CreateModel(requestContext, metadataModelKey);
      }
      finally
      {
        requestContext.TraceLeave(12011006, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (CreateModel));
      }
      return model;
    }

    internal static MetadataModel CreateModel(
      IVssRequestContext requestContext,
      AnalyticsContextTypesCacheService.MetadataModelKey modelKey)
    {
      AnalyticsContextTypesCacheService.ContextType contextType = AnalyticsContextTypesCacheService.GetContextType(requestContext, modelKey.DatabaseServiceVersion);
      return AnalyticsContextTypesCacheService.CreateModel(requestContext, contextType, modelKey.ModelCreationParameter, modelKey.ODataModelVersion);
    }

    private static AnalyticsContextTypesCacheService.ContextType GetContextType(
      IVssRequestContext requestContext,
      int dbServiceVersion)
    {
      requestContext.TraceEnter(12011013, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (GetContextType));
      try
      {
        AnalyticsContextTypesCacheService.EFModelVersionCacheService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<AnalyticsContextTypesCacheService.EFModelVersionCacheService>();
        AnalyticsContextTypesCacheService.ContextType contextType = service.Get(dbServiceVersion);
        if (contextType == null)
        {
          try
          {
            requestContext.TraceEnter(12011015, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), "CreateEfModel");
            contextType = new AnalyticsContextTypesCacheService.ContextType()
            {
              Type = typeof (AnalyticsContext),
              EfModel = AnalyticsContext.CreateModel(dbServiceVersion),
              Version = dbServiceVersion
            };
            VssCacheExpiryProvider<int, AnalyticsContextTypesCacheService.ContextType> expiryProvider = new VssCacheExpiryProvider<int, AnalyticsContextTypesCacheService.ContextType>(Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry), dbServiceVersion == AnalyticsComponent.MaxVersion ? Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry) : Capture.Create<TimeSpan>(TimeSpan.FromMinutes(60.0)));
            service.Set(dbServiceVersion, contextType, expiryProvider);
          }
          finally
          {
            requestContext.TraceLeave(12011016, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), "CreateEfModel");
          }
        }
        return contextType;
      }
      finally
      {
        requestContext.TraceLeave(12011014, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (GetContextType));
      }
    }

    private static MetadataModel CreateModel(
      IVssRequestContext requestContext,
      AnalyticsContextTypesCacheService.ContextType contextType,
      ModelCreationProcessFields modelCreationParameter,
      int odataModelVersion)
    {
      IEdmModel edmModel = AnalyticsContextTypesCacheService.CreateEdmModel(requestContext, contextType.Type, modelCreationParameter, contextType.Version, odataModelVersion);
      return new MetadataModel(contextType.Type, contextType.Version, edmModel, contextType.EfModel, modelCreationParameter != null ? modelCreationParameter.Level : 0);
    }

    private static IEdmModel CreateEdmModel(
      IVssRequestContext requestContext,
      Type type,
      ModelCreationProcessFields modelCreationParameter,
      int databaseServiceVersion,
      int odataModelVersion)
    {
      requestContext.TraceEnter(12011011, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (CreateEdmModel));
      try
      {
        Dictionary<string, bool> dictionary = ((IEnumerable<string>) AnalyticsContextTypesCacheService._modelBuilderFeatureFlags).ToDictionary<string, string, bool>((Func<string, string>) (f => f), (Func<string, bool>) (f => requestContext.IsFeatureEnabled(f)));
        IEdmModel edmModel = new AnalyticsModelBuilder(AnalyticsContextTypesCacheService.GetHttpConfiguration(requestContext), (IDictionary<string, bool>) dictionary).GetEdmModel(modelCreationParameter, type, databaseServiceVersion, odataModelVersion);
        AnalyticsContextTypesCacheService.TraceMetadataXmlHash(requestContext, edmModel);
        return edmModel;
      }
      finally
      {
        requestContext.TraceLeave(12011012, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (CreateEdmModel));
      }
    }

    private static HttpConfiguration GetHttpConfiguration(IVssRequestContext requestContext)
    {
      try
      {
        return WebApiConfiguration.GetHttpConfiguration(requestContext);
      }
      catch (Exception ex)
      {
        return (HttpConfiguration) null;
      }
    }

    private static void TraceMetadataXmlHash(IVssRequestContext requestContext, IEdmModel model)
    {
      if (!requestContext.IsTracing(12013023, TraceLevel.Verbose, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService)))
        return;
      string csdl = (string) null;
      IEnumerable<EdmError> errors = (IEnumerable<EdmError>) null;
      if (model.TryGetMetdataXml(out csdl, out errors))
        requestContext.Trace(12013023, TraceLevel.Verbose, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), string.Format("{{\"Hash\": \"{0}\"}}", (object) csdl.GetHashCode()));
      else
        requestContext.TraceAlways(12013024, TraceLevel.Error, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), string.Join<EdmError>(Environment.NewLine, errors));
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      ITeamFoundationSqlNotificationService service1 = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service1.RegisterNotification(systemRequestContext, "Default", AnalyticsContextTypesCacheService.ProcessChangedNotificationId, new SqlNotificationHandler(this.OnProcessChanged), false);
      service1.RegisterNotification(systemRequestContext, "Default", AnalyticsContextTypesCacheService.ServiceVersionChangedId, new SqlNotificationHandler(this.OnServiceVersionChanged), false);
      IVssRegistryService service2 = systemRequestContext.GetService<IVssRegistryService>();
      foreach (string builderFeatureFlag in AnalyticsContextTypesCacheService._modelBuilderFeatureFlags)
        service2.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), true, "/FeatureAvailability/Entries/" + builderFeatureFlag + "/*");
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.Clear(requestContext);
    }

    private void OnServiceVersionChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      if (args.Data == null || !args.Data.Contains("\"Analytics\""))
        return;
      this.Clear(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", AnalyticsContextTypesCacheService.ProcessChangedNotificationId, new SqlNotificationHandler(this.OnProcessChanged), false);
      service.UnregisterNotification(systemRequestContext, "Default", AnalyticsContextTypesCacheService.ServiceVersionChangedId, new SqlNotificationHandler(this.OnServiceVersionChanged), false);
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
      base.ServiceEnd(systemRequestContext);
    }

    private void OnProcessChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      requestContext.TraceEnter(12011007, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (OnProcessChanged));
      try
      {
        if (string.IsNullOrEmpty(args.Data))
        {
          this.Clear(requestContext);
        }
        else
        {
          foreach (string input in args.Data.Split(AnalyticsContextTypesCacheService._splitChars, StringSplitOptions.RemoveEmptyEntries))
          {
            Guid result;
            if (Guid.TryParse(input, out result))
              this.InvalidateProcess(requestContext, new Guid?(result));
          }
          this.InvalidateProcess(requestContext, new Guid?());
        }
      }
      finally
      {
        requestContext.TraceLeave(12011008, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (OnProcessChanged));
      }
    }

    private void InvalidateProcess(IVssRequestContext requestContext, Guid? processId)
    {
      requestContext.TraceEnter(12011009, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (InvalidateProcess));
      try
      {
        for (int odataModelVersion = Math.Min(ModelVersionExtension.GetDefaultVersion(), ModelVersionExtension.GetMinODataModelVersion()); odataModelVersion <= ModelVersionExtension.GetMaxODataModelVersion(); ++odataModelVersion)
        {
          AnalyticsContextTypesCacheService.ODataModelKey odataModelKey = new AnalyticsContextTypesCacheService.ODataModelKey(processId, odataModelVersion);
          MetadataModel metadataModel = (MetadataModel) null;
          if (this.TryGetValue(requestContext, odataModelKey, out metadataModel))
            this.UpdateModel(requestContext, odataModelKey);
        }
      }
      finally
      {
        requestContext.TraceLeave(12011010, "AnalyticsModel", nameof (AnalyticsContextTypesCacheService), nameof (InvalidateProcess));
      }
    }

    internal void WarmUp(IVssRequestContext systemRequestContext) => AnalyticsContextTypesCacheService.GetContextType(systemRequestContext, AnalyticsComponent.MaxVersion);

    internal class EFModelVersionCacheService : 
      VssMemoryCacheService<int, AnalyticsContextTypesCacheService.ContextType>
    {
      private static readonly TimeSpan cacheCleanupInterval = TimeSpan.FromMinutes(10.0);

      protected override void ServiceStart(IVssRequestContext systemRequestContext)
      {
        systemRequestContext.CheckDeploymentRequestContext();
        base.ServiceStart(systemRequestContext);
      }

      public EFModelVersionCacheService()
        : base(AnalyticsContextTypesCacheService.EFModelVersionCacheService.cacheCleanupInterval)
      {
      }

      public virtual void Set(
        int dbServiceVersion,
        AnalyticsContextTypesCacheService.ContextType contextType,
        VssCacheExpiryProvider<int, AnalyticsContextTypesCacheService.ContextType> expiryProvider)
      {
        this.MemoryCache.Add(dbServiceVersion, contextType, true, expiryProvider);
      }

      public virtual AnalyticsContextTypesCacheService.ContextType Get(int dbServiceVersion)
      {
        AnalyticsContextTypesCacheService.ContextType contextType;
        return this.MemoryCache.TryGetValue(dbServiceVersion, out contextType) ? contextType : (AnalyticsContextTypesCacheService.ContextType) null;
      }
    }

    public static class TraceConstants
    {
      public const string Area = "AnalyticsModel";
      public const string Layer = "AnalyticsContextTypesCacheService";
    }

    internal class CommonMetadataModelCache : 
      VssMemoryCacheService<AnalyticsContextTypesCacheService.MetadataModelKey, MetadataModel>
    {
      private static readonly MemoryCacheConfiguration<AnalyticsContextTypesCacheService.MetadataModelKey, MetadataModel> s_configuration = new MemoryCacheConfiguration<AnalyticsContextTypesCacheService.MetadataModelKey, MetadataModel>().WithInactivityInterval(TimeSpan.FromMinutes(60.0)).WithCleanupInterval(TimeSpan.FromMinutes(15.0));

      public CommonMetadataModelCache()
        : base((IEqualityComparer<AnalyticsContextTypesCacheService.MetadataModelKey>) EqualityComparer<AnalyticsContextTypesCacheService.MetadataModelKey>.Default, AnalyticsContextTypesCacheService.CommonMetadataModelCache.s_configuration)
      {
      }

      protected override void ServiceStart(IVssRequestContext systemRequestContext)
      {
        systemRequestContext.CheckDeploymentRequestContext();
        base.ServiceStart(systemRequestContext);
      }
    }

    internal class MetadataModelKey
    {
      public int DatabaseServiceVersion { get; private set; }

      public int ODataModelVersion { get; private set; }

      public ModelCreationProcessFields ModelCreationParameter { get; private set; }

      public MetadataModelKey(
        ModelCreationProcessFields modelCreationParameter,
        int dbServiceVersion,
        int odataModelVersion)
      {
        this.DatabaseServiceVersion = dbServiceVersion;
        this.ODataModelVersion = odataModelVersion;
        this.ModelCreationParameter = modelCreationParameter;
      }

      public override bool Equals(object obj) => obj is AnalyticsContextTypesCacheService.MetadataModelKey metadataModelKey && this.DatabaseServiceVersion == metadataModelKey.DatabaseServiceVersion && this.ODataModelVersion == metadataModelKey.ODataModelVersion && this.ModelCreationParameter.Equals((object) metadataModelKey.ModelCreationParameter);

      public override int GetHashCode() => (this.DatabaseServiceVersion, this.ODataModelVersion, this.ModelCreationParameter).GetHashCode();
    }

    internal class ContextType
    {
      public Type Type { get; set; }

      public DbCompiledModel EfModel { get; set; }

      public int Version { get; set; }
    }

    public class ODataModelKey
    {
      public Guid? ProcessId { get; }

      public Guid SafeProcessId => this.ProcessId ?? Guid.Empty;

      public int ODataModelVersion { get; }

      public ODataModelKey(Guid? processId, int odataModelVersion)
      {
        this.ProcessId = processId;
        this.ODataModelVersion = odataModelVersion;
      }

      public override int GetHashCode() => this.SafeProcessId.GetHashCode() ^ this.ODataModelVersion.GetHashCode();

      public override bool Equals(object obj)
      {
        if (obj == null || this.GetType() != obj.GetType())
          return false;
        AnalyticsContextTypesCacheService.ODataModelKey odataModelKey = (AnalyticsContextTypesCacheService.ODataModelKey) obj;
        return this.SafeProcessId == odataModelKey.SafeProcessId && this.ODataModelVersion == odataModelKey.ODataModelVersion;
      }
    }
  }
}
