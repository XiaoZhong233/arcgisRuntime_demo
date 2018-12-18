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
using System.Windows.Shapes;

namespace WpfApp1.form.Query
{
    /// <summary>
    /// QueryShapeFileByAttr.xaml 的交互逻辑
    /// </summary>
    public partial class QueryShapeFileByAttr : Window
    {
        #region
        private List<PropertyNodeItem> propertyNodeItems;
        private List<FeatureLayer> featureLayers;
        private String sqlString; //查询语句的where子句
        private FeatureTable selectedTable;//当前所选要素表对象
        #endregion

        public QueryShapeFileByAttr(List<PropertyNodeItem> propertyNodeItems)
        {
            InitializeComponent();
            this.propertyNodeItems = propertyNodeItems;
            init();
            initControls();
        }

        private void init()
        {
            featureLayers = new List<FeatureLayer>();
            PropertyNodeItem.traverseNode(propertyNodeItems, x =>
            {
                if(x.nodeType == PropertyNodeItem.NodeType.LeafNode)
                {
                    featureLayers.Add(x.layer as FeatureLayer);
                }
            });
        }

        private void initControls()
        {
            comboBoxLayers.ItemsSource = featureLayers;
            comboBoxLayers.DisplayMemberPath = "DisplayName";
        }



        private void onOperationBtnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
