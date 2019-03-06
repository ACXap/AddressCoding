using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AddressCoding.UserControls
{
    /// <summary>
    /// Логика взаимодействия для WorkIndication.xaml
    /// </summary>
    public partial class WorkIndication : UserControl
    {
        public WorkIndication()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WorkIndication), new PropertyMetadata(null));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(WorkIndication), new PropertyMetadata(false));

        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Brush), typeof(WorkIndication), new PropertyMetadata(Brushes.Red));

        public string TextAlternative
        {
            get { return (string)GetValue(TextAlternativeProperty); }
            set { SetValue(TextAlternativeProperty, value); }
        }

        public static readonly DependencyProperty TextAlternativeProperty =
            DependencyProperty.Register("TextAlternative", typeof(string), typeof(WorkIndication), new PropertyMetadata(null));

        public bool IsShowTextAlternative
        {
            get { return (bool)GetValue(IsShowTextAlternativeProperty); }
            set { SetValue(IsShowTextAlternativeProperty, value); }
        }

        public static readonly DependencyProperty IsShowTextAlternativeProperty =
            DependencyProperty.Register("IsShowTextAlternative", typeof(bool), typeof(WorkIndication), new PropertyMetadata(false));
    }
}