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
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

            btn_clear.Click += (s, e) =>
            {
                textBoxSQL.Clear();
            };

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
            listBoxFields.SelectionChanged +=  (s, e) =>
            {
                if(SelectedTable!=null && listBoxFields.SelectedIndex > -1)
                {
                    Field selField = selectedTable.Fields.ElementAt(listBoxFields.SelectedIndex);
                    FeatureQueryResult fqr = selectedTable.QueryFeaturesAsync(new QueryParameters()).Result;
                    if (fqr == null)
                        return;
                    listBoxFieldValue.Items.Clear();
                    foreach(Feature ft in fqr)
                    {
                        //取得当前查询到的featureLayer的字段值
                        string value = ft.GetAttributeValue(selField).ToString();
                        //检查该值是否在listValue中
                        if (!listBoxFieldValue.Items.Contains(value))
                        {
                            listBoxFieldValue.Items.Add(value);
                        }
                    }
                }
            };

            //表名列表双击回调
            listBoxFields.MouseDoubleClick += (s, e) =>
            {
                string field = listBoxFields.SelectedItem.ToString();
                textBoxSQL.Text = string.Concat(textBoxSQL.Text + " ", field);
            };

            listBoxFieldValue.MouseDoubleClick += (s, e) =>
            {
                string selValue = listBoxFieldValue.SelectedItem.ToString();
                if(textBoxSQL.Text.Trim().EndsWith("AND") || textBoxSQL.Text.Trim().EndsWith("OR"))
                {
                    return;
                }
                //如果为模糊查询，则插入值在%号前面
                if (textBoxSQL.Text.Trim().EndsWith("%"))
                {
                    textBoxSQL.Text = textBoxSQL.Text.Insert(textBoxSQL.Text.Count() - 1, "'"+selValue+"'");
                    return;
                }

                textBoxSQL.Text = String.Concat(textBoxSQL.Text + " ", "'", selValue, "'");
            };

            btn_OK.Click += (s, e) =>
            {
                //setBusyOverLay(true);
                
                try
                {
                    
                    FeatureQueryResult result = query(textBoxSQL.Text.ToString());
                    
                    if (result != null)
                    {
                        setBusyOverLay(false);
                        QueryResultForm qrf = new QueryResultForm(result);
                        qrf.Show();
                        //setBusyOverLay(false);
                        this.DialogResult = true;
                        this.Close();
                    }
                }catch(Exception ex)
                {
                    //setBusyOverLay(false);
                    MessageBox.Show("表达式无效", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            btn_cancel.Click += (s, e) =>
            {
                this.DialogResult = false;
                this.Close();
            };

            
        }

        /// <summary>
        /// 操作符按钮点击处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onOperationBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
                return;
            switch (btn.Name)
            {
                case "equal":
                    concatSQLString("=");
                    break;
                case "unequal":
                    concatSQLString("<>");
                    break;
                case "like":
                    concatSQLString("like %%");
                    break;
                case "lessThan":
                    concatSQLString("<");
                    break;
                case "lessThanOrEqual":
                    concatSQLString("<=");
                    break;
                case "and":
                    concatSQLString("and");
                    break;
                case "moreThan":
                    concatSQLString(">");
                    break;
                case "moreThanOrEqual":
                    concatSQLString(">=");
                    break;
                case "or":
                    concatSQLString("or");
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 私有方法
        private void concatSQLString(string sql) 
        {
            textBoxSQL.Text = String.Concat(textBoxSQL.Text, " ", sql);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        private  FeatureQueryResult query(string sql)
        {
            if (selectedTable == null)
                return null;
            QueryParameters qParams = new QueryParameters();
            qParams.WhereClause = sql.Trim();
            FeatureQueryResult result = selectedTable.QueryFeaturesAsync(qParams).Result;
            return result;
        }

        
        private void setBusyOverLay(bool visible = true)
        {
            if (visible)
            {
                BusyOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                BusyOverlay.Visibility = Visibility.Collapsed;
            }
        }
        #endregion


    }
}
