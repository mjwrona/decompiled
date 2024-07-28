// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.FixedRuleCondition
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  [KnownType(typeof (SuppressionRuleCondition))]
  public class FixedRuleCondition : RuleCondition
  {
    [DataMember]
    public string RoutingId { get; set; }

    [DataMember]
    public string CorrelationId { get; set; }

    [DataMember]
    public string Environment { get; set; }

    [DataMember]
    public string DataCenter { get; set; }

    [DataMember]
    public string DeviceGroup { get; set; }

    [DataMember]
    public string DeviceName { get; set; }

    [DataMember]
    public string ServiceInstanceId { get; set; }

    [DataMember]
    public long? SiloId { get; set; }

    public static void ThrowIfNotValid(RuleCondition condition)
    {
      if (!(condition is FixedRuleCondition fixedRuleCondition))
        throw new ArgumentException(TypeUtility.Format("Unsupported condition type [{0}] encountered while validating FixedRuleCondition", (object) condition.GetType().Name), nameof (condition));
      TypeUtility.ValidateStringParameter(fixedRuleCondition.CorrelationId, "condition.CorrelationId", 1, 500, true);
      TypeUtility.ValidateStringParameter(fixedRuleCondition.RoutingId, "condition.RoutingId", 1, 200, true);
      TypeUtility.ValidateStringParameter(fixedRuleCondition.Environment, "condition.Environment", 1, 32, true);
      TypeUtility.ValidateStringParameter(fixedRuleCondition.DataCenter, "condition.DataCenter", 1, 32, true);
      TypeUtility.ValidateStringParameter(fixedRuleCondition.DeviceGroup, "condition.DeviceGroup", 1, 64, true);
      TypeUtility.ValidateStringParameter(fixedRuleCondition.DeviceName, "condition.DeviceName", 1, 128, true);
      TypeUtility.ValidateStringParameter(fixedRuleCondition.ServiceInstanceId, "condition.ServiceInstanceId", 1, 64, true);
    }

    public static IDictionary<string, InvalidReasons> IsValid(RuleCondition condition)
    {
      if (!(condition is FixedRuleCondition fixedRuleCondition))
        throw new ArgumentException(TypeUtility.Format("Unsupported condition type [{0}] encountered while validating FixedRuleCondition", (object) condition.GetType().Name), nameof (condition));
      IDictionary<string, InvalidReasons> invalidFields = (IDictionary<string, InvalidReasons>) new Dictionary<string, InvalidReasons>();
      string validated;
      FieldValidator.ValidateCorrelationId(fixedRuleCondition.CorrelationId, out validated, invalidFields);
      FieldValidator.ValidateRoutingId(fixedRuleCondition.RoutingId, out validated, invalidFields);
      FieldValidator.ValidateEnvironment(fixedRuleCondition.Environment, out validated, invalidFields);
      FieldValidator.ValidateDataCenter(fixedRuleCondition.DataCenter, out validated, invalidFields);
      FieldValidator.ValidateDeviceGroup(fixedRuleCondition.DeviceGroup, out validated, invalidFields);
      FieldValidator.ValidateDeviceName(fixedRuleCondition.DeviceName, out validated, invalidFields);
      FieldValidator.ValidateServiceInstanceId(fixedRuleCondition.ServiceInstanceId, out validated, invalidFields);
      return invalidFields;
    }
  }
}
