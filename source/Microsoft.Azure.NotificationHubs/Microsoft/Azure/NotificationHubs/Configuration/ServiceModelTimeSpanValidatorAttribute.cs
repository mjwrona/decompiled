// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.ServiceModelTimeSpanValidatorAttribute
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  [AttributeUsage(AttributeTargets.Property)]
  internal sealed class ServiceModelTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
  {
    private TimeSpanValidatorAttribute innerValidatorAttribute;

    public ServiceModelTimeSpanValidatorAttribute()
    {
      this.innerValidatorAttribute = new TimeSpanValidatorAttribute();
      this.innerValidatorAttribute.MaxValueString = TimeoutHelper.MaxWait.ToString();
    }

    public override ConfigurationValidatorBase ValidatorInstance => (ConfigurationValidatorBase) new TimeSpanOrInfiniteValidator(this.MinValue, this.MaxValue);

    public TimeSpan MinValue => this.innerValidatorAttribute.MinValue;

    public string MinValueString
    {
      get => this.innerValidatorAttribute.MinValueString;
      set => this.innerValidatorAttribute.MinValueString = value;
    }

    public TimeSpan MaxValue => this.innerValidatorAttribute.MaxValue;

    public string MaxValueString
    {
      get => this.innerValidatorAttribute.MaxValueString;
      set => this.innerValidatorAttribute.MaxValueString = value;
    }
  }
}
