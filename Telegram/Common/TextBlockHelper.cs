using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Telegram.Common
{
    public static class TextBlockHelper
    {
        public static string GetMarkdown(DependencyObject obj)
        {
            return (string)obj.GetValue(MarkdownProperty);
        }

        public static void SetMarkdown(DependencyObject obj, string value)
        {
            obj.SetValue(MarkdownProperty, value);
        }

        public static readonly DependencyProperty MarkdownProperty =
            DependencyProperty.RegisterAttached("Markdown", typeof(string), typeof(TextBlockHelper), new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as TextBlock;
            var markdown = e.NewValue as string;

            sender.Inlines.Clear();

            var previous = 0;
            var index = markdown.IndexOf("**");
            var next = index > -1 ? markdown.IndexOf("**", index + 2) : -1;

            while (index > -1 && next > -1)
            {
                if (index - previous > 0)
                {
                    sender.Inlines.Add(new Run { Text = markdown.Substring(previous, index - previous) });
                }

                sender.Inlines.Add(new Run { Text = markdown.Substring(index + 2, next - index - 2), FontWeight = FontWeights.SemiBold });

                previous = next + 2;
                index = markdown.IndexOf("**", next + 2);
                next = index > -1 ? markdown.IndexOf("**", index + 2) : -1;
            }

            if (markdown.Length - previous > 0)
            {
                sender.Inlines.Add(new Run { Text = markdown.Substring(previous, markdown.Length - previous) });
            }
        }
    }
}
