// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingAddress
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal sealed class MessagingAddress
  {
    internal const string UriScheme = "sb";
    private static readonly UriTemplate CanonicalAddressTemplate = new UriTemplate("applications/{appid}/modules/{modname}/components/{compname}/exports/{exportname}");
    private static readonly UriTemplate CanonicalAddressWithEntityNameTemplate = new UriTemplate("applications/{appid}/modules/{modname}/components/{compname}/exports/{exportname}/{entityname}");
    private static readonly UriTemplate PublishedAddressTemplate = new UriTemplate("applications/{appid}/{containername}");
    private static readonly UriTemplate PublishedAddressWithEntityNameTemplate = new UriTemplate("applications/{appid}/{containername}/{entityname}");
    private const string EntityVariableName = "entityname";
    private readonly bool preservePathSegmentAsBaseAddress;

    public MessagingAddress(Uri logicalAddress, bool preservePathSegmentAsBaseAddress)
    {
      this.preservePathSegmentAsBaseAddress = preservePathSegmentAsBaseAddress;
      this.Initialize(logicalAddress);
    }

    public Uri NormalizedLogicalAddress { get; private set; }

    public Uri NamingAuthority { get; private set; }

    public Uri ResourceAddress { get; private set; }

    public string EntityName { get; private set; }

    public MessagingAddressType Type { get; private set; }

    public override string ToString() => this.NormalizedLogicalAddress.AbsoluteUri;

    private static Uri RemoveTrailingSlash(Uri uri)
    {
      string absoluteUri = uri.AbsoluteUri;
      return absoluteUri.EndsWith("/", StringComparison.Ordinal) ? new Uri(absoluteUri.Substring(0, absoluteUri.Length - 1)) : uri;
    }

    private void SetNamingAuthority(Uri logicalAddress) => this.NamingAuthority = new UriBuilder(logicalAddress.Scheme, logicalAddress.Host, logicalAddress.Port).Uri;

    private Uri GetResourceAddress(Uri normalizedAddress) => new Uri(this.NamingAuthority, normalizedAddress.PathAndQuery);

    private void Initialize(Uri logicalAddress)
    {
      this.SetNamingAuthority(logicalAddress);
      Uri uri = new Uri(logicalAddress.Scheme + Uri.SchemeDelimiter + logicalAddress.Authority);
      logicalAddress = MessagingAddress.RemoveTrailingSlash(logicalAddress);
      UriTemplateMatch uriTemplateMatch1;
      if ((uriTemplateMatch1 = MessagingAddress.CanonicalAddressTemplate.Match(uri, logicalAddress)) != null)
      {
        this.NormalizedLogicalAddress = logicalAddress;
        this.Type = MessagingAddressType.Container;
      }
      else
      {
        UriTemplateMatch uriTemplateMatch2;
        if ((uriTemplateMatch2 = MessagingAddress.CanonicalAddressWithEntityNameTemplate.Match(uri, logicalAddress)) != null)
        {
          string[] segments = logicalAddress.Segments;
          this.NormalizedLogicalAddress = new Uri(uri, string.Join(string.Empty, segments, 0, segments.Length - 1));
          this.Type = MessagingAddressType.Entity;
          this.EntityName = uriTemplateMatch2.BoundVariables["entityname"];
        }
        else if ((uriTemplateMatch1 = MessagingAddress.PublishedAddressTemplate.Match(uri, logicalAddress)) != null)
        {
          this.NormalizedLogicalAddress = logicalAddress;
          this.Type = MessagingAddressType.Container;
        }
        else
        {
          UriTemplateMatch uriTemplateMatch3;
          if ((uriTemplateMatch3 = MessagingAddress.PublishedAddressWithEntityNameTemplate.Match(uri, logicalAddress)) != null)
          {
            string[] segments = logicalAddress.Segments;
            this.NormalizedLogicalAddress = new Uri(uri, string.Join(string.Empty, segments, 0, segments.Length - 1));
            this.Type = MessagingAddressType.Entity;
            this.EntityName = uriTemplateMatch3.BoundVariables["entityname"];
          }
          else
          {
            string[] segments = logicalAddress.Segments;
            int num = 1;
            if (this.preservePathSegmentAsBaseAddress && segments.Length > 2)
              num = 2;
            this.Type = MessagingAddressType.Entity;
            this.EntityName = string.Join(string.Empty, segments, num, segments.Length - num);
            this.NormalizedLogicalAddress = new Uri(uri, string.Join(string.Empty, segments, 0, num));
          }
        }
      }
      this.NormalizedLogicalAddress = MessagingAddress.RemoveTrailingSlash(this.NormalizedLogicalAddress);
      this.ResourceAddress = this.GetResourceAddress(this.NormalizedLogicalAddress);
    }
  }
}
