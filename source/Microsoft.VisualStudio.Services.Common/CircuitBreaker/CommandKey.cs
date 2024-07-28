// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandKey : ImmutableKey
  {
    public CommandKey(string name)
      : base(name)
    {
    }

    public static implicit operator CommandKey(string name) => new CommandKey(name);

    public CommandKey(Type commandType)
      : base(CommandKey.GetDefaultNameForCommandType(commandType))
    {
    }

    private static string GetDefaultNameForCommandType(Type commandType) => !(commandType == (Type) null) ? commandType.Name : throw new ArgumentNullException(nameof (commandType));
  }
}
