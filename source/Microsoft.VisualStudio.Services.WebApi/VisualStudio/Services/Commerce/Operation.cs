// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Operation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class Operation
  {
    private OperationProperties displayProperties;

    public Operation(string provider, string resource, string operation, string description) => this.displayProperties = new OperationProperties(provider, resource, operation, description);

    public string name { get; set; }

    public OperationProperties display
    {
      get => this.displayProperties;
      set => value = this.displayProperties;
    }

    public static Operation GetOperationDescriptorForRPandAction(
      ResourceProvider resourceProvider,
      OperationAction action)
    {
      return Operation.GetOperationDescriptorForRPandAction(resourceProvider, ResourceProvider.None, action);
    }

    public static Operation GetOperationDescriptorForRPandAction(
      ResourceProvider rootResourceProvider,
      ResourceProvider childResourceProvider,
      OperationAction action)
    {
      string empty = string.Empty;
      string resource = childResourceProvider == ResourceProvider.None ? rootResourceProvider.ToString() : string.Format("{0}/{1}", (object) rootResourceProvider, (object) childResourceProvider);
      string operation;
      string description;
      switch (action)
      {
        case OperationAction.Write:
          operation = "Creates or updates the " + resource;
          description = "Set " + resource;
          break;
        case OperationAction.Delete:
          operation = "Deletes the " + resource;
          description = "Delete " + resource;
          break;
        case OperationAction.Read:
          operation = "Reads the " + resource;
          description = "Read " + resource;
          break;
        case OperationAction.Action:
          operation = "Registers the Azure Subscription with Microsoft.VisualStudio provider";
          description = "Register Azure Subscription with Microsoft.VisualStudio provider";
          break;
        default:
          operation = string.Empty;
          description = string.Empty;
          break;
      }
      return new Operation("Visual Studio", resource, operation, description)
      {
        name = string.Format("Microsoft.VisualStudio/{0}/{1}", action == OperationAction.Action ? (object) "Register" : (object) resource, (object) action)
      };
    }
  }
}
