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
    /// QueryForm.xaml 的交互逻辑
    /// </summary>
    public partial class QueryForm : Window
    {
        #region 变量
        private IReadOnlyList<GeodatabaseFeatureTable> tables;//表集合
        private String sqlString; //查询语句的where子句
        private GeodatabaseFeatureTable selectedTable;//当前所选要素表对象
        #endregion

        #region 属性
        public IReadOnlyList<GeodatabaseFeatureTable> Tables { get => tables; set => tables = value; }
        public GeodatabaseFeatureTable SelectedTable { get => selectedTable; set => selectedTable = value; }
        #endregion

        #region 初始化
        public QueryForm(IReadOnlyList<GeodatabaseFeatureTable> tables)
        {
            InitializeComponent();
            init(tables);
            initFormEvents();
            initControlsEvents();
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        private void init(IReadOnlyList<GeodatabaseFeatureTable> tables)
        {
            if (tables != null)
            {
                Tables = tables;
            }
            else
            {
                Tables = new List<GeodatabaseFeatureTable>();//初始化表集合
            }
            sqlString = string.Empty;//初始化字符串
            selectedTable = null;
            listBoxFields.Items.Clear();//清空字段组合框的内容
            listBoxFieldValue.Items.Clear();//清空字段值组合框的内容
            textBoxSQL.Text = string.Empty;//清空文本框
        }

        /// <summary>
        /// 初始化窗体事件
        /// </summary>
        private void initFormEvents()
        {
            this.Loaded += (s, e) =>
            {
                if (Tables.Count > 0)
                {
                    foreach (GeodatabaseFeatureTable tb in Tables)
                    {
                        comboBoxLayers.Items.Add(tb.TableName);
                    }
                }
            };
        }

        /// <summary>
        /// 初始化控件事件
        /// </summary>
        private void initControlsEvents()
        {
            //cmb选择表事件回调
            comboBoxLayers.SelectionChanged += (s, e) =>
            {
                if (comboBoxLayers.SelectedIndex > -1)
                {
                    SelectedTable = Tables.ElementAt(comboBoxLayers.SelectedIndex);
                    if (SelectedTable != null)
                    {
                        listBoxFields.Items.Clear();//清空字段组合框内容
                        foreach (Field fld in selectedTable.Fields){//遍历表字段集合，将字段名加入
                            listBoxFields.Items.Add(fld.Name);
                        }
                    }
                }
            };

            //表名列表选择回调
            listBoxFields.SelectionChanged += (s, e) =>
            {

            };
        }
        #endregion




    }
}
