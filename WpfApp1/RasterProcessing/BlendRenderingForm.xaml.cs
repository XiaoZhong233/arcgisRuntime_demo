using Esri.ArcGISRuntime.Rasters;
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

namespace WpfApp1.Raster
{
    /// <summary>
    /// BlendRenderingForm.xaml 的交互逻辑
    /// </summary>
    public partial class BlendRenderingForm : Window
    {
        private PresetColorRampType predefineColorRampType;//预定义的颜色条带
        private SlopeType selSlopeType;//坡度类型
        private double curAltitude;//太阳高度角
        private double curAzimuth;//太阳方位角

        public PresetColorRampType PredefineColorRampType { get => predefineColorRampType; set => predefineColorRampType = value; }
        public SlopeType SelSlopeType { get => selSlopeType; set => selSlopeType = value; }
        public double CurAltitude { get => curAltitude; set => curAltitude = value; }
        public double CurAzimuth { get => curAzimuth; set => curAzimuth = value; }



        public BlendRenderingForm()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                buttonOK.IsEnabled = false;//OK按钮失效
                comboBoxSlopeType.ItemsSource = System.Enum.GetNames(typeof(SlopeType));//为坡度组合框添加项
                comboBoxColorRamp.ItemsSource = System.Enum.GetNames(typeof(PresetColorRampType));//为颜色条带组合框添加项
            };
            initControls();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void initControls()
        {
            buttonOK.Click += (s, e) =>
            {
                CurAltitude = sliderAltitude.Value;//取得滑动条的值作为太阳高度角的值
                CurAzimuth = sliderAzimuth.Value;//取得滑动条的值作为太阳方位角的值
                SelSlopeType = (SlopeType)System.Enum.Parse(typeof(SlopeType), comboBoxSlopeType.SelectedValue.ToString());
                if (comboBoxColorRamp.IsEnabled)
                {
                    PredefineColorRampType = 
                    (PresetColorRampType)System.Enum.Parse(typeof(PresetColorRampType), comboBoxColorRamp.SelectedValue.ToString());
                    this.DialogResult = true;//关闭对话框并返回true
                }
            };

            buttonCancel.Click += (s, e) =>
            {
                this.DialogResult = false;
            };

            sliderAltitude.ValueChanged += (s, e) =>
            {
                double curValue = sliderAltitude.Value;
                altitudeLabel.Content = "太阳高度角:" + curValue.ToString();
            };

            sliderAzimuth.ValueChanged += (s, e) =>
            {
                double curValue = sliderAzimuth.Value;
                azimuthLabel.Content = "太阳方位角:" + curValue.ToString();
            };

            comboBoxSlopeType.SelectionChanged += (s, e) =>
            {
                if (comboBoxSlopeType.SelectedIndex < 0) return;
                if (comboBoxColorRamp.SelectedIndex < 0) return;
                buttonOK.IsEnabled = true;
            };

            comboBoxColorRamp.SelectionChanged += (s, e) =>
            {
                if (comboBoxSlopeType.SelectedIndex < 0) return;
                if (comboBoxColorRamp.SelectedIndex < 0) return;
                buttonOK.IsEnabled = true;
            };
        }
        
    }
}
