// Copyright (c) 2014-2020 Sarin Na Wangkanai, All Rights Reserved.
// The Apache v2. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Wangkanai.Detection.Hosting;
using Wangkanai.Detection.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResponsiveBuilderExtensions
    {
        public static IDetectionBuilder AddResponsiveService(this IDetectionBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.TryAddScoped<IPreferenceService, PreferenceService>();
            builder.Services.TryAddTransient<IResponsiveService, ResponsiveService>();

            builder.Services.Configure<RazorViewEngineOptions>(
                options =>
                {
                    options.ViewLocationExpanders.Add(new ResponsiveViewLocationExpander(ResponsiveViewLocationFormat.Suffix));
                    options.ViewLocationExpanders.Add(new ResponsiveViewLocationExpander(ResponsiveViewLocationFormat.Subfolder));
                });

            builder.Services.ConfigureOptions<ResponsiveDefaultUIConfigurationOptions>();

            return builder;
        }
    }

    internal class ResponsiveDefaultUIConfigurationOptions : IPostConfigureOptions<RazorPagesOptions>
    {
        private const string              ResponsiveUIDefaultAreaName = "Responsive";
        public        IWebHostEnvironment Environment { get; }

        public ResponsiveDefaultUIConfigurationOptions(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void PostConfigure(string name, RazorPagesOptions options)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var convention = new ResponsivePageModelConvention();
            options.Conventions.AddAreaFolderApplicationModelConvention(
                ResponsiveUIDefaultAreaName,
                "/",
                pam => convention.Apply(pam));
        }
    }

    internal class ResponsivePageModelConvention : IPageApplicationModelConvention
    {
        public void Apply(PageApplicationModel model)
        {
            var defaultUIAttribute = model.ModelType.GetCustomAttribute<ResponsiveDefaultUIAttribute>();
            if (defaultUIAttribute == null)
                return;

            ValidateTemplate(defaultUIAttribute.Template);
            model.ModelType = defaultUIAttribute.Template.GetTypeInfo();
        }

        private void ValidateTemplate(Type template)
        {
            if (template.IsAbstract || template.IsGenericTypeDefinition)
                throw new InvalidOperationException("Implementation type can't be abstract or generic");
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class ResponsiveDefaultUIAttribute : Attribute
    {
        public Type Template { get; }

        public ResponsiveDefaultUIAttribute(Type implementationTemplate)
        {
            Template = implementationTemplate;
        }
    }
}