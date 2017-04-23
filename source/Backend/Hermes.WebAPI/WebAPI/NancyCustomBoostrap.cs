using System;
using Hermes.WebAPI.WebAPI.Helpers;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Json;

namespace Hermes.WebAPI.WebAPI
{
    public class NancyCustomBootstrap : DefaultNancyBootstrapper
    {
        public NancyCustomBootstrap()
        {
            AppDomainAssemblyTypeScanner.AddAssembliesToScan("Hermes.DataObjects.dll");

            JsonSettings.PrimitiveConverters.Add(new JsonConvertEnum());
            JsonSettings.RetainCasing = true;
            JsonSettings.MaxJsonLength = Int32.MaxValue;
        }
    }
}
