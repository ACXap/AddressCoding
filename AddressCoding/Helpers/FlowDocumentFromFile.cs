using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace AddressCoding
{
    /// <summary>
    /// Класс для описания поведения всплывающего окна помощи
    /// </summary>
    class FlowDocumentFromFile : Behavior<FlowDocumentScrollViewer>
    {
        public string FileName
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FlowDocumentFromFile), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            SetDoc();
        }

        private void SetDoc()
        {
            FlowDocumentScrollViewer control = AssociatedObject;

            if (control == null)
            {
                throw new Exception(Application.Current.FindResource("csFlyoutHelpError") as string);
            }

            Paragraph paragraph = new Paragraph();
            if (File.Exists(FileName))
            {
                paragraph.Inlines.Add(File.ReadAllText(FileName, Encoding.Default));
            }
            else
            {
                paragraph.Inlines.Add($"{Application.Current.FindResource("csFlyoutHelpErrorTxt")} {FileName}");
            }

            control.Document = new FlowDocument(paragraph);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}