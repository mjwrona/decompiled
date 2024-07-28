// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.ProcessGuidanceSite
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Common.SharePoint;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProcessGuidanceSite : TfsRelativeWebSite
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.ProcessGuidanceSite;

    public string AsciiName
    {
      get => this.GetProperty<string>(nameof (AsciiName));
      set => this.SetProperty<string>(nameof (AsciiName), value);
    }

    public Uri FullyQualifiedUrl
    {
      get => this.GetProperty<Uri>(nameof (FullyQualifiedUrl));
      set => this.SetProperty<Uri>(nameof (FullyQualifiedUrl), value);
    }

    public ProcessGuidanceType ResourceSubType
    {
      get => this.GetProperty<ProcessGuidanceType>(nameof (ResourceSubType));
      set => this.SetProperty<ProcessGuidanceType>(nameof (ResourceSubType), value);
    }

    public string GuidanceFileName
    {
      get => this.GetProperty<string>(nameof (GuidanceFileName));
      set => this.SetProperty<string>(nameof (GuidanceFileName), value);
    }

    public Uri FullUrl
    {
      get
      {
        switch (this.ResourceSubType)
        {
          case ProcessGuidanceType.WssDocumentLibrary:
            return UriUtility.Combine(this.GetFullyQualifiedUri(), this.AsciiName, true);
          case ProcessGuidanceType.WebSite:
            return this.FullyQualifiedUrl;
          default:
            return (Uri) null;
        }
      }
    }

    public Uri WellKnownGuidancePageUrl => SharePointUtilities.GetWellKnownProcessGuidancePageUrl(this.ResourceSubType, this.FullUrl, this.GuidanceFileName);

    public bool HasAdminUrl => this.ResourceSubType == ProcessGuidanceType.WssDocumentLibrary;

    public Uri AdminUrl
    {
      get
      {
        if (this.HasAdminUrl && this.ReferencedResource != null)
        {
          SharePointWebApplication pointWebApplication = this.ReferencedResource.As<SharePointWebApplication>();
          if (pointWebApplication != null)
            return pointWebApplication.AdminUrlServiceLocation;
        }
        return (Uri) null;
      }
    }

    public void ResetValues()
    {
      this.ResourceSubType = ProcessGuidanceType.WssDocumentLibrary;
      this.AsciiName = (string) null;
      this.ReferencedResource = (CatalogObject) null;
      this.RelativePath = (string) null;
      this.FullyQualifiedUrl = (Uri) null;
    }

    public new static class Fields
    {
      public const string AsciiName = "AsciiName";
      public const string FullyQualifiedUrl = "FullyQualifiedUrl";
      public const string ResourceSubType = "ResourceSubType";
      public const string GuidanceFileName = "GuidanceFileName";
    }
  }
}
