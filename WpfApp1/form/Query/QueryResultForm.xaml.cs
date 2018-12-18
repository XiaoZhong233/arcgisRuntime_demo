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

namespace WpfApp1.form
{
    /// <summary>
    /// QueryResultForm.xaml 的交互逻辑
    /// </summary>
    public partial class QueryResultForm : Window
    {
        private FeatureQueryResult featureQuerySet; //查询结果集合

        public FeatureQueryResult FeatureQuerySet { get => featureQuerySet; set => featureQuerySet = value; }

        #region 初始化
        public QueryResultForm(FeatureQueryResult fqs)
        {
            InitializeComponent();
            init(fqs);
            initFormEvent();
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        /// <param name="fqs"></param>
        private void init(FeatureQueryResult fqs)
        {
            FeatureQuerySet = fqs;
        }




        private void initFormEvent()
        {

            this.Loaded += (s, e) =>
            {
                //MessageBox.Show("");
                if (featureQuerySet != null)
                {
                    grid_result.AutoGenerateColumns = true;
                    for(int i = 0; i < featureQuerySet.Count(); i++)
                    {
                        cb_result.Items.Add(i);
                    }
                    cb_result.SelectedIndex = 0;
                    Feature feature = featureQuerySet.First();
                    grid_result.ItemsSource = feature.Attributes;
                    //FeatureLayer featureLayer = new FeatureLayer(feature.FeatureTable);
                    //MainWindow.mainwindow.MyMapView.Map.OperationalLayers.Add(featureLayer);
                    //feature.FeatureTable.FeatureLayer.SelectFeature(feature);                
                    MainWindow.mainwindow.MyMapView.SetViewpointGeometryAsync(feature.Geometry);
                    try
                    {
                        feature.FeatureTable.FeatureLayer.SelectionColor = System.Drawing.Color.Red;
                        //如果数据有问题，可能造成表缺失
                        feature.FeatureTable.FeatureLayer.SelectFeature(feature);
                    }
                    catch(Exception ee)
                    {
                        
                    }

                }
            };
        }
        #endregion

    }
}
