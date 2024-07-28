// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceOutputBuilder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal sealed class ResourceOutputBuilder : IResourceOutputBuilder
  {
    private const string Area = "Commerce";
    private const string Layer = "ResourceOutputBuilder";

    public ResourceOutput CreateResourceOutput(
      AzureResourceAccount resourceAccount,
      IVssRequestContext requestContext,
      ResourceState state = ResourceState.Started)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      requestContext.TraceEnter(5106331, "Commerce", nameof (ResourceOutputBuilder), nameof (CreateResourceOutput));
      try
      {
        ArgumentUtility.CheckForNull<AzureResourceAccount>(resourceAccount, nameof (resourceAccount));
        string accountName = (string) null;
        Guid serviceHostId = Guid.Empty;
        Uri accountUrl = (Uri) null;
        string identityDomain = (string) null;
        CommerceIntrinsicSettings o = new CommerceIntrinsicSettings();
        string label = (string) null;
        string tfsRegion = (string) null;
        ExtendedUsageMeterCollection meters;
        if (resourceAccount.OperationResult == OperationResult.Succeeded)
        {
          try
          {
            IVssRequestContext elevatedRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
            requestContext.Trace(5106332, TraceLevel.Verbose, "Commerce", nameof (ResourceOutputBuilder), "Account service GetAccount call enter.");
            CollectionHelper.WithCollectionContext(elevatedRequestContext, resourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
            {
              serviceHostId = collectionContext.ServiceHost.InstanceId;
              Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
              string regionCode = collection != null ? collection.PreferredRegion : throw new Exception(HostingResources.AccountDoesNotExist((object) resourceAccount.AccountId));
              tfsRegion = requestContext.GetExtension<ICommerceRegionHandler>().GetRegionDisplayName(elevatedRequestContext, regionCode);
              accountName = collection.Name;
              requestContext.Trace(5106333, TraceLevel.Verbose, "Commerce", nameof (ResourceOutputBuilder), "Account service GetAccount call leave.");
              Guid organizationAadTenantId = collectionContext.GetOrganizationAadTenantId();
              identityDomain = organizationAadTenantId == Guid.Empty ? "Windows Live ID" : organizationAadTenantId.ToString();
              try
              {
                accountUrl = requestContext.GetService<IUrlHostResolutionService>().GetHostUri(requestContext, collectionContext.ServiceHost.CollectionServiceHost.InstanceId, ServiceInstanceTypes.TFS);
              }
              catch (Exception ex) when (ex is InvalidOperationException || ex is ServiceOwnerNotFoundException)
              {
                collectionContext.TraceException(5106611, "Commerce", nameof (ResourceOutputBuilder), ex);
              }
            }), method: nameof (CreateResourceOutput));
          }
          catch (Exception ex)
          {
            requestContext.Trace(5106339, TraceLevel.Error, "Commerce", nameof (ResourceOutputBuilder), string.Format("Exception occurred getting account url or account name, falling back to simple output. Exception details \"{0}\"", (object) ex));
            return this.GetDefaultResourceOutput(resourceAccount, state);
          }
          try
          {
            meters = CommerceUtil.GetUsageMeters(requestContext, serviceHostId);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(5106339, "Commerce", nameof (ResourceOutputBuilder), ex);
            meters = CommerceUtil.GetDefaultUsageMeters(requestContext);
          }
          o.Items.Add(new KeyValuePair("AdditionalMeterData", CommerceUtil.EncodeToBase64(meters.AdditionalMeterData)));
          o.Items.Add(new KeyValuePair("AccountURL", accountUrl != (Uri) null ? accountUrl.ToString() : string.Empty));
          o.Items.Add(new KeyValuePair("DisplayName", accountName));
          o.Items.Add(new KeyValuePair("IdentityDomain", identityDomain));
          o.Items.Add(new KeyValuePair("TfsRegion", tfsRegion ?? resourceAccount.AzureGeoRegion));
          label = accountName;
        }
        else
          meters = CommerceUtil.GetDefaultUsageMeters(requestContext);
        XmlNode intrinsicSettings = (XmlNode) new XmlDocument();
        using (XmlWriter xmlWriter = intrinsicSettings.CreateNavigator().AppendChild())
          new XmlSerializer(typeof (CommerceIntrinsicSettings)).Serialize(xmlWriter, (object) o);
        UsageMeterCollection usageMeters = meters != null ? new UsageMeterCollection((IEnumerable<UsageMeter>) meters) : new UsageMeterCollection();
        return this.GetResourceOutput(resourceAccount, state, label, intrinsicSettings, usageMeters);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106339, "Commerce", nameof (ResourceOutputBuilder), ex);
        return this.GetDefaultResourceOutput(resourceAccount, state);
      }
      finally
      {
        requestContext.TraceLeave(5106340, "Commerce", nameof (ResourceOutputBuilder), nameof (CreateResourceOutput));
      }
    }

    public ResourceOutput GetDefaultResourceOutput(
      AzureResourceAccount azureResourceAccount,
      ResourceState state = ResourceState.Started)
    {
      return new ResourceOutput()
      {
        CloudServiceSettings = new CloudServiceSettings()
        {
          GeoRegion = azureResourceAccount.AzureGeoRegion ?? "West US"
        },
        ETag = azureResourceAccount.ETag,
        IntrinsicSettings = new XmlNode[0],
        Name = azureResourceAccount.AzureResourceName,
        OperationStatus = new OperationStatus()
        {
          Result = azureResourceAccount.OperationResult
        },
        OutputItems = new OutputItemList(),
        Plan = "None",
        SchemaVersion = "1.0",
        State = state.ToString(),
        SubState = azureResourceAccount.OperationResult.ToString(),
        Type = "account",
        UsageMeters = new UsageMeterCollection()
      };
    }

    public ResourceOutput GetResourceOutput(
      AzureResourceAccount azureResourceAccount,
      ResourceState state,
      string label,
      XmlNode intrinsicSettings,
      UsageMeterCollection usageMeters)
    {
      return new ResourceOutput()
      {
        CloudServiceSettings = new CloudServiceSettings()
        {
          GeoRegion = azureResourceAccount.AzureGeoRegion ?? "West US"
        },
        ETag = azureResourceAccount.ETag,
        IntrinsicSettings = new XmlNode[1]
        {
          intrinsicSettings
        },
        Name = azureResourceAccount.AzureResourceName,
        OperationStatus = new OperationStatus()
        {
          Result = azureResourceAccount.OperationResult
        },
        OutputItems = new OutputItemList(),
        Plan = "None",
        SchemaVersion = "1.0",
        State = state.ToString(),
        SubState = azureResourceAccount.OperationResult.ToString(),
        Type = "account",
        UsageMeters = usageMeters,
        Label = label
      };
    }
  }
}
