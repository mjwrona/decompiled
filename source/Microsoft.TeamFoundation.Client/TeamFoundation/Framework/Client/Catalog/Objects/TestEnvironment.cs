// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.TestEnvironment
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TestEnvironment : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.TestEnvironment;

    public string EnvironmentName
    {
      get => this.GetProperty<string>(nameof (EnvironmentName));
      set => this.SetProperty<string>(nameof (EnvironmentName), value);
    }

    public string ControllerName
    {
      get => this.GetProperty<string>(nameof (ControllerName));
      set => this.SetProperty<string>(nameof (ControllerName), value);
    }

    public string ProjectName
    {
      get => this.GetProperty<string>(nameof (ProjectName));
      set => this.SetProperty<string>(nameof (ProjectName), value);
    }

    public static class Fields
    {
      public const string EnvironmentName = "EnvironmentName";
      public const string ControllerName = "ControllerName";
      public const string ProjectName = "ProjectName";
    }
  }
}
