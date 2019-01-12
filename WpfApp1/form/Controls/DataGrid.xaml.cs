using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
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

namespace WpfApp1.form.Controls
{
    /// <summary>
    /// DataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class DataGrid : UserControl
    {

        FeatureLayer layer;
        IReadOnlyList<Field> fields;

        public DataGrid(FeatureLayer layer)
        {
            InitializeComponent();
            this.layer = layer;
            init();
        }

        private void init()
        {
            fields = layer.FeatureTable.Fields;
        }
    }
}
