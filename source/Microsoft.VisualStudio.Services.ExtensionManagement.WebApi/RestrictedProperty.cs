// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.RestrictedProperty
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class RestrictedProperty
  {
    public const string ServiceInstanceTypeProperty = "::ServiceInstanceType";
    public const string AttributesProperty = "::Attributes";
    public const string BaseUriProperty = "::BaseUri";
    public const string FallbackBaseUriProperty = "::FallbackBaseUri";
    public const string VersionProperty = "::Version";
    public const string RegistrationIdProperty = "::RegistrationId";
    public const string HashcodeProperty = "::Hashcode";

    public static class AttributeValue
    {
      public const int BuiltIn = 1;
      public const int MultiVersion = 2;
      public const int Paid = 4;
      public const int Preview = 8;
      public const int Public = 16;
      public const int System = 32;
      public const int Trusted = 64;
    }
  }
}
