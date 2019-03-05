using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AddressCoding.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ButtonStartStop.xaml
    /// </summary>
    public partial class ButtonStartStop : UserControl
    {
        public ButtonStartStop()
        {
            InitializeComponent();
        }

        public string TextStart
        {
            get { return (string)GetValue(TextStartProperty); }
            set { SetValue(TextStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextStartProperty =
            DependencyProperty.Register("TextStart", typeof(string), typeof(ButtonStartStop), new PropertyMetadata(null));

        public string TextStop
        {
            get { return (string)GetValue(TextStopProperty); }
            set { SetValue(TextStopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextStop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextStopProperty =
            DependencyProperty.Register("TextStop", typeof(string), typeof(ButtonStartStop), new PropertyMetadata(null));

        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(ButtonStartStop), new PropertyMetadata(false));

        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("ShowProgress", typeof(bool), typeof(ButtonStartStop), new PropertyMetadata(false));

        public int Percent
        {
            get { return (int)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register("Percent", typeof(int), typeof(ButtonStartStop), new PropertyMetadata(0));

        public ICommand CommandStart
        {
            get { return (ICommand)GetValue(CommandStartProperty); }
            set { SetValue(CommandStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandStartProperty =
            DependencyProperty.Register("CommandStart", typeof(ICommand), typeof(ButtonStartStop), new PropertyMetadata(null));

        public ICommand CommandStop
        {
            get { return (ICommand)GetValue(CommandStopProperty); }
            set { SetValue(CommandStopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandStop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandStopProperty =
            DependencyProperty.Register("CommandStop", typeof(ICommand), typeof(ButtonStartStop), new PropertyMetadata(null));
    }
}