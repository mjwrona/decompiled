// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AgentCloudContributionExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class AgentCloudContributionExtensions
  {
    public static TaskAgentCloudType ToTaskAgentCloudType(
      this Contribution contribution,
      IVssRequestContext requestContext)
    {
      JObject properties = contribution.Properties;
      TaskAgentCloudType taskAgentCloudType = new TaskAgentCloudType();
      taskAgentCloudType.Name = AgentCloudContributionExtensions.GetRequiredValue<string>(properties, "name");
      taskAgentCloudType.DisplayName = AgentCloudContributionExtensions.GetRequiredValue<string>(properties, "displayName");
      taskAgentCloudType.InputDescriptors.AddRange((IEnumerable<InputDescriptor>) AgentCloudContributionExtensions.GetInputDescriptors(properties));
      return taskAgentCloudType;
    }

    public static Contribution Clone(this Contribution contribution)
    {
      Contribution contribution1 = new Contribution();
      contribution1.Id = contribution.Id;
      contribution1.Type = contribution.Type;
      return contribution1;
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
          InputDescriptor inputInputDescriptor = AgentCloudContributionExtensions.ToFormInputInputDescriptor(jo, defaultConfidentiality);
          inputInputDescriptor.Description = SafeHtmlWrapper.MakeSafe(inputInputDescriptor.Description);
          inputDescriptors.Add(inputInputDescriptor);
        }
      }
      return inputDescriptors;
    }

    private static InputDescriptor ToFormInputInputDescriptor(
      JObject jo,
      bool defaultConfidentiality)
    {
      InputDescriptor inputInputDescriptor = new InputDescriptor();
      inputInputDescriptor.Id = AgentCloudContributionExtensions.GetRequiredValue<string>(jo, "id");
      inputInputDescriptor.Name = AgentCloudContributionExtensions.GetRequiredValue<string>(jo, "name");
      inputInputDescriptor.Description = AgentCloudContributionExtensions.GetOptionalValue<string>(jo, "description");
      inputInputDescriptor.InputMode = AgentCloudContributionExtensions.GetOptionalEnumValue<InputMode>(jo, "inputMode", InputMode.None);
      inputInputDescriptor.IsConfidential = AgentCloudContributionExtensions.GetOptionalValue<bool>(jo, "isConfidential", defaultConfidentiality);
      inputInputDescriptor.GroupName = AgentCloudContributionExtensions.GetOptionalValue<string>(jo, "groupName");
      inputInputDescriptor.ValueHint = AgentCloudContributionExtensions.GetOptionalValue<string>(jo, "valueHint");
      inputInputDescriptor.HasDynamicValueInformation = AgentCloudContributionExtensions.GetOptionalValue<bool>(jo, "hasDynamicValueInformation");
      JObject jo1;
      if (jo.TryGetValue<JObject>("validation", out jo1))
        inputInputDescriptor.Validation = new InputValidation()
        {
          DataType = AgentCloudContributionExtensions.GetOptionalEnumValue<InputDataType>(jo1, "dataType", InputDataType.None),
          IsRequired = AgentCloudContributionExtensions.GetOptionalValue<bool>(jo1, "isRequired"),
          Pattern = AgentCloudContributionExtensions.GetOptionalValue<string>(jo1, "pattern"),
          PatternMismatchErrorMessage = AgentCloudContributionExtensions.GetOptionalValue<string>(jo1, "patternMismatchErrorMessage"),
          MinValue = AgentCloudContributionExtensions.GetOptionalValue<Decimal?>(jo1, "minValue"),
          MaxValue = AgentCloudContributionExtensions.GetOptionalValue<Decimal?>(jo1, "maxValue"),
          MinLength = AgentCloudContributionExtensions.GetOptionalValue<int?>(jo1, "minValue"),
          MaxLength = AgentCloudContributionExtensions.GetOptionalValue<int?>(jo1, "maxLength")
        };
      JObject jobject;
      if (jo.TryGetValue<JObject>("values", out jobject))
      {
        InputValues inputValues = new InputValues();
        inputValues.InputId = AgentCloudContributionExtensions.GetRequiredValue<string>(jobject, "inputId");
        inputValues.DefaultValue = AgentCloudContributionExtensions.GetOptionalValue<string>(jobject, "defaultValue");
        JArray jarray;
        if (jobject.TryGetValue<JArray>("possibleValues", out jarray))
        {
          inputValues.PossibleValues = (IList<InputValue>) new List<InputValue>();
          foreach (JObject jo2 in jarray)
            inputValues.PossibleValues.Add(new InputValue()
            {
              Value = AgentCloudContributionExtensions.GetRequiredValue<string>(jo2, "value"),
              DisplayValue = AgentCloudContributionExtensions.GetOptionalValue<string>(jo2, "displayValue")
            });
        }
        inputValues.IsLimitedToPossibleValues = AgentCloudContributionExtensions.GetOptionalValue<bool>(jobject, "isLimitedToPossibleValues");
        inputValues.IsDisabled = AgentCloudContributionExtensions.GetOptionalValue<bool>(jobject, "isDisabled");
        inputValues.IsReadOnly = AgentCloudContributionExtensions.GetOptionalValue<bool>(jobject, "isReadOnly");
        JObject jo3;
        if (jo.TryGetValue<JObject>("error", out jo3))
          inputValues.Error = new InputValuesError()
          {
            Message = AgentCloudContributionExtensions.GetOptionalValue<string>(jo3, "message")
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

    public static T GetOptionalValue<T>(JObject jo, string propertyName, T defaultValue = null)
    {
      JToken jtoken;
      return jo.TryGetValue(propertyName, out jtoken) ? jtoken.ToObject<T>() : defaultValue;
    }

    public static T GetRequiredValue<T>(JObject jo, string propertyName)
    {
      JToken jtoken;
      if (jo.TryGetValue(propertyName, out jtoken))
        return jtoken.ToObject<T>();
      throw new Exception("Missing property " + propertyName);
    }
  }
}
