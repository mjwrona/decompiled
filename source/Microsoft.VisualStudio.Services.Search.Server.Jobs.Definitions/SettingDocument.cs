// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.SettingDocument
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class SettingDocument : CorePipelineDocument<string>
  {
    public Setting Setting { get; }

    public SettingDocument(Setting setting)
      : base(Setting.GetSettingId(setting.Title, setting.Scope))
    {
      this.Setting = setting;
    }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("Setting document id: {0}", (object) Setting.GetSettingId(this.Setting.Title, this.Setting.Scope)));
  }
}
