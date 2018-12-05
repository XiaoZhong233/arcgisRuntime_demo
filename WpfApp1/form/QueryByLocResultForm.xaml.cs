using Esri.ArcGISRuntime.Data;
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
    /// QueryByLocResultForm.xaml 的交互逻辑
    /// </summary>
    public partial class QueryByLocResultForm : Window
    {
        private IReadOnlyList<IdentifyLayerResult> resultCollection;//地图识别操作结果集合
        private bool isClosed;//窗体关闭标记
        private IdentifyLayerResult curSelLayer;//当前选择的图层

        public IReadOnlyList<IdentifyLayerResult> ResultCollection { get => resultCollection; set => resultCollection = value; }
        public bool IsClosed { get => isClosed; set => isClosed = value; }

        public QueryByLocResultForm()
        {
            InitializeComponent();
            initFormEvent();
            initControls();
        }

        private void initFormEvent()
        {
            this.Activated += (s, e) =>
            {
                if (resultCollection != null)
                {
                    foreach(IdentifyLayerResult result in resultCollection)
                    {
                        comboBoxLayer.Items.Add(result.LayerContent.Name);//添加图层名到图层组合框
                    }
                    IdentifyLayerResult firstLayerResult = resultCollection.First();
                    IReadOnlyCollection<GeoElement> geoElements = firstLayerResult.GeoElements;

                    comboBoxRecord.Items.Clear();
                    for (int i = 1; i <= geoElements.Count; i++)
                    {
                        comboBoxRecord.Items.Add(i);//遍历要素集合，为记录组合框添加项
                    }
                    Feature ft = (Feature)geoElements.First();//取得要素集合中第一个要素
                    dataGridContent.AutoGenerateColumns = true;//数据格网对象自动生成列
                    dataGridContent.ItemsSource = ft.Attributes;//设置数据格网中项的数据源
                }
            };

            this.Closed += (s, e) =>
            {
                IsClosed = true;
            };
            
        }


        private void initControls()
        {
            comboBoxLayer.SelectionChanged += (s, e) =>
            {
                if (comboBoxLayer.SelectedIndex > -1)
                {
                    curSelLayer = resultCollection[comboBoxLayer.SelectedIndex];//取得索引对应的要素图层
                    comboBoxRecord.Items.Clear();//清空记录组合框的内容
                    for (int i = 1; i <= curSelLayer.GeoElements.Count; i++)
                    {
                        comboBoxRecord.Items.Add(i);//遍历图层记录数，为记录组合框添加
                    }
                    Feature ft = (Feature)curSelLayer.GeoElements.First();
                    dataGridContent.ItemsSource = ft.Attributes;//设置数据格网对象中项的数据源
                }
            };

            comboBoxRecord.SelectionChanged += (s, e) =>
            {
                if (comboBoxRecord.SelectedIndex > -1 && curSelLayer!=null)
                {
                    Feature ft = (Feature)curSelLayer.GeoElements[comboBoxRecord.SelectedIndex];//获取索引所在要素
                    dataGridContent.ItemsSource = ft.Attributes;//设置数据格网对象中项的数据源
                }
            };
        }


    }
}
