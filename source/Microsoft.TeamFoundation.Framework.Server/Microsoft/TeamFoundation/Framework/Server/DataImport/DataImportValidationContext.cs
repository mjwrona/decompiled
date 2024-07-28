// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.DataImportValidationContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public class DataImportValidationContext
  {
    private const string c_area = "DataImport";
    private const string c_layer = "DataImportValidationContext";

    public ITFLogger Logger { get; private set; }

    public IImportInteractiveProvider Console { get; private set; }

    public IDictionary<string, object> Items { get; } = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public IVssRequestContext RequestContext { get; set; }

    public bool RequireNetwork { get; set; } = true;

    public event EventHandler<DataImportActivityCompletedEventArgs> ValidationProgressChanged;

    public bool IsServiceBeingIncluded(Guid serviceInstanceTypeId)
    {
      HashSet<Guid> serviceInstanceTypeId1 = this.IncludedServiceInstanceTypeId;
      // ISSUE: explicit non-virtual call
      return serviceInstanceTypeId1 != null && __nonvirtual (serviceInstanceTypeId1.Contains(serviceInstanceTypeId));
    }

    public bool IsServiceBeingExcluded(Guid serviceInstanceTypeId)
    {
      HashSet<Guid> serviceInstanceTypeId1 = this.ExcludedServiceInstanceTypeId;
      // ISSUE: explicit non-virtual call
      return serviceInstanceTypeId1 != null && __nonvirtual (serviceInstanceTypeId1.Contains(serviceInstanceTypeId));
    }

    public DataImportValidationContext(
      bool requireNetwork,
      ITFLogger logger,
      IVssRequestContext requestContext,
      IImportInteractiveProvider interactiveProvider)
      : this(logger)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.RequestContext = requestContext;
      this.Logger = logger;
      this.Console = interactiveProvider;
      this.RequireNetwork = requireNetwork;
    }

    public DataImportValidationContext(ITFLogger logger) => this.Logger = logger;

    public T CreateComponent<T>() where T : class, ISqlResourceComponent, new() => (this.RequestContext ?? throw new ApplicationException("The request context need to be set in order to create a component.")).CreateComponent<T>();

    public object this[string name]
    {
      get => this.Items[name];
      set => this.Items[name] = value;
    }

    public T GetItem<T>(string itemName, T defaultValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(itemName, nameof (itemName));
      object obj;
      if (!this.Items.TryGetValue(itemName, out obj))
        return defaultValue;
      try
      {
        if (obj == null)
          return (T) obj;
        return obj is string str ? RegistryUtility.FromString<T>(str) : (T) obj;
      }
      catch (Exception ex)
      {
        string message = string.Format("While looking up Item {0} encounter an exception while attempting to cast {1} ({2}) to {3}. {4}", (object) itemName, obj, (object) obj?.GetType().FullName, (object) typeof (T).FullName, (object) ex);
        IVssRequestContext requestContext = this.RequestContext;
        if (requestContext != null)
          requestContext.Trace(15080817, TraceLevel.Info, "DataImport", nameof (DataImportValidationContext), message);
        this.Logger?.Warning(message);
        return defaultValue;
      }
    }

    public void UpdateItem(string itemName, object value) => this.Items[itemName] = value;

    public void SetLogger(ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      this.Logger = logger;
    }

    public virtual void OnValidationProgressChanged(DataImportActivityCompletedEventArgs e)
    {
      EventHandler<DataImportActivityCompletedEventArgs> validationProgressChanged = this.ValidationProgressChanged;
      if (validationProgressChanged == null)
        return;
      validationProgressChanged((object) this, e);
    }

    public HashSet<Guid> IncludedServiceInstanceTypeId { get; set; }

    public HashSet<Guid> ExcludedServiceInstanceTypeId { get; set; }
  }
}
