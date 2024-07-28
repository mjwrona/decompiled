// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandServiceFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandServiceFactory : ICommandFactory
  {
    private IVssRequestContext _requestContext;
    private ICommandProperties _properties;

    public CommandServiceFactory(
      IVssRequestContext requestContext,
      CommandKey commandKey,
      CommandPropertiesSetter defaultProperties)
    {
      this._requestContext = requestContext;
      this.CommandKey = commandKey;
      this._properties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, commandKey, defaultProperties);
    }

    public CommandKey CommandKey { get; private set; }

    public CommandAsync CreateCommandAsync(
      CommandSetter commandSetter,
      Func<Task> run = null,
      Func<Task> fallback = null,
      bool continueOnCapturedContext = false)
    {
      return (CommandAsync) new CommandServiceAsync(this._requestContext, commandSetter, this._properties, run, fallback, continueOnCapturedContext);
    }

    public CommandAsync<TResult> CreateCommandAsync<TResult>(
      CommandSetter commandSetter,
      Func<Task<TResult>> run = null,
      Func<Task<TResult>> fallback = null,
      bool continueOnCapturedContext = false)
    {
      return (CommandAsync<TResult>) new CommandServiceAsync<TResult>(this._requestContext, commandSetter, this._properties, run, fallback, continueOnCapturedContext);
    }
  }
}
