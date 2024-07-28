// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsValidator
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class RemoteSettingsValidator : IRemoteSettingsValidator
  {
    internal static readonly string CyclesDetectedMessage = "Cycles detected in Scopes";
    private ICycleDetection cycleDetection;
    private IScopesStorageHandler scopesStorageHandler;

    public RemoteSettingsValidator(
      ICycleDetection cycleDetection,
      IScopesStorageHandler scopesStorageHandler)
    {
      cycleDetection.RequiresArgumentNotNull<ICycleDetection>(nameof (cycleDetection));
      scopesStorageHandler.RequiresArgumentNotNull<IScopesStorageHandler>(nameof (scopesStorageHandler));
      this.cycleDetection = cycleDetection;
      this.scopesStorageHandler = scopesStorageHandler;
    }

    public void ValidateDeserialized(DeserializedRemoteSettings remoteSettings) => this.ValidateScopes((IEnumerable<Scope>) remoteSettings.Scopes);

    public void ValidateStored()
    {
      List<Scope> scopeList = new List<Scope>();
      foreach (string allScope in this.scopesStorageHandler.GetAllScopes())
      {
        string scope = this.scopesStorageHandler.GetScope(allScope);
        scopeList.Add(new Scope()
        {
          Name = allScope,
          ScopeString = scope
        });
      }
      this.ValidateScopes((IEnumerable<Scope>) scopeList);
    }

    private void ValidateScopes(IEnumerable<Scope> scopes)
    {
      if (this.cycleDetection.HasCycles(scopes))
        throw new RemoteSettingsValidationException(RemoteSettingsValidator.CyclesDetectedMessage);
    }
  }
}
