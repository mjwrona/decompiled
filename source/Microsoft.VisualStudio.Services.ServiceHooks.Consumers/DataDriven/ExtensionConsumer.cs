// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.ExtensionConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public class ExtensionConsumer : DataDrivenConsumer
  {
    private string m_id;

    public ExtensionConsumer(JObject jo)
      : base(ExtensionConsumer.ToDataDrivenConsumerConfig(jo))
    {
      this.Actions = (IList<ConsumerActionImplementation>) new List<ConsumerActionImplementation>();
      if (this.m_consumerDataConfig.Actions == null)
        return;
      foreach (DataDrivenConsumerActionConfig action in this.m_consumerDataConfig.Actions)
        this.Actions.Add((ConsumerActionImplementation) new ExtensionConsumerAction(action));
    }

    public override string Id => this.m_id != null ? this.m_id : base.Id;

    public void SetExtensionConsumerId(string value) => this.m_id = value;

    public static DataDrivenConsumerConfig ToDataDrivenConsumerConfig(JObject jo) => DataDrivenConsumerConfig.CreateFromJsonString(jo.ToString());
  }
}
