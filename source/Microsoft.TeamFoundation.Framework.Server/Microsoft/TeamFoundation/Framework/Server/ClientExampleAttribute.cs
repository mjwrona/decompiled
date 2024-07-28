// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientExampleAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ClientExampleAttribute : Attribute
  {
    public ClientExampleAttribute(
      string httpExample,
      string title = null,
      string notes = null,
      string cSharpExample = null)
    {
      this.Title = title;
      this.Notes = notes;
      this.HttpExample = httpExample;
      this.CSharpExample = cSharpExample;
    }

    public string Title { get; }

    public string Notes { get; }

    public string HttpExample { get; }

    public string CSharpExample { get; }
  }
}
