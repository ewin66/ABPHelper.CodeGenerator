using System;
using System.IO;
using System.Reflection;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace ABPHelper.CodeGenerator.Helper
{
    public abstract class HelperBase<TParam>
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly DTE2 Dte;

        protected HelperBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
        }

        public abstract bool CanExecute(TParam parameter);
        public abstract void Execute(TParam parameter);

        protected void CreateAndAddFile(ProjectItem parentItem, string fileName, string content)
        {
            string path = Path.GetTempPath();
            Directory.CreateDirectory(path);
            string file = Path.Combine(path, fileName);
            File.WriteAllText(file, content, Encoding.UTF8);
            try
            {
                parentItem.ProjectItems.AddFromFileCopy(file);
            }
            finally
            {
                File.Delete(file);
            }
        }

        protected ProjectItem FindProjectItem(ProjectItem parentItem, string name, string type)
        {
            return FindProjectItem(parentItem.ProjectItems, name, type);
        }

        protected ProjectItem FindProjectItem(ProjectItems parentItems, string name, string type)
        {
            foreach (ProjectItem projectItem in parentItems)
            {
                if (projectItem.Name == name && projectItem.Kind == type) return projectItem;
            }
            return null;
        }

        /// <summary>
        /// 得到模板的text文本，嵌入式资源
        /// </summary>
        /// <param name="templateName">模板名称，不需要cshtml</param>
        /// <returns></returns>
        protected string GetTemplateText(string templateName)
        {
            //获得当前运行的Assembly
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ABPHelper.CodeGenerator.Templates.{templateName}.cshtml");

            s.Position = 0; //将stream的其实点归零 
            StreamReader reader = new StreamReader(s, System.Text.Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}