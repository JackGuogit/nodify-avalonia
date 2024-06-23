using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace Nodify.Shapes;

public class TemplateAsDataTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is Template template)
        {
            return template.Build();
        }

        return null;
    }

    public bool Match(object? data)
    {
        return data is Template;
    }
}