// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.DefaultLanguageConfigurator
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal sealed class DefaultLanguageConfigurator : TfsLanguageConfiguratorBase
  {
    public DefaultLanguageConfigurator(IVssRequestContext requestContext, Stream content = null)
      : base(requestContext, content)
    {
    }

    protected override string[] LanguageNameAttributes => Array.Empty<string>();

    protected override string[] PathExclusionAttributes => Array.Empty<string>();

    protected override bool TryParseLine(string line, string[] v, out string[] config)
    {
      config = (string[]) null;
      return false;
    }
  }
}
