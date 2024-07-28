// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.ServiceModelEnumValidator
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Diagnostics;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  internal class ServiceModelEnumValidator : ConfigurationValidatorBase
  {
    private Type enumHelperType;
    private MethodInfo isDefined;

    public ServiceModelEnumValidator(Type enumHelperType)
    {
      this.enumHelperType = enumHelperType;
      this.isDefined = this.enumHelperType.GetMethod("IsDefined", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public override bool CanValidate(Type type) => this.isDefined != (MethodInfo) null;

    public override void Validate(object value)
    {
      if (!(bool) this.isDefined.Invoke((object) null, new object[1]
      {
        value
      }))
      {
        ParameterInfo[] parameters = this.isDefined.GetParameters();
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidEnumArgumentException(nameof (value), (int) value, parameters[0].ParameterType));
      }
    }
  }
}
