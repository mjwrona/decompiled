// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.ServiceModelEnumValidatorAttribute
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  [AttributeUsage(AttributeTargets.Property)]
  internal sealed class ServiceModelEnumValidatorAttribute : ConfigurationValidatorAttribute
  {
    private Type enumHelperType;

    public ServiceModelEnumValidatorAttribute(Type enumHelperType) => this.EnumHelperType = enumHelperType;

    public Type EnumHelperType
    {
      get => this.enumHelperType;
      set => this.enumHelperType = value;
    }

    public override ConfigurationValidatorBase ValidatorInstance => (ConfigurationValidatorBase) new ServiceModelEnumValidator(this.enumHelperType);
  }
}
