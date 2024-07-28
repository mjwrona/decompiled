using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: Extension]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("© Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyDescription("Azure Key Vault enables users to store and use cryptographic keys within the Microsoft Azure environment. Azure Key Vault supports multiple key types and algorithms and enables the use of Hardware Security Modules (HSM) for high value customer keys. In addition, Azure Key Vault allows users to securely store secrets in a Key Vault; secrets are limited size octet objects and Azure Key Vault applies no specific semantics to these objects. A Key Vault may contain a mix of keys and secrets at the same time, and access control for the two types of object is independently controlled. Users, subject to appropriate authorization, may: 1) Manage cryptographic keys using Create, Import, Update, Delete and other operations 2) Manage secrets using Get, Set, Delete and other operations 3) Use cryptographic keys with Sign/Verify, WrapKey/UnwrapKey and Encrypt/Decrypt operations. Operations against Key Vaults are authenticated and authorized using Azure Active Directory. Key Vault now supports certificates, a complex type that makes use of existing key and secret infrastructure for certificate operations. KV certificates also support notification and auto-renewal as well as other management features.\r\n\r\nThis library has been replaced by the following new Azure SDKs. You can read about the new Azure SDKs at https://aka.ms/azsdkvalueprop.\r\n\r\nThe latest libraries to interact with the Azure KeyVault service are:\r\n\r\n*\thttps://www.nuget.org/packages/Azure.Security.KeyVault.Keys\r\n*\thttps://www.nuget.org/packages/Azure.Security.KeyVault.Secrets\r\n*\thttps://www.nuget.org/packages/Azure.Security.KeyVault.Certificates\r\n\r\nIt is recommended that you move to the new package.")]
[assembly: AssemblyFileVersion("3.0.520.10303")]
[assembly: AssemblyInformationalVersion("3.0.5+99f52a3417df5d3023d10997cb20e7499207e976")]
[assembly: AssemblyProduct("Azure .NET SDK")]
[assembly: AssemblyTitle("Microsoft Azure Key Vault")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyVersion("3.0.5.0")]
