// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionIdentifier
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public class ExtensionIdentifier
  {
    private static char[] s_nameSeparator = new char[1]
    {
      '.'
    };
    private string m_fullyQualifiedName;
    private string m_publisherName;
    private string m_extensionName;

    public ExtensionIdentifier()
    {
    }

    public ExtensionIdentifier(string fullyQualifiedName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fullyQualifiedName, nameof (fullyQualifiedName));
      string[] strArray = fullyQualifiedName.Split(ExtensionIdentifier.s_nameSeparator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 2)
        throw new ArgumentException(GalleryWebApiResources.InvalidExtensionIdentifier((object) fullyQualifiedName));
      GalleryUtil.CheckPublisherName(strArray[0]);
      GalleryUtil.CheckExtensionName(strArray[1]);
      this.m_publisherName = strArray[0];
      this.m_extensionName = strArray[1];
      this.m_fullyQualifiedName = fullyQualifiedName;
    }

    public ExtensionIdentifier(string publisherName, string extensionName)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      this.m_publisherName = publisherName;
      this.m_extensionName = extensionName;
      this.m_fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(this.m_publisherName, this.m_extensionName);
    }

    [DataMember]
    public string ExtensionName
    {
      get => this.m_extensionName;
      set
      {
        GalleryUtil.CheckExtensionName(value);
        this.m_extensionName = value;
        if (this.m_publisherName == null)
          return;
        this.m_fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(this.m_publisherName, this.m_extensionName);
      }
    }

    [DataMember]
    public string PublisherName
    {
      get => this.m_publisherName;
      set
      {
        GalleryUtil.CheckPublisherName(value);
        this.m_publisherName = value;
        if (this.m_extensionName == null)
          return;
        this.m_fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(this.m_publisherName, this.m_extensionName);
      }
    }

    public override string ToString() => this.m_fullyQualifiedName;

    public static bool TryParse(
      string extensionFullyQualifiedName,
      out ExtensionIdentifier extensionIdentifier)
    {
      try
      {
        extensionIdentifier = new ExtensionIdentifier(extensionFullyQualifiedName);
        return true;
      }
      catch (ArgumentException ex)
      {
        extensionIdentifier = (ExtensionIdentifier) null;
        return false;
      }
    }
  }
}
