// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CustomerStageService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Commerce;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class CustomerStageService : ICustomerStageService, IVssFrameworkService
  {
    private static readonly IEnumerable<string> s_customerStagePropertyList = (IEnumerable<string>) new string[1]
    {
      "SystemProperty.CustomerStage"
    };
    private readonly IFeatureAvailabilitySecurityManager m_securityManager;
    private const string c_featureAvailabilityCustomerStageCachePath = "/FeatureAvailability/CustomerStageCache";
    [EditorBrowsable(EditorBrowsableState.Never)]
    private const string s_Area = "FeatureAvailabilityService";
    [EditorBrowsable(EditorBrowsableState.Never)]
    private const string s_Layer = "CustomerStageService";

    public CustomerStageService()
      : this((IFeatureAvailabilitySecurityManager) new FeatureAvailabilitySecurityManager())
    {
    }

    public CustomerStageService(
      IFeatureAvailabilitySecurityManager securityManager)
    {
      this.m_securityManager = securityManager;
    }

    public string GetCustomerStage(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      if (requestContext.IsInfrastructureHost())
        return CustomerStageService.StoredCustomerStage.NotSet.Stage;
      object customerStage;
      if (requestContext.Items.TryGetValue(RequestContextItemsKeys.CustomerStage, out customerStage))
        return (string) customerStage;
      CustomerStageService.StoredCustomerStage stageFromRegistry = CustomerStageService.GetCustomerStageFromRegistry(requestContext);
      requestContext.Items[RequestContextItemsKeys.CustomerStage] = (object) stageFromRegistry.Stage;
      if (requestContext.ServiceHost.Status == TeamFoundationServiceHostStatus.Started)
      {
        CustomerStageService.StoredCustomerStage collectionService = CustomerStageService.GetCustomerStageFromCollectionService(requestContext);
        if (collectionService.Sequence > stageFromRegistry.Sequence)
          CustomerStageService.SetCustomerStageInRegistry(requestContext, collectionService);
      }
      return stageFromRegistry.Stage;
    }

    public void SetCustomerStage(IVssRequestContext requestContext, string stage, int sequence)
    {
      requestContext.CheckProjectCollectionRequestContext();
      if (requestContext.IsInfrastructureHost())
        throw new InvalidOperationException(FrameworkResources.CannotSetEarlyAdopterStageOnInfrastructureHost());
      this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.EditFeatureFlags, false);
      CustomerStageService.StoredCustomerStage storedCustomerStage = new CustomerStageService.StoredCustomerStage(stage, sequence);
      CustomerStageService.StoredCustomerStage stageFromRegistry = CustomerStageService.GetCustomerStageFromRegistry(requestContext);
      CustomerStageService.StoredCustomerStage collectionService = CustomerStageService.GetCustomerStageFromCollectionService(requestContext);
      if (storedCustomerStage.Sequence <= stageFromRegistry.Sequence || storedCustomerStage.Sequence <= collectionService.Sequence)
        throw new CustomerStageSequenceOutOfDateException(storedCustomerStage.ValueString, stageFromRegistry.ValueString, collectionService.ValueString);
      ICollectionService service = requestContext.GetService<ICollectionService>();
      PropertyBag propertyBag = new PropertyBag();
      propertyBag.Add("SystemProperty.CustomerStage", (object) storedCustomerStage.ValueString);
      IVssRequestContext context = requestContext;
      PropertyBag properties = propertyBag;
      service.UpdateProperties(context, properties);
    }

    private static void SetCustomerStageInRegistry(
      IVssRequestContext requestContext,
      CustomerStageService.StoredCustomerStage value)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<IVssRegistryService>().SetValue<string>(vssRequestContext, "/FeatureAvailability/CustomerStageCache", value.ValueString);
    }

    private static CustomerStageService.StoredCustomerStage GetCustomerStageFromCollectionService(
      IVssRequestContext requestContext)
    {
      try
      {
        ICollectionService service = requestContext.GetService<ICollectionService>();
        if (service == null)
        {
          requestContext.Trace(1010102, TraceLevel.Error, "FeatureAvailabilityService", nameof (CustomerStageService), "collectionService was null in GetCustomerStageFromCollectionService");
          return CustomerStageService.StoredCustomerStage.NotSet;
        }
        Collection collection = service.GetCollection(requestContext, CustomerStageService.s_customerStagePropertyList);
        if (collection != null)
        {
          string str;
          return collection.Properties.TryGetValue<string>("SystemProperty.CustomerStage", out str) ? CustomerStageService.ParseStoredCustomerStage(requestContext, str) : CustomerStageService.StoredCustomerStage.NotSet;
        }
        requestContext.Trace(1010102, TraceLevel.Error, "FeatureAvailabilityService", nameof (CustomerStageService), "collection was null in GetCustomerStageFromCollectionService");
        return CustomerStageService.StoredCustomerStage.NotSet;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1010101, "FeatureAvailabilityService", nameof (CustomerStageService), ex);
        return CustomerStageService.StoredCustomerStage.NotSet;
      }
    }

    private static CustomerStageService.StoredCustomerStage GetCustomerStageFromRegistry(
      IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/FeatureAvailability/CustomerStageCache", false, (string) null);
      return CustomerStageService.ParseStoredCustomerStage(requestContext, str);
    }

    private static CustomerStageService.StoredCustomerStage ParseStoredCustomerStage(
      IVssRequestContext requestContext,
      string value)
    {
      CustomerStageService.StoredCustomerStage storedCustomerStage;
      if (!CustomerStageService.StoredCustomerStage.TryParse(value, out storedCustomerStage))
        requestContext.TraceException(1010100, "FeatureAvailabilityService", nameof (CustomerStageService), (Exception) new CustomerStageService.InvalidStoredCustomerStageStringException(value));
      return storedCustomerStage;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private class InvalidStoredCustomerStageStringException : Exception
    {
      public InvalidStoredCustomerStageStringException(string message)
        : base(message)
      {
      }
    }

    [DebuggerDisplay("{ValueString}")]
    private struct StoredCustomerStage
    {
      public static CustomerStageService.StoredCustomerStage NotSet { get; } = new CustomerStageService.StoredCustomerStage((string) null, 0);

      public string Stage { get; }

      public int Sequence { get; }

      public StoredCustomerStage(string stage, int sequence)
      {
        this.Stage = stage;
        this.Sequence = sequence;
      }

      public string ValueString => this.Sequence.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "|" + this.Stage;

      public static bool TryParse(
        string value,
        out CustomerStageService.StoredCustomerStage storedCustomerStage)
      {
        storedCustomerStage = CustomerStageService.StoredCustomerStage.NotSet;
        if (string.IsNullOrWhiteSpace(value))
          return true;
        string[] strArray = value.Split(new char[1]{ '|' }, 2, StringSplitOptions.None);
        int result;
        if (strArray.Length != 2 || !int.TryParse(strArray[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) || result < 1)
          return false;
        string stage = string.IsNullOrWhiteSpace(strArray[1]) ? (string) null : strArray[1];
        storedCustomerStage = new CustomerStageService.StoredCustomerStage(stage, result);
        return true;
      }
    }
  }
}
