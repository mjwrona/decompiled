// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.StsUriElementCollection
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  [ConfigurationCollection(typeof (StsUriElement), AddItemName = "stsUri", CollectionType = ConfigurationElementCollectionType.BasicMap)]
  public sealed class StsUriElementCollection : ConfigurationElementCollection
  {
    private volatile List<Uri> stsUris;
    private readonly object syncLock;

    public StsUriElementCollection() => this.syncLock = new object();

    protected override string ElementName => string.Empty;

    protected override bool IsElementName(string elementName) => elementName == "stsUri";

    protected override ConfigurationElement CreateNewElement() => (ConfigurationElement) new StsUriElement();

    protected override object GetElementKey(ConfigurationElement element) => (object) ((StsUriElement) element).Value;

    public IEnumerable<Uri> Addresses
    {
      get
      {
        if (this.stsUris == null)
        {
          lock (this.syncLock)
          {
            if (this.stsUris == null)
              this.stsUris = this.Cast<StsUriElement>().Select<StsUriElement, Uri>((Func<StsUriElement, Uri>) (stsUri => stsUri.Value)).ToList<Uri>();
          }
        }
        return (IEnumerable<Uri>) this.stsUris;
      }
    }
  }
}
