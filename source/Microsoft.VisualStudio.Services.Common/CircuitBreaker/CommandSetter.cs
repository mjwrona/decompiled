// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandSetter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandSetter
  {
    public CommandGroupKey GroupKey { get; }

    public CommandKey CommandKey { get; private set; }

    public CommandPropertiesSetter CommandPropertiesDefaults { get; private set; }

    public CommandSetter(CommandGroupKey groupKey) => this.GroupKey = groupKey;

    public static CommandSetter WithGroupKey(CommandGroupKey groupKey) => new CommandSetter(groupKey);

    public CommandSetter AndCommandKey(CommandKey commandKey)
    {
      this.CommandKey = commandKey;
      return this;
    }

    public CommandSetter AndCommandPropertiesDefaults(
      CommandPropertiesSetter commandPropertiesDefaults)
    {
      this.CommandPropertiesDefaults = commandPropertiesDefaults;
      return this;
    }
  }
}
