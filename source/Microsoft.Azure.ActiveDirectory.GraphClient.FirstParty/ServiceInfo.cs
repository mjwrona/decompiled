// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ServiceInfo
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [Entity("serviceInfo", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo", "Microsoft.DirectoryServices.ServiceInfo"})]
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  public class ServiceInfo : DirectoryObject
  {
    private string _serviceInstance;
    private int? _version;
    private ChangeTrackingCollection<string> _serviceElements;
    private bool _serviceElementsInitialized;

    public ServiceInfo() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo";

    public ServiceInfo(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("serviceInstance")]
    public string ServiceInstance
    {
      get => this._serviceInstance;
      set
      {
        this._serviceInstance = value;
        this.ChangedProperties.Add(nameof (ServiceInstance));
      }
    }

    [JsonProperty("version")]
    public int? Version
    {
      get => this._version;
      set
      {
        this._version = value;
        this.ChangedProperties.Add(nameof (Version));
      }
    }

    [JsonProperty("serviceElements")]
    public ChangeTrackingCollection<string> ServiceElements
    {
      get
      {
        if (this._serviceElements == null)
          this._serviceElements = new ChangeTrackingCollection<string>();
        if (!this._serviceElementsInitialized)
        {
          this._serviceElements.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ServiceElements)));
          this._serviceElementsInitialized = true;
        }
        return this._serviceElements;
      }
      set
      {
        this._serviceElements = value;
        this.ChangedProperties.Add(nameof (ServiceElements));
      }
    }
  }
}
