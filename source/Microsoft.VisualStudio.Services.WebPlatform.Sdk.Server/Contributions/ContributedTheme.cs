// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedTheme
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public class ContributedTheme
  {
    public bool Accessible;
    public string Extends;
    public bool Dark;
    public bool Preview;
    public Dictionary<string, string> Data;
    public string Name;
    public int Priority;
    public string Key;

    public Contribution Contribution { get; }

    public ContributedTheme(Contribution contribution)
    {
      this.Contribution = contribution;
      this.Accessible = contribution.GetProperty<bool>("accessible");
      this.Dark = contribution.GetProperty<bool>("dark");
      this.Preview = contribution.GetProperty<bool>("preview");
      this.Data = contribution.GetProperty<Dictionary<string, string>>("data");
      this.Extends = contribution.GetProperty<string>("extends");
      this.Name = contribution.GetProperty<string>("name");
      this.Priority = contribution.GetProperty<int>("priority");
      this.Key = string.Format("{0}.{1}", (object) this.Priority.ToString("D8"), (object) contribution.Id);
    }

    public override string ToString() => this.Contribution.Id.ToString();
  }
}
