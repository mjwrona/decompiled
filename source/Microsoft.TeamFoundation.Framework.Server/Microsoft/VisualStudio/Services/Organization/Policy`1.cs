// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Policy`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class Policy<T>
  {
    public Policy(string name, T value, bool enforce, bool isValueUndefined)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.Name = name;
      this.Value = value;
      this.Enforce = enforce;
      this.IsValueUndefined = isValueUndefined;
    }

    public Policy(
      string name,
      T value,
      bool enforce,
      bool isValueUndefined,
      Policy<T> parentPolicy)
      : this(name, value, enforce, isValueUndefined)
    {
      this.ParentPolicy = parentPolicy;
    }

    public string Name { get; }

    public T Value { get; }

    public bool Enforce { get; }

    public bool IsValueUndefined { get; }

    public Policy<T> ParentPolicy { get; }

    public T EffectiveValue => this.ParentPolicy == null || !this.ParentPolicy.Enforce ? this.Value : this.ParentPolicy.EffectiveValue;
  }
}
