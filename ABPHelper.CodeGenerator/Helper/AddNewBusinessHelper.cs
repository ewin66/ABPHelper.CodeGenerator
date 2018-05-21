using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ABPHelper.CodeGenerator.Extensions;
using ABPHelper.CodeGenerator.Models.HelperModels;
using ABPHelper.CodeGenerator.Models.TemplateModels;
using EnvDTE;
using RazorEngine;
using RazorEngine.Templating;

namespace ABPHelper.CodeGenerator.Helper
{
    public class AddNewBusinessHelper : HelperBase<AddNewBusinessModel>
    {
        private Project _appProj;

        private Project _webProj;

        private readonly string _appName;

        private StatusBar _statusBar;

        private int _totalSteps;

        private int _steps;

        public AddNewBusinessHelper(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _appName = Dte.Solution.Properties.Item("Name").Value.ToString();
            _statusBar = Dte.StatusBar;
        }

        public override bool CanExecute(AddNewBusinessModel parameter)
        {
            var solution = Dte.Solution;
            var projects = solution.Projects.OfType<Project>();
            foreach (var project in projects)
            {
                var m = Regex.Match(project.Name, @"(.+)\.Application");
                if (m.Success)
                {
                    _appProj = project;
                }
                //解决正则匹配到webapi项目
                if (Regex.IsMatch(project.Name, @"(.+)\.Web")&& !Regex.IsMatch(project.Name, @"(.+)\.WebApi"))
                {
                    _webProj = project;
                }
                if (_appProj != null && _webProj != null) break;
            }
            if (_appProj == null)
            {
                Utils.MessageBox("Cannot find the Application project. Please ensure that your are in the ABP solution.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (_webProj == null)
            {
                Utils.MessageBox("Cannot find the Web project. Please ensure that your are in the ABP solution.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }

        public override void Execute(AddNewBusinessModel parameter)
        {
            try
            {
                _totalSteps = parameter.ViewFiles.Count()*2 + 2;
                _steps = 1;

                var folder = AddDeepFolder(_appProj.ProjectItems, parameter.ServiceFolder);
                AddDeepFolder(folder.ProjectItems, "Dto");
                CreateServiceFile(parameter, folder);
                CreateServiceInterfaceFile(parameter, folder);
                folder = AddDeepFolder(_webProj.ProjectItems, parameter.ViewFolder);
                CreateViewFiles(parameter, folder);

                Utils.MessageBox("Done!");
            }
            catch (Exception e)
            {
                Utils.MessageBox("Generation failed.\r\nException: {0}", MessageBoxButton.OK, MessageBoxImage.Exclamation, e.Message);
            }
            finally
            {
                _statusBar.Progress(false);
            }
        }

        private void CreateViewFiles(AddNewBusinessModel parameter, ProjectItem folder)
        {
            foreach (var viewFileViewModel in parameter.ViewFiles)
            {
                var model = new ViewFileModel
                {
                    BusinessName = parameter.BusinessName,
                    Namespace = GetNamespace(parameter.ViewFolder),
                    FileName = viewFileViewModel.FileName,
                    IsPopup = viewFileViewModel.IsPopup,
                    ViewFolder = parameter.ViewFolder,
                    ViewFiles = parameter.ViewFiles
                };
                foreach (var ext in new[] { ".cshtml", ".js" })
                {
                    var fileName = viewFileViewModel.FileName + ext;
                    _statusBar.Progress(true, $"Generating view file: {fileName}", _steps++, _totalSteps);
                    if (FindProjectItem(folder, fileName, ItemType.PhysicalFile) != null) continue;

                    var template = GetTemplateText(ext == ".cshtml" ? "CshtmlTemplate" : "JsTemplate");

                    var content = Engine.Razor.RunCompile(template, "ViewFileModel"+ ((ext == ".cshtml") ? "CshtmlTemplate" : "JsTemplate"), null, model);

                 //   string content = Engine.Razor.RunCompile(ext == ".cshtml" ? "CshtmlTemplate" : "JsTemplate", typeof(ViewFileModel), model);
                    CreateAndAddFile(folder, fileName, content);
                }
            }
        }

        private string GetNamespace(string viewFolder)
        {
            return string.Join(".", viewFolder.Split('\\').Select(s => s.LowerFirstChar()));
        }


        private void CreateServiceFile(AddNewBusinessModel parameter, ProjectItem folder)
        {
            var fileName = parameter.ServiceName + ".cs";
            _statusBar.Progress(true, $"Generating service file: {fileName}", _steps++, _totalSteps);
            if (FindProjectItem(folder, fileName, ItemType.PhysicalFile) != null) return;
            var model = new ServiceFileModel
            {
                BusinessName = parameter.BusinessName,
                AppName = _appName,
                Namespace = GetNamespace(parameter),
                InterfaceName = parameter.ServiceInterfaceName,
                ServiceName = parameter.ServiceName
            };

            var template = GetTemplateText("ServiceFileTemplate");

            var content = Engine.Razor.RunCompile(template, "ServiceFileModel", null, model);

          //  string content = Engine.Razor.RunCompile("ServiceFileTemplate", typeof(ServiceFileModel), model);
            CreateAndAddFile(folder, fileName, content);
        }

        private void CreateServiceInterfaceFile(AddNewBusinessModel parameter, ProjectItem folder)
        {
            var fileName = parameter.ServiceInterfaceName + ".cs";
            _statusBar.Progress(true, $"Generating interface file: {fileName}", _steps++, _totalSteps);
            if (FindProjectItem(folder, fileName, ItemType.PhysicalFile) != null) return;
            var model = new ServiceInterfaceFileModel
            {
                BusinessName = parameter.BusinessName,
                Namespace = GetNamespace(parameter),
                InterfaceName = parameter.ServiceInterfaceName
            };
            var template = GetTemplateText("ServiceInterfaceFileTemplate");

            var content = Engine.Razor.RunCompile(template, "ServiceInterfaceFileModel", null, model);
         //   string content = Engine.Razor.RunCompile("ServiceInterfaceFileTemplate", typeof(ServiceInterfaceFileModel), model);
            CreateAndAddFile(folder, fileName, content);
        }

        private string GetNamespace(AddNewBusinessModel parameter)
        {
            var str = parameter.ServiceFolder.Replace('\\', '.');
            return $"{_appName}.{str}";
        }

        private ProjectItem AddDeepFolder(ProjectItems parentItems, string deepFolder)
        {
            ProjectItem addedFolder = null;
            foreach (var folder in deepFolder.Split('\\'))
            {
                var projectItem = FindProjectItem(parentItems, folder, ItemType.PhysicalFolder);
                addedFolder = projectItem ?? parentItems.AddFolder(folder);
                parentItems = addedFolder.ProjectItems;
            }
            return addedFolder;
        }
    }
}