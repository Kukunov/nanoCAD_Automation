using HostMgd.ApplicationServices;
using Teigha.Runtime;
using Application = HostMgd.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(NanoCAD.API.Main))]

namespace NanoCAD.API
{
    public class Main : IExtensionApplication
    {
        public void Initialize() { }
        public void Terminate() { }
    }
}