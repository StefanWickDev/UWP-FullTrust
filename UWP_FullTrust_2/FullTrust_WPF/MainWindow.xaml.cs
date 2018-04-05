using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FullTrust_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // display command line parameters, if any
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                List<ParameterItem> parameters = new List<ParameterItem>();
                for (int i=1; i<args.Length; i++)
                {
                    parameters.Add(new ParameterItem("arg" + i.ToString(), args[i]));
                }
                this.DataContext = parameters;
            }
        }
    }

    public class ParameterItem
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ParameterItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
