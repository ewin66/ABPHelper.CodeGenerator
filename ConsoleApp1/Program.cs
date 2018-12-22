using System;
using System.IO;
using System.Reflection;
using ConsoleApp.Test.Model;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace ConsoleApp.Test
{

    class Program
    {
        static void Main(string[] args)
        {
          //  InitRazorEngine();
            //获得当前运行的Assembly
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.Template.ServiceFileTemplate.cshtml");

            s.Position = 0; //将stream的其实点归零 
            StreamReader reader = new StreamReader(s, System.Text.Encoding.UTF8);
            string template = reader.ReadToEnd();

            var model = new UserInfo { FirstName = "Bill", LastName = "Gates" };
            var m2 = new ServiceFileModel
            {
                ServiceName = "Contract",
                Namespace = "Namespace",
                AppName = "AppName",
                InterfaceName = "InterfaceName"

            };

            var result = Engine.Razor.RunCompile(template, "templateKey2", null, m2);
           Test();

            string content = Engine.Razor.RunCompile("UserInfo", typeof(UserInfo), model);

     


            string a = Engine.Razor.RunCompile("ServiceFileTemplate", typeof(ServiceFileModel), m2);

            Console.WriteLine(content);
            Console.WriteLine(a);
            Console.Read();
        }

        static void Test()
        {

            var template = "Hello @Model.Name, welcome to use RazorEngine!";
            var result = Engine.Razor.RunCompile(template, "templateKey1",null, new { Name = "World" });
            Console.WriteLine(result);
            Console.Read();

        }

        private static void InitRazorEngine()
        {
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedResourceTemplateManager(typeof(Template.Template))
            };
            Engine.Razor = RazorEngineService.Create(config);
        }
    }
}
