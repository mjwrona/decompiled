// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.ReportingFolder
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ReportingFolder : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.ReportingFolder;
    private string m_fullPath;

    public string ItemPath
    {
      get => this.GetProperty<string>(nameof (ItemPath));
      set => this.SetProperty<string>(nameof (ItemPath), value);
    }

    public CatalogObject ReferencedResource
    {
      get => this.GetDependency<CatalogObject>(nameof (ReferencedResource));
      set => this.SetDependency<CatalogObject>(nameof (ReferencedResource), value);
    }

    protected override void OnRefresh()
    {
      base.OnRefresh();
      this.m_fullPath = (string) null;
    }

    public string FullPath
    {
      get
      {
        if (this.m_fullPath == null)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(this.ItemPath);
          CatalogObject catalogObject1 = this.ReferencedResource;
          while (catalogObject1 != null)
          {
            string str = (string) null;
            CatalogObject catalogObject2 = (CatalogObject) null;
            ReportingFolder reportingFolder = catalogObject1.As<ReportingFolder>();
            if (reportingFolder != null)
            {
              str = reportingFolder.ItemPath;
              catalogObject2 = reportingFolder.ReferencedResource;
            }
            catalogObject1 = catalogObject2;
            if (!string.IsNullOrEmpty(str) && str != "/")
              stringBuilder.Insert(0, str + "/");
          }
          if (stringBuilder[0] != '/')
            stringBuilder.Insert(0, '/');
          this.m_fullPath = stringBuilder.ToString();
        }
        return this.m_fullPath;
      }
    }

    public ReportingServer GetReportServer()
    {
      CatalogObject referencedResource = this.ReferencedResource;
      ReportingFolder reportingFolder;
      for (; referencedResource != null; referencedResource = reportingFolder == null ? (CatalogObject) null : reportingFolder.ReferencedResource)
      {
        ReportingServer reportServer = referencedResource.As<ReportingServer>();
        if (reportServer != null)
          return reportServer;
        reportingFolder = referencedResource.As<ReportingFolder>();
      }
      return (ReportingServer) null;
    }

    public static class Fields
    {
      public const string ItemPath = "ItemPath";
      public const string ReferencedResource = "ReferencedResource";
    }
  }
}
