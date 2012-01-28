using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SMSdisplay.Plugins.FullscreenChat.BuildInfo;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Fullscreen Chat")]
[assembly: AssemblyDescription("Full screen chat showing multiple messages")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct(Product.Name)]
[assembly: AssemblyCopyright("Copyright © 2009, 2010 Jasper Boot")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("558362ad-424a-4a1f-950e-529c1d862d74")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(BuildVersion.ProductVersion)]
[assembly: AssemblyFileVersion(PluginApi.TechnicalVersion)]
