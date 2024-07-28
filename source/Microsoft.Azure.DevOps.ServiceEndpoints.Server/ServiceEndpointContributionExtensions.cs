// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointContributionExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class ServiceEndpointContributionExtensions
  {
    private const string AuthenticationParameter = "AuthenticationParameter";

    public static ServiceEndpointType ToServiceEndpointType(
      this Contribution contribution,
      IVssRequestContext requestContext,
      IEnumerable<Contribution> authContributions)
    {
      JObject properties = contribution.Properties;
      ServiceEndpointType serviceEndpointType = new ServiceEndpointType()
      {
        Name = ServiceEndpointContributionExtensions.GetRequiredValue<string>(properties, "name"),
        DisplayName = ServiceEndpointContributionExtensions.GetRequiredValue<string>(properties, "displayName"),
        Description = contribution.Description
      };
      try
      {
        serviceEndpointType.EndpointUrl = ServiceEndpointContributionExtensions.GetEndpointUrl(properties);
      }
      catch (UriFormatException ex)
      {
        requestContext.Trace(34000822, TraceLevel.Info, "ServiceEndpoints", "ServiceEndpoints", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found an invalid endpoint url from endpoint type definition: {0}", (object) properties?.ToString()));
        serviceEndpointType.EndpointUrl = (EndpointUrl) null;
      }
      JArray jarray1;
      if (properties.TryGetValue<JArray>("dataSources", out jarray1))
      {
        foreach (JObject jo in jarray1)
          serviceEndpointType.DataSources.Add(ServiceEndpointContributionExtensions.ToDataSource(jo));
      }
      JArray jarray2;
      if (properties.TryGetValue<JArray>("dependencyData", out jarray2))
      {
        foreach (JObject jo in jarray2)
          serviceEndpointType.DependencyData.Add(ServiceEndpointContributionExtensions.ToDependencyData(jo));
      }
      serviceEndpointType.InputDescriptors.AddRange((IEnumerable<InputDescriptor>) ServiceEndpointContributionExtensions.GetInputDescriptors(properties));
      JArray jarray3;
      if (properties.TryGetValue<JArray>("authenticationSchemes", out jarray3))
      {
        foreach (JObject jobject in jarray3)
        {
          string type = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jobject, "type");
          Contribution authContribution = authContributions.FirstOrDefault<Contribution>((Func<Contribution, bool>) (c => ((IEnumerable<string>) c.Id.Split('.')).Last<string>().Equals(((IEnumerable<string>) type.Split('.')).Last<string>())));
          if (authContribution != null)
          {
            string optionalValue = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jobject, "displayName", ServiceEndpointContributionExtensions.GetRequiredValue<string>(authContribution.Properties, "displayName"));
            ServiceEndpointAuthenticationScheme authenticationScheme = ServiceEndpointContributionExtensions.ToServiceEndpointAuthenticationScheme(requestContext, authContribution, jobject, optionalValue, serviceEndpointType.Name);
            if (authenticationScheme != null)
              serviceEndpointType.AuthenticationSchemes.Add(authenticationScheme);
          }
        }
      }
      JArray jarray4;
      if (properties.TryGetValue<JArray>("trustedHosts", out jarray4))
      {
        foreach (JValue jvalue in jarray4)
          serviceEndpointType.TrustedHosts.Add(jvalue.ToObject<string>());
      }
      serviceEndpointType.HelpLink = ServiceEndpointContributionExtensions.ToHelpLink(ServiceEndpointContributionExtensions.GetOptionalValue<JObject>(properties, "helpLink"));
      serviceEndpointType.HelpMarkDown = SafeHtmlWrapper.MakeSafe(ServiceEndpointContributionExtensions.GetOptionalValue<string>(properties, "helpMarkDown"));
      JToken property = contribution.Properties["useBuiltInIcon"];
      if (property == null || !(bool) property)
        serviceEndpointType.IconUrl = ServiceEndpointContributionExtensions.GetIconUrl(requestContext, contribution);
      string str;
      if (properties.TryGetValue<string>("uiContributionId", out str))
        serviceEndpointType.UiContributionId = str;
      return serviceEndpointType;
    }

    public static List<InputDescriptor> GetInputDescriptors(
      JObject properties,
      bool defaultConfidentiality = false)
    {
      List<InputDescriptor> inputDescriptors = new List<InputDescriptor>();
      JArray jarray;
      if (properties.TryGetValue<JArray>("inputDescriptors", out jarray))
      {
        foreach (JObject jo in jarray)
        {
          InputDescriptor inputInputDescriptor = ServiceEndpointContributionExtensions.ToFormInputInputDescriptor(jo, defaultConfidentiality);
          inputInputDescriptor.Description = SafeHtmlWrapper.MakeSafe(inputInputDescriptor.Description);
          inputDescriptors.Add(inputInputDescriptor);
        }
      }
      return inputDescriptors;
    }

    private static EndpointUrl GetEndpointUrl(JObject properties)
    {
      try
      {
        EndpointUrl optionalValue = ServiceEndpointContributionExtensions.GetOptionalValue<EndpointUrl>(properties, "url");
        if (optionalValue != null)
          optionalValue.HelpText = SafeHtmlWrapper.MakeSafe(optionalValue.HelpText);
        return optionalValue;
      }
      catch (JsonSerializationException ex)
      {
        Uri optionalValue = ServiceEndpointContributionExtensions.GetOptionalValue<Uri>(properties, "url");
        return new EndpointUrl() { Value = optionalValue };
      }
    }

    private static ServiceEndpointAuthenticationScheme ToServiceEndpointAuthenticationScheme(
      IVssRequestContext requestContext,
      Contribution authContribution,
      JObject properties,
      string authenticationSchemeName,
      string endpointType)
    {
      string featureName = ServiceEndpointContributionExtensions.GetOptionalValue<string>(authContribution.Properties, "featureFlag").IsNullOrEmpty<char>() ? ServiceEndpointContributionExtensions.GetOptionalValue<string>(properties, "featureFlag") : ServiceEndpointContributionExtensions.GetOptionalValue<string>(authContribution.Properties, "featureFlag");
      ITeamFoundationFeatureAvailabilityService service = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      if (featureName != null && !service.IsFeatureEnabled(requestContext, featureName))
        return (ServiceEndpointAuthenticationScheme) null;
      ServiceEndpointAuthenticationScheme authenticationScheme = new ServiceEndpointAuthenticationScheme();
      List<InputDescriptor> inputDescriptors1 = ServiceEndpointContributionExtensions.GetInputDescriptors(properties, true);
      List<AuthorizationHeader> authorizationHeaders1 = ServiceEndpointContributionExtensions.GetAuthorizationHeaders(properties);
      List<ClientCertificate> clientCertificates = ServiceEndpointContributionExtensions.GetClientCertificates(properties);
      List<DataSourceBinding> dataSourceBindings = ServiceEndpointContributionExtensions.GetDataSourceBindings(properties);
      authenticationScheme.Scheme = ServiceEndpointContributionExtensions.GetRequiredValue<string>(authContribution.Properties, "name");
      authenticationScheme.DisplayName = authenticationSchemeName;
      List<InputDescriptor> inputDescriptors2 = ServiceEndpointContributionExtensions.GetInputDescriptors(authContribution.Properties, true);
      List<AuthorizationHeader> authorizationHeaders2 = ServiceEndpointContributionExtensions.GetAuthorizationHeaders(authContribution.Properties);
      List<DataSourceBinding> authDataSourceBindings = ServiceEndpointContributionExtensions.GetDataSourceBindings(authContribution.Properties);
      List<InputDescriptor> second = inputDescriptors1;
      InputDescriptorComparer comparer = new InputDescriptorComparer();
      IEnumerable<InputDescriptor> collection1 = inputDescriptors2.Except<InputDescriptor>((IEnumerable<InputDescriptor>) second, (IEqualityComparer<InputDescriptor>) comparer);
      IEnumerable<AuthorizationHeader> collection2 = authorizationHeaders2.Except<AuthorizationHeader>((IEnumerable<AuthorizationHeader>) authorizationHeaders1, (IEqualityComparer<AuthorizationHeader>) new AuthorizationHeaderComparer());
      Predicate<DataSourceBinding> match = (Predicate<DataSourceBinding>) (dataSourceBinding => authDataSourceBindings.Exists((Predicate<DataSourceBinding>) (authDataSourceBinding => authDataSourceBinding.Target.Equals(dataSourceBinding.Target, StringComparison.OrdinalIgnoreCase))));
      List<DataSourceBinding> all = dataSourceBindings.FindAll(match);
      authenticationScheme.DataSourceBindings = all;
      inputDescriptors1.AddRange(collection1);
      authorizationHeaders1.AddRange(collection2);
      string str1;
      if (properties.TryGetValue<string>("authorizationUrl", out str1))
        authenticationScheme.AuthorizationUrl = str1;
      string str2;
      properties.TryGetValue<string>("requiresOAuth2Configuration", out str2);
      bool result;
      authenticationScheme.RequiresOAuth2Configuration = !bool.TryParse(str2, out result) || result;
      IDictionary<string, string> dictionary;
      properties.TryGetValue<IDictionary<string, string>>(nameof (properties), out dictionary);
      if (dictionary != null)
        authenticationScheme.Properties = dictionary;
      foreach (InputDescriptor inputDescriptor in inputDescriptors1)
        inputDescriptor.GroupName = "AuthenticationParameter";
      authenticationScheme.InputDescriptors.AddRange((IEnumerable<InputDescriptor>) inputDescriptors1);
      authenticationScheme.AuthorizationHeaders.AddRange((IEnumerable<AuthorizationHeader>) authorizationHeaders1);
      authenticationScheme.ClientCertificates.AddRange((IEnumerable<ClientCertificate>) clientCertificates);
      return authenticationScheme;
    }

    private static List<AuthorizationHeader> GetAuthorizationHeaders(JObject properties)
    {
      List<AuthorizationHeader> authorizationHeaders = new List<AuthorizationHeader>();
      JArray jarray;
      if (properties.TryGetValue<JArray>("headers", out jarray))
      {
        foreach (JObject jo in jarray)
          authorizationHeaders.Add(ServiceEndpointContributionExtensions.ToAuthorizationHeader(jo));
      }
      return authorizationHeaders;
    }

    private static List<DataSourceBinding> GetDataSourceBindings(JObject properties)
    {
      List<DataSourceBinding> dataSourceBindings = new List<DataSourceBinding>();
      JArray jarray;
      if (properties.TryGetValue<JArray>("dataSourceBindings", out jarray))
      {
        foreach (JObject jo in jarray)
          dataSourceBindings.Add(ServiceEndpointContributionExtensions.ToDataSourceBinding(jo));
      }
      return dataSourceBindings;
    }

    private static List<ClientCertificate> GetClientCertificates(JObject properties)
    {
      List<ClientCertificate> clientCertificates = new List<ClientCertificate>();
      JArray jarray;
      if (properties.TryGetValue<JArray>("clientCertificates", out jarray))
      {
        foreach (JObject jo in jarray)
          clientCertificates.Add(ServiceEndpointContributionExtensions.ToClientCertificate(jo));
      }
      return clientCertificates;
    }

    private static HelpLink ToHelpLink(JObject jo)
    {
      if (jo == null)
        return (HelpLink) null;
      return new HelpLink()
      {
        Text = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "text"),
        Url = ServiceEndpointContributionExtensions.GetOptionalValue<Uri>(jo, "url")
      };
    }

    private static DataSource ToDataSource(JObject jo) => new DataSource()
    {
      Headers = ServiceEndpointContributionExtensions.GetAuthorizationHeaders(jo),
      Name = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "name"),
      EndpointUrl = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "endpointUrl"),
      ResourceUrl = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "resourceUrl", string.Empty),
      ResultSelector = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "resultSelector"),
      RequestVerb = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "requestVerb"),
      RequestContent = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "requestContent"),
      AuthenticationScheme = ServiceEndpointContributionExtensions.GetOptionalValue<AuthenticationSchemeReference>(jo, "authenticationScheme"),
      CallbackContextTemplate = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "callbackContextTemplate"),
      CallbackRequiredTemplate = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "callbackRequiredTemplate"),
      InitialContextTemplate = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "initialContextTemplate")
    };

    private static DependencyData ToDependencyData(JObject jo)
    {
      DependencyData dependencyData = new DependencyData();
      dependencyData.Input = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "input");
      JArray jarray1;
      if (jo.TryGetValue<JArray>("map", out jarray1))
      {
        List<KeyValuePair<string, List<KeyValuePair<string, string>>>> keyValuePairList1 = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
        foreach (JObject jobject in jarray1)
        {
          string requiredValue1 = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jobject, "key");
          List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
          JArray jarray2;
          if (jobject.TryGetValue<JArray>("value", out jarray2))
          {
            foreach (JObject jo1 in jarray2)
            {
              string requiredValue2 = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo1, "key");
              string optionalValue = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo1, "value");
              keyValuePairList2.Add(new KeyValuePair<string, string>(requiredValue2, optionalValue));
            }
          }
          keyValuePairList1.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(requiredValue1, keyValuePairList2));
        }
        dependencyData.Map = keyValuePairList1;
      }
      return dependencyData;
    }

    private static DataSourceBinding ToDataSourceBinding(JObject jo)
    {
      DataSourceBinding dataSourceBinding = new DataSourceBinding();
      dataSourceBinding.Target = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "target");
      dataSourceBinding.RequestVerb = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "requestVerb");
      dataSourceBinding.RequestContent = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "requestContent");
      dataSourceBinding.ResultSelector = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "resultSelector");
      dataSourceBinding.ResultTemplate = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "resultTemplate");
      dataSourceBinding.EndpointUrl = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "endpointUrl");
      dataSourceBinding.DataSourceName = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "dataSourceName");
      if (dataSourceBinding.RequestVerb.IsNullOrEmpty<char>())
        dataSourceBinding.RequestVerb = "GET";
      ServiceEndpointValidator.ValidateRequestParameters(dataSourceBinding.RequestVerb, dataSourceBinding.RequestContent);
      return dataSourceBinding;
    }

    private static AuthorizationHeader ToAuthorizationHeader(JObject jo) => new AuthorizationHeader()
    {
      Name = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "name"),
      Value = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "value")
    };

    private static ClientCertificate ToClientCertificate(JObject jo) => new ClientCertificate()
    {
      Value = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "value")
    };

    private static InputDescriptor ToFormInputInputDescriptor(
      JObject jo,
      bool defaultConfidentiality)
    {
      InputDescriptor inputInputDescriptor = new InputDescriptor();
      inputInputDescriptor.Id = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "id");
      inputInputDescriptor.Name = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo, "name");
      inputInputDescriptor.Description = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "description");
      inputInputDescriptor.InputMode = ServiceEndpointContributionExtensions.GetOptionalEnumValue<InputMode>(jo, "inputMode", InputMode.None);
      inputInputDescriptor.IsConfidential = ServiceEndpointContributionExtensions.GetOptionalValue<bool>(jo, "isConfidential", defaultConfidentiality);
      inputInputDescriptor.GroupName = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "groupName");
      inputInputDescriptor.ValueHint = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo, "valueHint");
      inputInputDescriptor.HasDynamicValueInformation = ServiceEndpointContributionExtensions.GetOptionalValue<bool>(jo, "hasDynamicValueInformation");
      inputInputDescriptor.Properties = ServiceEndpointContributionExtensions.GetOptionalValue<IDictionary<string, object>>(jo, "properties");
      JObject jo1;
      if (jo.TryGetValue<JObject>("validation", out jo1))
        inputInputDescriptor.Validation = new InputValidation()
        {
          DataType = ServiceEndpointContributionExtensions.GetOptionalEnumValue<InputDataType>(jo1, "dataType", InputDataType.None),
          IsRequired = ServiceEndpointContributionExtensions.GetOptionalValue<bool>(jo1, "isRequired"),
          Pattern = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo1, "pattern"),
          PatternMismatchErrorMessage = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo1, "patternMismatchErrorMessage"),
          MinValue = ServiceEndpointContributionExtensions.GetOptionalValue<Decimal?>(jo1, "minValue"),
          MaxValue = ServiceEndpointContributionExtensions.GetOptionalValue<Decimal?>(jo1, "maxValue"),
          MinLength = ServiceEndpointContributionExtensions.GetOptionalValue<int?>(jo1, "minValue"),
          MaxLength = ServiceEndpointContributionExtensions.GetOptionalValue<int?>(jo1, "maxLength")
        };
      JObject jobject;
      if (jo.TryGetValue<JObject>("values", out jobject))
      {
        InputValues inputValues = new InputValues();
        inputValues.InputId = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jobject, "inputId");
        inputValues.DefaultValue = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jobject, "defaultValue");
        JArray jarray;
        if (jobject.TryGetValue<JArray>("possibleValues", out jarray))
        {
          inputValues.PossibleValues = (IList<InputValue>) new List<InputValue>();
          foreach (JObject jo2 in jarray)
            inputValues.PossibleValues.Add(new InputValue()
            {
              Value = ServiceEndpointContributionExtensions.GetRequiredValue<string>(jo2, "value"),
              DisplayValue = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo2, "displayValue")
            });
        }
        inputValues.IsLimitedToPossibleValues = ServiceEndpointContributionExtensions.GetOptionalValue<bool>(jobject, "isLimitedToPossibleValues");
        inputValues.IsDisabled = ServiceEndpointContributionExtensions.GetOptionalValue<bool>(jobject, "isDisabled");
        inputValues.IsReadOnly = ServiceEndpointContributionExtensions.GetOptionalValue<bool>(jobject, "isReadOnly");
        JObject jo3;
        if (jo.TryGetValue<JObject>("error", out jo3))
          inputValues.Error = new InputValuesError()
          {
            Message = ServiceEndpointContributionExtensions.GetOptionalValue<string>(jo3, "message")
          };
        inputInputDescriptor.Values = inputValues;
      }
      JArray jarray1;
      if (jo.TryGetValue<JArray>("dependencyInputIds", out jarray1))
        inputInputDescriptor.DependencyInputIds = (IList<string>) jarray1.ToObject<List<string>>();
      return inputInputDescriptor;
    }

    private static T GetOptionalEnumValue<T>(JObject jo, string propertyName, T defaultValue) where T : struct
    {
      JToken jtoken;
      if (!jo.TryGetValue(propertyName, out jtoken))
        return defaultValue;
      T result = defaultValue;
      Enum.TryParse<T>(jtoken.Value<string>(), true, out result);
      return result;
    }

    public static T GetRequiredValue<T>(JObject jo, string propertyName)
    {
      JToken jtoken;
      if (jo.TryGetValue(propertyName, out jtoken))
        return jtoken.ToObject<T>();
      throw new ServiceEndpointException(ServiceEndpointResources.MissingProperty((object) propertyName));
    }

    public static T GetOptionalValue<T>(JObject jo, string propertyName, T defaultValue = null)
    {
      JToken jtoken;
      return jo.TryGetValue(propertyName, out jtoken) ? jtoken.ToObject<T>() : defaultValue;
    }

    private static Uri GetIconUrl(IVssRequestContext requestContext, Contribution typeContribution)
    {
      IContributionService service = requestContext.GetService<IContributionService>();
      string str = typeContribution.Properties["icon"]?.ToString() ?? "Microsoft.VisualStudio.Services.Icons.Default";
      IVssRequestContext requestContext1 = requestContext;
      string id = typeContribution.Id;
      string assetType = str;
      Uri result;
      Uri.TryCreate(service.QueryAssetLocation(requestContext1, id, assetType), UriKind.Absolute, out result);
      return result;
    }
  }
}
