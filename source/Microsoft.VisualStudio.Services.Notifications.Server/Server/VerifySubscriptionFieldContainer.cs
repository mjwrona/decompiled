// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.VerifySubscriptionFieldContainer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class VerifySubscriptionFieldContainer : IFieldContainer
  {
    public bool DualExecution { get; set; }

    public void AddOrUpdateNode(string name, string value)
    {
    }

    public string GetDocumentString() => "Test";

    public IFieldContainer GetDynamicFieldContainer(DynamicFieldContainerType type) => (IFieldContainer) this;

    public object GetFieldValue(string fieldName) => (object) "False";
  }
}
